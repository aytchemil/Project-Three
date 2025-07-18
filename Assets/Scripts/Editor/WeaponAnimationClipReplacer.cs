#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using static AM;


public class WeaponClipRenameAndReplace : EditorWindow
{
    public AnimatorController animatorController;
    public DefaultAsset weaponFolder; // e.g. SWORD folder with UPPER/LOWER subfolders
    public string oldPrefix = "AXE_";
    public string newPrefix = "SWORD_";

    private HashSet<string> validSlotNames;

    [MenuItem("Tools/Weapon Clips/Rename + Replace")]
    public static void ShowWindow()
    {
        GetWindow<WeaponClipRenameAndReplace>("Weapon Rename + Replace");
    }

    void OnGUI()
    {
        GUILayout.Label("Weapon Clip Renamer + Replacer", EditorStyles.boldLabel);

        weaponFolder = (DefaultAsset)EditorGUILayout.ObjectField("Weapon Folder", weaponFolder, typeof(DefaultAsset), false);
        animatorController = (AnimatorController)EditorGUILayout.ObjectField("Animator Controller", animatorController, typeof(AnimatorController), false);

        oldPrefix = EditorGUILayout.TextField("Old Prefix", oldPrefix);
        newPrefix = EditorGUILayout.TextField("New Prefix", newPrefix);

        if (GUILayout.Button("Rename Clips + Replace Animator"))
        {
            if (weaponFolder == null || animatorController == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a weapon folder AND an AnimatorController", "OK");
                return;
            }

            // Build enum names for valid slots
            BuildValidSlotNames();

            // Step 1: Rename all clips in folder
            RenameClipsInFolder();

            // Step 2: Replace Animator states with renamed clips
            ReplaceAnimatorClips();
        }
    }

    void BuildValidSlotNames()
    {
        validSlotNames = new HashSet<string>();

        foreach (var n in System.Enum.GetNames(typeof(MoveAnims.Anims))) if (n != "NONE") validSlotNames.Add(n);
        foreach (var n in System.Enum.GetNames(typeof(AtkAnims.Anims))) if (n != "NONE") validSlotNames.Add(n);
        foreach (var n in System.Enum.GetNames(typeof(BlkAnims.Anims))) if (n != "NONE") validSlotNames.Add(n);
    }

    void RenameClipsInFolder()
    {
        string folderPath = AssetDatabase.GetAssetPath(weaponFolder);

        // Find all AnimationClips recursively
        string[] clipGuids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folderPath });
        int renamedCount = 0;

        foreach (string guid in clipGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            string filename = Path.GetFileNameWithoutExtension(assetPath);

            if (filename.StartsWith(oldPrefix))
            {
                string newName = newPrefix + filename.Substring(oldPrefix.Length);
                AssetDatabase.RenameAsset(assetPath, newName);
                renamedCount++;
                Debug.Log($"Renamed {filename} → {newName}");
            }
        }

        if (renamedCount > 0)
        {
            AssetDatabase.SaveAssets();
            Debug.Log($"Renamed {renamedCount} clips in folder {folderPath}");
        }
        else
        {
            Debug.Log("No clips needed renaming");
        }
    }

    void ReplaceAnimatorClips()
    {
        string folderPath = AssetDatabase.GetAssetPath(weaponFolder);

        // Load all new clips recursively after renaming
        var clipGuids = AssetDatabase.FindAssets("t:AnimationClip", new[] { folderPath });
        var newWeaponClips = clipGuids
            .Select(guid => AssetDatabase.LoadAssetAtPath<AnimationClip>(AssetDatabase.GUIDToAssetPath(guid)))
            .ToDictionary(c => c.name, c => c);

        int replacedCount = 0;

        foreach (var layer in animatorController.layers)
        {
            string layerName = layer.name.ToUpper();

            foreach (var state in layer.stateMachine.states)
            {
                if (state.state.motion is AnimationClip)
                {
                    string stateName = state.state.name; // e.g. IDLE3, FORWARD, Atk_Diag_L

                    if (!validSlotNames.Contains(stateName)) continue;

                    string expectedClipName = BuildExpectedClipName(layerName, stateName);

                    if (newWeaponClips.TryGetValue(expectedClipName, out var newClip))
                    {
                        state.state.motion = newClip;
                        replacedCount++;
                        Debug.Log($"Replaced {stateName} → {expectedClipName}");
                    }
                    else
                    {
                        Debug.LogWarning($"No matching clip found for {expectedClipName}");
                    }
                }
            }
        }

        EditorUtility.SetDirty(animatorController);
        AssetDatabase.SaveAssets();

        EditorUtility.DisplayDialog("Done", $"Animator replaced {replacedCount} clips!", "OK");
    }

    string BuildExpectedClipName(string layerName, string stateName)
    {
        // Attacks & Blocks stay the same (no U_/L_)
        if (stateName.StartsWith("Atk_") || stateName.StartsWith("Block_"))
            return newPrefix + stateName;

        // Movement/Idle need U_/L_ based on layer
        string ulPrefix = "";
        if (layerName.Contains("UPPER")) ulPrefix = "U_";
        else if (layerName.Contains("LOWER")) ulPrefix = "L_";

        return newPrefix + ulPrefix + stateName; // e.g. SWORD_U_FORWARD, SWORD_L_IDLE1
    }
}


public class DebugAnimatorClips : EditorWindow
{
    public AnimatorController animatorController;

    [MenuItem("Tools/Debug/Print Animator Clips")]
    public static void ShowWindow()
    {
        GetWindow<DebugAnimatorClips>("Debug Animator Clips");
    }

    void OnGUI()
    {
        animatorController = (AnimatorController)EditorGUILayout.ObjectField("Animator Controller", animatorController, typeof(AnimatorController), false);

        if (GUILayout.Button("Print All Clips"))
        {
            if (animatorController == null)
            {
                EditorUtility.DisplayDialog("Error", "Assign an AnimatorController", "OK");
                return;
            }

            PrintAllClips();
        }
    }

    void PrintAllClips()
    {
        Debug.Log($"--- Printing all clips in {animatorController.name} ---");

        foreach (var layer in animatorController.layers)
        {
            Debug.Log($"Layer: {layer.name}");

            foreach (var childState in layer.stateMachine.states)
            {
                var state = childState.state;
                Debug.Log($"  State: {state.name}");

                if (state.motion is AnimationClip clip)
                {
                    Debug.Log($"    → Clip: {clip.name}");
                }
                else if (state.motion is BlendTree bt)
                {
                    Debug.Log($"    → BlendTree: {bt.name}");
                }
                else
                {
                    Debug.Log($"    → No direct clip");
                }
            }
        }
    }
}

#endif