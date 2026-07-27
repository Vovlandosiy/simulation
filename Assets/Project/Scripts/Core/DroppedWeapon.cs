using UnityEngine;

public class DroppedWeapon : MonoBehaviour
{
    public WeaponData weaponData;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (weaponData != null && weaponData.weaponSprite != null)
        {
            spriteRenderer.sprite = weaponData.weaponSprite;
        }
    }

    // Обработка подбора через триггер
    private void OnTriggerEnter2D(Collider2D other)
    {
        Pawn pawn = other.GetComponent<Pawn>();
        if (pawn != null)
        {
            // Пытаемся дать оружие юниту
            if (pawn.TryEquipWeapon(weaponData))
            {
                // Если юнит успешно подобрал, удаляем пушку с земли
                Destroy(gameObject);
            }
        }
    }
}
