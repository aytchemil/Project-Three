using UnityEngine;
using System.Collections.Generic;
using static AM;
using System.Runtime.ConstrainedExecution;
using System;
using static UnityEngine.EventSystems.EventTrigger;

public static class EntityControllerFactory 
{
    private static List<GameObject> activeEntities = new List<GameObject>();


    public static GameObject SpawnEntity(GameObject entityPrefab, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        if(entityPrefab == null) return null;

        GameObject entity = UnityEngine.Object.Instantiate(entityPrefab, pos, rot, parent);
        activeEntities.Add(entity);

        return entity;
    }

    public static GameObject SpawnEntityPremade(GameObject entityPrefab, List<ModeData> modes,  Weapon wpn, List<AbilitySet> abilitySets, Vector3 pos, Quaternion rot, Transform parent = null)
    {
        Type[] animations = new Type[] {
                typeof(MoveAnims),
                typeof(AtkAnims),
                typeof(BlkAnims)
        };

        CyclePackage idleCycle = new(
               (int)MoveAnims.Anims.IDLE1,
               12f,
               MoveAnims.idles.Length
        );

        Debug.Log("Spawning Entity...");

        GameObject newEntity = new EntityControllerBuilder(entityPrefab)
            .WithCombatFunctionality()
            .WithDIAbilitySets(abilitySets)
            .WithDIModes(modes)
            .WithWeaponAndWeaponManager(wpn)
            .WithCharacterAnimController(animations, wpn.layerCount)
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

        public EntityControllerBuilder(GameObject entityPrefab)
        {
            newEntity = GameObject.Instantiate(entityPrefab);
            entity = newEntity.GetComponent<EntityController>();
            entity.mode = "Attack";
            Debug.Log($"Instantiated New Entity");
        }

        public EntityControllerBuilder WithCombatFunctionality()
        {
            CombatFunctionality cf = entity.gameObject.AddComponent<CombatFunctionality>();
            Debug.Log($"Added Combat Functionality"); 
            return this;
        }

        public EntityControllerBuilder WithDIAbilitySets(List<AbilitySet> abilitySets)
        {
            entity.abilitySetInputs = abilitySets;

            Debug.Log("2");

            return this;
        }

        public EntityControllerBuilder WithDIModes(List<ModeData> modes)
        {
            entity.Init(modes); 
            return this;
        }


        public EntityControllerBuilder WithWeaponAndWeaponManager(Weapon wpn)
        {
            this.wpn = wpn;
            wpnManager = newEntity.AddComponent<WeaponManager>();
            wpnManager.Init(wpn, entity.animControllerParent);
            Debug.Log($"Added Weapon ({wpn}) and WeaponManager");

            return this;
        }

        public EntityControllerBuilder WithCharacterAnimController(Type[] animations, int layerCount)
        {
            animController = (CharacterAnimationController)entity.animControllerParent.gameObject.AddComponent(typeof(CharacterAnimationController));
            entity.animControllerParent.gameObject.name = entity.gameObject.name + " " + entity.animControllerParent.gameObject.name;
            animController.animations = new Type[]
            {
                typeof(MoveAnims),
                typeof(AtkAnims),
                typeof(BlkAnims)
            };

            animController.Init(layerCount, entity, wpn.animationController);
            Debug.Log($"Added AnimationController And Animations"); 


            return this;
        }
        public EntityControllerBuilder WithAnimCyclePackage(string name, AM.CyclePackage pckg)
        {
            animController.CyclePackages.Add(name, pckg); 

            Debug.Log($"Added an Anim Cycle Package");
            return this;
        }


        public GameObject Build()
        {
            Debug.Log("Built New Entity");
            return newEntity;
        }

    }
}
