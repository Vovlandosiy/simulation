using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [Header("Настройки взрыва (Только для Ракет)")]
    [SerializeField] private GameObject explosionPrefab;
    private int damage;
    private int ownerTeam;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Setup(Vector2 direction, float bulletSpeed, int dmg, int team)
    {
        damage = dmg;
        ownerTeam = team;

        Vector2 normalizedDir = direction.normalized;

        // Поворачиваем пулю по направлению полета
        float angle = Mathf.Atan2(normalizedDir.y, normalizedDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Задаем физическую скорость напрямую (оптимально для Unity 6)
        rb.linearVelocity = normalizedDir * bulletSpeed;

        // Время жизни пули, чтобы не засорять сцену
        Destroy(gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Проверяем, попали ли в Юнита
        if (other.TryGetComponent<Unit>(out Unit pawn))
        {
            if (pawn.team != ownerTeam)
            {
                pawn.TakeDamage(damage);
                ExecuteDestruction();
            }
        }
        // Если врезались в стену арены (не триггер)
        else if (!other.isTrigger) 
        {
            ExecuteDestruction();
        }
    }

// НОВЫЙ ВСПОМОГАТЕЛЬНЫЙ МЕТОД ДЛЯ СПАВНА ВЗРЫВА:
private void ExecuteDestruction()
{
    // Если префаб взрыва задан — спавним его в точке удара
    if (explosionPrefab != null)
    {
        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
    }
    
    Destroy(gameObject);
}
}
