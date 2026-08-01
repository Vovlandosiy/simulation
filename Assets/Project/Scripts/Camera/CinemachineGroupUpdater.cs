using UnityEngine;
using Unity.Cinemachine; // Новый неймспейс для Unity 6

public class CinemachineGroupUpdater : MonoBehaviour
{
    private CinemachineTargetGroup targetGroup;

    void Awake()
    {
        targetGroup = GetComponent<CinemachineTargetGroup>();
    }

    void Update()
    {
        if (targetGroup == null) return;

        // Ищем вообще всех активных юнитов Unit.cs на сцене
        Unit[] allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);

        // Полностью очищаем старый список целей
        targetGroup.Targets.Clear();

        // Заполняем актуальными живыми юнитами
        for (int i = 0; i < allUnits.Length; i++)
        {
            if (allUnits[i] != null)
            {
                var newTarget = new CinemachineTargetGroup.Target
                {
                    Object = allUnits[i].transform,
                    Weight = 1f, // Степень влияния юнита на центр камеры
                    Radius = 1f  // Радиус вокруг юнита, который гарантированно должен влезть в экран
                };
                
                targetGroup.Targets.Add(newTarget);
            }
        }
    }
}
