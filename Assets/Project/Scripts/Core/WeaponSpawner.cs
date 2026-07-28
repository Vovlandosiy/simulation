using UnityEngine;

public class WeaponSpawner : MonoBehaviour
{
    [Header("Префаб оружия для спавна")]
    public GameObject weaponPrefab;

    [Header("Настройки времени")]
    public float spawnInterval = 5f; // Каждые 5 секунд спавнить пушку

    [Header("Настройки лимита")]
    [SerializeField] private int maxWeaponsCount = 2; // Максимальное количество пушек на сцене

    [Header("Зона спавна")]
    public Vector2 spawnAreaSize = new Vector2(4f, 6f); // Размеры прямоугольника внутри арены

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
            // Проверяем, можно ли спавнить
            if (CanSpawn())
            {
                SpawnWeapon();
            }
            
            timer = spawnInterval; // Сброс таймера происходит в любом случае, чтобы проверить снова через 5 сек
        }
    }

    private bool CanSpawn()
    {
        if (weaponPrefab == null) return false;

        // Получаем WeaponData, которое должен спавнить ЭТОТ конкретный спавнер
        if (!weaponPrefab.TryGetComponent<DroppedWeapon>(out var prefabWeaponScript) || prefabWeaponScript.weaponData == null)
        {
            return false;
        }
        
        WeaponData targetData = prefabWeaponScript.weaponData;

        // Находим вообще все лежащие пушки на арене
        DroppedWeapon[] allWeaponsOnGround = FindObjectsByType<DroppedWeapon>(FindObjectsSortMode.None);
        
        int sameTypeCount = 0;

        // Считаем только те пушки, у которых совпадает ScriptableObject данные
        for (int i = 0; i < allWeaponsOnGround.Length; i++)
        {
            if (allWeaponsOnGround[i].weaponData == targetData)
            {
                sameTypeCount++;
            }
        }

        // Если пушек именно ЭТОГО типа уже слишком много — спавн отменяется
        return sameTypeCount < maxWeaponsCount;
    }


    private void SpawnWeapon()
    {
        // Генерируем случайную точку внутри зоны спавнера
        float randomX = Random.Range(-spawnAreaSize.x / 2f, spawnAreaSize.x / 2f);
        float randomY = Random.Range(-spawnAreaSize.y / 2f, spawnAreaSize.y / 2f);
        Vector3 spawnPosition = transform.position + new Vector3(randomX, randomY, 0f);

        // Спавним пушку на арене
        Instantiate(weaponPrefab, spawnPosition, Quaternion.identity);
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
