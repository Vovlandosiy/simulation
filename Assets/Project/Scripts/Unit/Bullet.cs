using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] private GameObject hitParticlePrefab;

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
                if (CameraJuiceManager.Instance != null) CameraJuiceManager.Instance.ShakeOnHit();
                if (hitParticlePrefab != null)
                {
                    Vector3 spawnPos = transform.position;

                    Vector2 splashDirection = rb.linearVelocity.normalized;

                    // Спавним префаб ровно (без вращения), чтобы работали его внутренние настройки конуса
                    GameObject fx = Instantiate(hitParticlePrefab, spawnPos, Quaternion.identity);
                    
                    if (fx.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
                    {
                        // 1. Красим в цвет команды подбитого юнита
                        if (HealthUIManager.Instance != null)
                        {
                            Color targetTeamColor = HealthUIManager.Instance.GetTeamColorPublic(pawn.team);
                            var mainModule = ps.main;
                            mainModule.startColor = targetTeamColor;
                        }

                        // 2. Управляем направлением через модуль Velocity over Lifetime
                        var velocityModule = ps.velocityOverLifetime;
                        velocityModule.enabled = true;
                        velocityModule.space = ParticleSystemSimulationSpace.World; // Считаем по мировым осям, а не локальным


                        float splashForce = 10f; 

                        // Принудительно заставляем ВСЕ созданные частицы лететь по нашему вектору
                        velocityModule.x = splashDirection.x * splashForce;
                        velocityModule.y = splashDirection.y * splashForce;
                        velocityModule.z = 0f;

                        // Автоуничтожение по времени жизни
                        var main = ps.main;
                        float totalLifetime = main.duration + main.startLifetime.constantMax;
                        Destroy(fx, totalLifetime);
                    }
                    else
                    {
                        Destroy(fx, 1f);
                    }
                }
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
        if (CameraJuiceManager.Instance != null) CameraJuiceManager.Instance.ShakeOnExplosion();
    }
    
    Destroy(gameObject);
}
}
