using UnityEngine;

public class WeaponHandler : MonoBehaviour
{
    public Weapon currentWeapon;
    public Bootstrapper bootstrapper;

    public void SetWeapon(Weapon weapon)
    {
        currentWeapon = weapon;
        Debug.Log($"[WeaponHandler] Equipped Weapon: {weapon.name}");
        bootstrapper.wpn = weapon;
    }
}