using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Simulation/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "Пистолет";
    public int ammo = 1;
    public int damage = 2;
    public float fireRange = 6f;
    public float bulletSpeed = 12f; 
    public float aimDuration = 0.8f; 
    public Sprite weaponSprite;
}
