using UnityEngine;
using System.Collections.Generic;
using static AM;
using System;

public static class EntityControllerFactory 
{
    private static GameObject debugTextObj;
    private static TextMesh debugTextMesh;

    private static void DebugStepMarker(int step)
    {
        // Create it only once
        if (debugTextObj == null)
        {
            debugTextObj = new GameObject("DebugStepText");
            debugTextMesh = debugTextObj.AddComponent<TextMesh>();

            // Transform
            debugTextObj.transform.position = new Vector3(25f, 15f, -25f);
            debugTextObj.transform.rotation = Quaternion.identity;
            debugTextObj.transform.localScale = new Vector3(6f, 6f, 6f);

            // Style
            debugTextMesh.characterSize = 0.5f;
            debugTextMesh.fontStyle = FontStyle.Bold;
            debugTextMesh.anchor = TextAnchor.MiddleCenter;
            debugTextMesh.alignment = TextAlignment.Center;
            debugTextMesh.color = Color.cyan;
        }

        // Just update the text
        debugTextMesh.text = $"STEP {step}";
    }

    private static List<GameObject> activeEntities = new List<GameObject>();


    public static GameObject SpawnEntity(GameObject entityPrefab, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        if(entityPrefab == null) return null;

        GameObject entity = UnityEngine.Object.Instantiate(entityPrefab, pos, rot, parent);
        activeEntities.Add(entity);

        return entity;
    }

    public static GameObject SpawnEntityPremade(GameObject entityPrefab, List<ModeData> modes, Type[] Anims,  Weapon wpn, List<AbilitySet> abilitySets, Vector3 pos, Quaternion rot, Transform parent = null)
    {

        CyclePackage idleCycle = new(
               (int)MoveAnims.Anims.IDLE1,
               12f,
               MoveAnims.idles.Length
        );

        Debug.Log("Spawning Entity...");

        GameObject newEntity = new EntityControllerBuilder(entityPrefab, pos, rot, parent)
            .WithCombatFunctionality()
            .WithDIAbilitySets(abilitySets)
            .WithDIModes(modes)
            .WithWeaponAndWeaponManager(wpn)
            .WithCharacterAnimController(Anims, wpn.layerCount, Anims)
            .WithAnimCyclePackage("idle", idleCycle)
            .Build();

        return newEntity;
    }


    public class EntityControllerBuilder
    {
        GameObject newEntity;
        EntityController entity;
        CharacterAnimationController animController;
        WeaponManager wpnManager;
        Weapon wpn;
        int animLayerCount;

        public EntityControllerBuilder(GameObject entityPrefab, Vector3 worldPos, Quaternion worldRot, Transform parent = null)
        {
            Quaternion finalRot = worldRot * Quaternion.Euler(0f, 180f, 0f);
            newEntity = GameObject.Instantiate(entityPrefab, worldPos, finalRot, parent); entity = newEntity.GetComponent<EntityController>();
            entity.mode = "Attack";
            Debug.Log("Instantiated New Entity");
            DebugStepMarker(1);
        }


        public EntityControllerBuilder WithCombatFunctionality()
        {
            CombatFunctionality cf = entity.gameObject.AddComponent<CombatFunctionality>();
            Debug.Log($"Added Combat Functionality");

            DebugStepMarker(2);
            return this;
        }

        public EntityControllerBuilder WithDIAbilitySets(List<AbilitySet> abilitySets)
        {
            entity.abilitySetInputs = abilitySets;
            Debug.Log("Ability Sets Injected");

            DebugStepMarker(3);
            return this;
        }

        public EntityControllerBuilder WithDIModes(List<ModeData> modes)
        {
            entity.Init(modes);
            Debug.Log("Modes Injected");

            DebugStepMarker(4);
            return this;
        }

        public EntityControllerBuilder WithWeaponAndWeaponManager(Weapon wpn)
        {
            this.wpn = wpn;
            wpnManager = newEntity.AddComponent<WeaponManager>();
            wpnManager.Init(wpn, entity.animControllerParent);
            Debug.Log($"Added Weapon ({wpn}) and WeaponManager");

            DebugStepMarker(5);
            return this;
        }

        public EntityControllerBuilder WithCharacterAnimController(Type[] animations, int layerCount, Type[] Anims)
        {
            animController = entity.animControllerParent.gameObject.AddComponent<CharacterAnimationController>();
            entity.animControllerParent.gameObject.name = entity.gameObject.name + " " + entity.animControllerParent.gameObject.name;
            animController.animations = Anims;
            animController.Init(layerCount, entity, wpn.animationController);

            Debug.Log($"Added AnimationController And Animations");
            DebugStepMarker(6);

            return this;
        }

        public EntityControllerBuilder WithAnimCyclePackage(string name, AM.CyclePackage pckg)
        {
            animController.CyclePackages.Add(name, pckg);
            Debug.Log($"Added an Anim Cycle Package");

            DebugStepMarker(7);
            return this;
        }


        public GameObject Build()
        {
            Debug.Log("Built New Entity");


            return newEntity;
        }

    }
}
