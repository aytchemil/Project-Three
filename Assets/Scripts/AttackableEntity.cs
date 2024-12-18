using UnityEngine;

public class AttackbleEntity : MonoBehaviour
{
    [SerializeField] private float _health;
    public float health
    {
        get => _health;
        set
        {
            Debug.Log("Setting Health");
            _health = value;
        }
    }

    public GameObject attackedEffect;
    
    public virtual void Attacked(Ability atkedWithAbility)
    {

    }



    //Tick damage over a certain time?


}
