using UnityEngine;

public class DroppedWeapon : MonoBehaviour
{
    public WeaponData weaponData;
    private SpriteRenderer spriteRenderer;

    [Header("Настройки пульсации (Juice)")]
    [SerializeField] private float pulseSpeed = 3f;      // Скорость изменения размера
    [SerializeField] private float pulseAmount = 0.15f;  // На сколько процентов увеличивать/уменьшать (0.15 = 15%)

    private Vector3 baseScale; // Базовый размер пушки, взятый из инспектора

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale; // Запоминаем исходный размер
    }

    void Start()
    {
        if (weaponData != null && weaponData.weaponSprite != null)
        {
            spriteRenderer.sprite = weaponData.weaponSprite;
        }
    }

    void Update()
    {
        // Плавная пульсация масштаба на основе синусоиды времени
        float scaleFactor = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * scaleFactor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Unit unit = other.GetComponent<Unit>();
        if (unit != null)
        {
            if (unit.TryEquipWeapon(weaponData))
            {
                Destroy(gameObject);
            }
        }
    }
}
