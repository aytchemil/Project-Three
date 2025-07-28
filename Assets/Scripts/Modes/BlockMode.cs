using System.Collections;
using UnityEngine;
using static EntityController;

public class BlockMode : MonoBehaviour, ICombatMode
{
    public CombatFunctionality cf { get; set; }
    public string MODE { get => "Block"; }
    public RuntimeModeData Mode { get => cf.Controls.Mode(MODE); }

    public bool readyToUnblock = false;
    private void OnEnable()
    {
        if (cf == null)
            cf = gameObject.GetComponent<CombatFunctionality>();
        if (cf.Controls == null)
            cf.Controls = cf.gameObject.GetComponent<EntityController>();

        //Waiting for EntityController to initialize
        if (cf.Controls.initialized == false)
        {
            WaitExtension.WaitAFrame(this, OnEnable);
            return;
        }

        //Null Guards
        if (cf == null)
            Debug.LogError($"[{gameObject.name}] CF not properly set");
        if (cf.Controls == null)
            Debug.LogError($"[{gameObject.name}] Controls not properly set");
        if (Mode == null)
            Debug.LogError($"[{gameObject.name}] Mode Reference not getting");

        InitializeFunctionalityAfterOnEnable();
    }


    public void InitializeFunctionalityAfterOnEnable()
    {
        print(cf.Controls);
        cf.Controls.CombatWheelSelectDirection += ChangeBlock;
        print(cf.Controls.CombatWheelSelectDirection);

        if (Mode != null && Mode.functionality != null)
            cf.Controls.blockStop += Mode.functionality.Finish;
        print(cf.Controls.blockStop);

        cf.Controls.EnterCombat += EnterCombatAutoBlock;
    }

    private void OnDisable()
    {
        if (cf == null)
            cf = gameObject.GetComponent<CombatFunctionality>();
        cf.Controls.CombatWheelSelectDirection -= ChangeBlock;
        cf.Controls.blockStop -= Mode.functionality.Finish;

        cf.Controls.EnterCombat -= EnterCombatAutoBlock;

    }

    IEnumerator WaitToStartBlockingToUnblock(AbilityBlock ability)
    {
        readyToUnblock = false;
        yield return new WaitForSeconds(ability.defaultBlockTimeToBlocking);
        readyToUnblock = true;
    }

    void ChangeBlock(string dir)
    {
        //print($"[{gameObject.name}] BLOCK Direction Changing to {dir}");
        ChangeBlockAbility(dir);
        cf.Controls.useAbility?.Invoke("Block");

    }

    /// <summary>
    /// Changes the current block ability in the ModeData
    /// </summary>
    /// <param name="dir"></param>
    public void ChangeBlockAbility(string dir)
    {
        //print($"[{gameObject.name}] Blocking {dir}");
        //Setup
        RuntimeModeData d = cf.Controls.Mode("Block");


        //Mutations
        if (dir == "right")
            d.ability = d.data.abilitySet.right;
        else if(dir == "left")
            d.ability = d.data.abilitySet.left;
        else if (dir == "up")
            d.ability = d.data.abilitySet.up;
        else if (dir == "down")
            d.ability = d.data.abilitySet.down;
    }

    public void UseModeImplementation()
    {
        AbilityBlock ability = (AbilityBlock)Mode.ability;
        ability.Use(this, cf, Mode);
    }


    void EnterCombatAutoBlock()
    {
        Mode.functionality.UseMode();
    }

    /// <summary>
    /// ADDITIONAL FUNCTIONALITY (Type: Regular)
    /// </summary>
    /// <param name="ability"></param>
    void AF_Regular(AbilityBlock ability)
    {
        //print("[BlockMode] AF Regular");
    }
}
