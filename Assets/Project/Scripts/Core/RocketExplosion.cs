using UnityEngine;

public class RocketExplosion : MonoBehaviour
{
    void Start()
    {
        // Дважды оптимизированное автоуничтожение: 
        // Находим самый долгий Particle System в объекте и удаляем взрыв строго по его завершению
        ParticleSystem[] systems = GetComponentsInChildren<ParticleSystem>();
        float maxDuration = 0f;

        for (int i = 0; i < systems.Length; i++)
        {
            var main = systems[i].main;
            float totalLifetime = main.duration + main.startLifetime.constantMax;
            if (totalLifetime > maxDuration)
            {
                maxDuration = totalLifetime;
            }
        }

        Destroy(gameObject, maxDuration);
    }
}
