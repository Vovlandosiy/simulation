using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class HealthUIManager : MonoBehaviour
{
    [Header("Префабы элементов UI")]
    [SerializeField] private GameObject rowPrefab;         
    [SerializeField] private GameObject heartPrefab;      

    [Header("Визуал сердечек")]
    [SerializeField] private Sprite fullHeartSprite;      
    [SerializeField] private Sprite emptyHeartSprite;     

    [Header("Цвета команд по номерам (0 = Серый, 1 = Красный, 2 = Синий, 3 = Зеленый)")]
    [SerializeField] private Color[] teamColors = new Color[] {
        Color.gray, Color.red, Color.blue, Color.green, Color.yellow
    };

    [Header("Позиция интерфейса на экране")]
    [Range(-1500f, 1500f)] [SerializeField] private float positionOffsetX = 0f; 
    [Range(-3000f, 1500f)] [SerializeField] private float positionOffsetY = -50f; 

    [Header("Глобальные настройки размеров (В пикселях)")]
    [Range(10, 600)] [SerializeField] private int teamIconSize = 50;   
    [Range(10, 600)] [SerializeField] private int heartSize = 40;      
    [Range(0, 300)]  [SerializeField] private int spaceBetweenHearts = 8; 
    [Range(0, 300)]  [SerializeField] private int spaceAfterIcon = 25;    
    [Range(0, 300)]  [SerializeField] private int spaceBetweenRows = 15;   

    [Header("Тонкая настройка сердечек")]
    [Range(-200f, 200f)] [SerializeField] private float heartsVerticalOffset = 0f; // Смещение сердечек вверх/вниз

    [Header("Настройки цвета")]
    [SerializeField] private bool colorHeartsByTeam = true; 
    [SerializeField] private Color emptyHeartColor = new Color(0.2f, 0.2f, 0.2f, 0.4f); 

    private Dictionary<Unit, RowUIElements> activeRows = new Dictionary<Unit, RowUIElements>();
    private List<GameObject> rowPool = new List<GameObject>();
    
    private RectTransform managerRectTransform;
    private VerticalLayoutGroup managerLayoutGroup;

    private class RowUIElements
    {
        public GameObject rowObject;
        public Image teamIcon;
        public RectTransform teamIconRect;
        public RectTransform heartsContainerRect; // Добавили RectTransform контейнера для смещения
        public HorizontalLayoutGroup rowLayout;
        public HorizontalLayoutGroup heartsLayout;
        public Transform heartsContainer;
        public List<Image> heartImages = new List<Image>();
    }

    void Awake()
    {
        managerRectTransform = GetComponent<RectTransform>();
        managerLayoutGroup = GetComponent<VerticalLayoutGroup>();
    }

    void Update()
    {
        if (managerRectTransform != null)
        {
            managerRectTransform.anchoredPosition = new Vector2(positionOffsetX, positionOffsetY);
        }

        if (managerLayoutGroup != null)
        {
            managerLayoutGroup.spacing = spaceBetweenRows;
        }

        Unit[] allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);

        for (int i = 0; i < allUnits.Length; i++)
        {
            Unit unit = allUnits[i];
            if (unit == null) continue;

            if (!activeRows.ContainsKey(unit))
            {
                CreateRowForUnit(unit);
            }
        }

        List<Unit> unitsToRemove = new List<Unit>();

        foreach (var pair in activeRows)
        {
            Unit unit = pair.Key;
            if (unit == null)
            {
                unitsToRemove.Add(unit);
                continue;
            }

            ApplyDynamicSettings(unit);
            UpdateHearts(unit);
        }

        for (int i = 0; i < unitsToRemove.Count; i++)
        {
            ReturnRowToPool(unitsToRemove[i]);
        }
    }

    private Color GetColorForTeam(int teamNumber)
    {
        if (teamNumber >= 0 && teamNumber < teamColors.Length) return teamColors[teamNumber];
        return Color.white; 
    }

    private void CreateRowForUnit(Unit unit)
    {
        GameObject rowObj = (rowPool.Count > 0) ? rowPool[rowPool.Count - 1] : Instantiate(rowPrefab, transform);
        if (rowPool.Count > 0) rowPool.RemoveAt(rowPool.Count - 1);
        rowObj.SetActive(true);

        RowUIElements elements = new RowUIElements();
        elements.rowObject = rowObj;
        
        Transform iconTransform = rowObj.transform.Find("TeamIcon");
        elements.heartsContainer = rowObj.transform.Find("HealthContainer");
        
        elements.teamIcon = iconTransform.GetComponent<Image>();
        elements.teamIconRect = iconTransform.GetComponent<RectTransform>();
        elements.heartsContainerRect = elements.heartsContainer.GetComponent<RectTransform>();
        elements.rowLayout = rowObj.GetComponent<HorizontalLayoutGroup>();
        elements.heartsLayout = elements.heartsContainer.GetComponent<HorizontalLayoutGroup>();
        
        if (elements.rowLayout != null)
        {
            elements.rowLayout.childControlWidth = false;
            elements.rowLayout.childControlHeight = false;
            elements.rowLayout.childForceExpandWidth = false;
            elements.rowLayout.childForceExpandHeight = false;
        }

        if (elements.heartsLayout != null)
        {
            elements.heartsLayout.childControlWidth = false;
            elements.heartsLayout.childControlHeight = false;
            elements.heartsLayout.childForceExpandWidth = false;
            elements.heartsLayout.childForceExpandHeight = false;
        }

        iconTransform.SetSiblingIndex(0);
        elements.heartsContainer.SetSiblingIndex(1);

        int maxHealth = (unit.unitData != null) ? unit.unitData.maxHealth : 3;
        foreach (Transform child in elements.heartsContainer) Destroy(child.gameObject);

        for (int i = 0; i < maxHealth; i++)
        {
            GameObject heartObj = Instantiate(heartPrefab, elements.heartsContainer);
            if (heartObj.TryGetComponent<Image>(out Image img))
            {
                img.sprite = fullHeartSprite;
                elements.heartImages.Add(img);
            }
        }

        activeRows.Add(unit, elements);
    }

    private void ApplyDynamicSettings(Unit unit)
    {
        if (!activeRows.TryGetValue(unit, out RowUIElements elements)) return;

        elements.teamIcon.color = GetColorForTeam(unit.team);

        if (elements.teamIconRect != null)
        {
            elements.teamIconRect.sizeDelta = new Vector2(teamIconSize, teamIconSize);
        }
        
        // Применяем вертикальное смещение только для контейнера сердечек
        if (elements.heartsContainerRect != null)
        {
            Vector2 currentPos = elements.heartsContainerRect.anchoredPosition;
            elements.heartsContainerRect.anchoredPosition = new Vector2(currentPos.x, heartsVerticalOffset);
        }
        
        if (elements.rowLayout != null) elements.rowLayout.spacing = spaceAfterIcon;
        if (elements.heartsLayout != null) elements.heartsLayout.spacing = spaceBetweenHearts;

        for (int i = 0; i < elements.heartImages.Count; i++)
        {
            if (elements.heartImages[i] != null)
            {
                elements.heartImages[i].rectTransform.sizeDelta = new Vector2(heartSize, heartSize);
            }
        }
    }

    private void UpdateHearts(Unit unit)
    {
        if (!activeRows.TryGetValue(unit, out RowUIElements elements)) return;

        int currentHp = unit.GetCurrentHealth();
        Color teamColor = GetColorForTeam(unit.team);

        for (int i = 0; i < elements.heartImages.Count; i++)
        {
            if (i < currentHp)
            {
                elements.heartImages[i].sprite = fullHeartSprite;
                elements.heartImages[i].color = colorHeartsByTeam ? teamColor : Color.white;
            }
            else
            {
                elements.heartImages[i].sprite = emptyHeartSprite;
                elements.heartImages[i].color = emptyHeartColor;
            }
        }
    }

    private void ReturnRowToPool(Unit unit)
    {
        if (activeRows.TryGetValue(unit, out RowUIElements elements))
        {
            elements.rowObject.SetActive(false);
            rowPool.Add(elements.rowObject);
            activeRows.Remove(unit);
        }
    }
}
