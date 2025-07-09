using UnityEngine;

[System.Serializable]
public class AF_movement : AF
{
    public override string afname => "movement";
    [SerializeField] public float movementAmount;

    public AF_movement()
    {
        movementAmount = 0;
    }

    public AF_movement(float movementAmount)
    {
        this.movementAmount = movementAmount;
    }
}