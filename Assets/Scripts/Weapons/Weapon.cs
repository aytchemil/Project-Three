using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "ScriptableObjects/Weapon")]
public class Weapon : ScriptableObject
{
    public Sprite icon;
    public string name;
    public string animHeirarchName;
    public GameObject prefab;
    public RuntimeAnimatorController animationController;
    public int layerCount;

    public List<AbilitySet> allAbilitySets;


}
