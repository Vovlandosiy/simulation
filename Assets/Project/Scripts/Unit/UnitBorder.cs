using UnityEngine;

public class UnitBorder : MonoBehaviour
{
    [SerializeField] private float darknessFactor = 0.25f;
    [SerializeField] private SpriteRenderer parentRenderer;

    private SpriteRenderer borderRenderer;

    void Awake()
    {
        borderRenderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        ApplyDarkerColor();
    }

    private void ApplyDarkerColor()
    {
        if (parentRenderer == null || borderRenderer == null) return;

        // Получаем текущий цвет родителя
        Color parentColor = parentRenderer.color;

        // Переводим RGB в HSV (Hue, Saturation, Value/Brightness)
        Color.RGBToHSV(parentColor, out float h, out float s, out float v);

        // Затемняем яркость (V), уменьшая её на заданный коэффициент
        v = Mathf.Clamp01(v - darknessFactor);

        // Конвертируем обратно в RGB и сохраняем исходную прозрачность (альфа-канал)
        Color darkColor = Color.HSVToRGB(h, s, v);
        darkColor.a = parentColor.a; 

        // Применяем цвет к обводке
        borderRenderer.color = darkColor;
    }
}
