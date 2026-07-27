using UnityEngine;

public class Pawn : MonoBehaviour
{
    public UnitData unitData;  
    public int team = 1;

    private int currentHealth;
    private float speed;
    private Rigidbody2D rb;
    private Vector2 moveDirection;

    // Слот для текущего оружия
    private WeaponData currentWeapon;
    private int currentAmmo;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (unitData != null)
        {
            currentHealth = unitData.health;
            speed = unitData.movementSpeed;
        }
        LaunchInRandomDirection();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = moveDirection * speed;
    }

    private void LaunchInRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized;
    }

    // Метод подбора оружия. Вызывается из скрипта DroppedWeapon
    public bool TryEquipWeapon(WeaponData newWeapon)
    {
        // Если оружие уже есть, игнорируем подбор
        if (currentWeapon != null) return false;

        currentWeapon = newWeapon;
        currentAmmo = newWeapon.ammo;
        
        Debug.Log($"{gameObject.name} подобрал {newWeapon.weaponName}!");

        // Сразу пытаемся выстрелить из поднятого оружия
        ExecuteShot();
        return true;
    }

    private void ExecuteShot()
    {
        if (currentWeapon == null || currentAmmo <= 0) return;

        // Ищем ближайшего врага на арене
        Pawn target = FindClosestEnemy();

        if (target != null)
        {
            // Проверяем, достает ли пушка по дистанции
            float distance = Vector2.Distance(transform.position, target.transform.position);
            if (distance <= currentWeapon.fireRange)
            {
                // Наносим урон
                target.TakeDamage(currentWeapon.damage);
                currentAmmo--;

                // Рандомный сильный отскок в сторону ПОСЛЕ выстрела (эффект отдачи)
                LaunchInRandomDirection();

                Debug.Log($"{gameObject.name} выстрелил в {target.gameObject.name} из {currentWeapon.weaponName}!");
            }
        }

        // Если патроны кончились, выбрасываем/ломаем пушку
        if (currentAmmo <= 0)
        {
            currentWeapon = null;
        }
    }

    private Pawn FindClosestEnemy()
    {
        Pawn[] allPawns = FindObjectsByType<Pawn>(FindObjectsSortMode.None);
        Pawn closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Pawn p in allPawns)
        {
            if (p == this || p.team == this.team) continue;

            float dist = Vector2.Distance(transform.position, p.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                closest = p;
            }
        }
        return closest;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Vector2 normal = collision.contacts[0].normal;
        Vector2 reflectDirection = Vector2.Reflect(moveDirection, normal);
        float randomOffset = Random.Range(-15f, 15f);
        
        moveDirection = Quaternion.Euler(0, 0, randomOffset) * reflectDirection;
        moveDirection.Normalize();

        // Урон в ближнем бою при столкновении
        Pawn otherPawn = collision.gameObject.GetComponent<Pawn>();
        if (otherPawn != null && otherPawn.team != this.team)
        {
            otherPawn.TakeDamage(unitData.damage);
            Debug.Log($"{gameObject.name} ударил {otherPawn.gameObject.name} в ближнем бою!");
        }
    }

    public void TakeDamage(int damageAmount)
    {
        currentHealth -= damageAmount;
        if (currentHealth <= 0) Die();
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}
