using UnityEngine;

public class DroppedWeapon : MonoBehaviour
{
    [HideInInspector] public WeaponData weaponData; 
    
    private SpriteRenderer spriteRenderer;

    [Header("Juice")]
    [SerializeField] private float pulseSpeed = 3f;      
    [SerializeField] private float pulseAmount = 0.15f;  

    private Vector3 baseScale; 
    private bool isInitialized = false;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        baseScale = transform.localScale; 
    }

    public void Initialize(WeaponData data)
    {
        weaponData = data;

        if (weaponData != null && weaponData.weaponSprite != null)
        {
            spriteRenderer.sprite = weaponData.weaponSprite;

            transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0f);
        }

         isInitialized = true;
    }

    void Update()
    {
        if (!isInitialized) return;

        float scaleFactor = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = baseScale * scaleFactor;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isInitialized || weaponData == null) return; 

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
