using UnityEngine;
using Unity.Cinemachine;

[RequireComponent(typeof(CinemachineImpulseSource))]
public class CameraJuiceManager : MonoBehaviour
{
    public static CameraJuiceManager Instance { get; private set; }

    [Header("Настройки интенсивности тряски")]
    [Range(0f, 50f)] [SerializeField] private float shotShakeIntensity = 1f;
    [Range(0f, 50f)] [SerializeField] private float hitShakeIntensity = 3f;
    [Range(0f, 50f)] [SerializeField] private float explosionShakeIntensity = 20f;

    private CinemachineImpulseSource impulseSource;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void ShakeOnShot() => GenerateImpulse(shotShakeIntensity * 0.01f);

    public void ShakeOnHit() => GenerateImpulse(hitShakeIntensity * 0.01f);

    public void ShakeOnExplosion() => GenerateImpulse(explosionShakeIntensity * 0.01f);

    private void GenerateImpulse(float intensity)
    {
        if (impulseSource == null) return;

        // Генерируем случайное направление толчка в 2D (по осям X и Y)
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        
        // Превращаем в Vector3, оставляя ось Z нулевой (чтобы камеру не уносило вглубь сцены)
        Vector3 impulseVelocity = new Vector3(randomDirection.x, randomDirection.y, 0f) * intensity;

        // В Unity 6 этот метод сразу применяет переданную скорость к текущемуBump
        impulseSource.GenerateImpulseWithVelocity(impulseVelocity);
    }
}
