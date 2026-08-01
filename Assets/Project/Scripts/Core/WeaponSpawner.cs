using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Weapon Prefab")]
    public GameObject weaponPrefab;

    [Header("Weapon Data")]
    public WeaponData weaponData; 

    [Header("Spawn Interval")]
    public float spawnInterval = 5f;

    [Header("Max Weapon Count")]
    [SerializeField] private int maxWeaponsCount = 2; 

    [Header("Spawn Area Size")]
    public Vector2 spawnAreaSize = new Vector2(4f, 6f); 

    private float timer;

    void Start()
    {
        timer = spawnInterval;
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            if (CanSpawn())
            {
                SpawnWeapon();
            }
            
            timer = spawnInterval; // Сброс таймера происходит в любом случае, чтобы проверить снова через 5 сек
        }
    }

    private bool CanSpawn()
    {
        if (weaponPrefab == null || weaponData == null) return false;

        DroppedWeapon[] allWeaponsOnGround = FindObjectsByType<DroppedWeapon>(FindObjectsSortMode.None);
        
        int sameTypeCount = 0;

        // 3. Исправленный цикл: считаем пушки с таким же ScriptableObject
        for (int i = 0; i < allWeaponsOnGround.Length; i++)
        {
            if (allWeaponsOnGround[i].weaponData == weaponData)
            {
                sameTypeCount++;
            }
        }
        return sameTypeCount < maxWeaponsCount;
    }

private void SpawnWeapon()
{
    float randomX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
    float randomY = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);
    
    // ИЗМЕНЕНО: Явно берем transform.position.z спавнера, чтобы пушка не спавнилась в глубине сцены
    Vector3 spawnPosition = new Vector3(transform.position.x + randomX, transform.position.y + randomY, transform.position.z);

    GameObject spawnedObject = Instantiate(weaponPrefab, spawnPosition, Quaternion.identity);
    if (spawnedObject.TryGetComponent<DroppedWeapon>(out var droppedWeapon))
    {
        // Сначала передаем данные, только потом пушка будет готова к подбору
        droppedWeapon.Initialize(weaponData);
    }
}

    // Визуальные границы спавнера в окне Scene
    private void OnDrawGizmos()
    {
        // Рисуем заполненный полупрозрачный желтый прямоугольник
        Gizmos.color = new Color(1f, 0.92f, 0.016f, 0.15f); // Желтый с альфа-каналом 15%
        Gizmos.DrawCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0f));

        // Рисуем его плотные границы (контур)
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position, new Vector3(spawnAreaSize.x, spawnAreaSize.y, 0f));
    }
}