using UnityEngine;

[CreateAssetMenu(fileName = "NewWeaponData", menuName = "Simulation/Weapon Data")]
public class WeaponData : ScriptableObject
{
    public string weaponName = "Оружие";
    public int ammo = 1;
    public int damage = 2;
    public float fireRange = 6f;
    public float bulletSpeed = 12f; 
    public float aimDuration = 0.8f; 
    public Sprite weaponSprite;
    
    [Header("Префаб снаряда для этого оружия")]
    public GameObject bulletPrefab; // Сюда для Пистолета перетащите Bullet, а для РПГ — префаб Ракеты
}
