using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Simulation/Unit Data")]
public class UnitData : ScriptableObject
{
    public string unitName = "Pawn";
    public int maxHealth = 3;
    public int health = 3;
    public float movementSpeed = 5f;
    public int damage = 1;
    public float attackRange = 1f;
    public float attackCooldown = 1f;
    public Sprite unitSprite;
}
