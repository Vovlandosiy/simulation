using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header("Juice settings")]
    [SerializeField] private Transform spriteTransform;
    [SerializeField] private float squashAmount = 0.4f;
    [SerializeField] private float restoreSpeed = 10f;
    private Vector3 originalSpriteScale = Vector3.one;

    [Header("Weapon Settings")] 
    [SerializeField] private Transform weaponPivot;        
    [SerializeField] private SpriteRenderer weaponVisual;

    [Header("Audio")]
    [SerializeField] private AudioSource _hitWall;
    [SerializeField] private AudioSource _pickup;
    [SerializeField] private AudioSource _punchUnit;
    [SerializeField] private AudioSource _hitUnit;

    private bool hasWeapon = false;
    private float aimTimer = 0f;
    private float recoilTimer = 0f;
    private bool isRecoiling = false;

    public UnitData unitData;  
    public int team = 1;

    private int currentHealth;
    private float speed;
    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private Vector2 moveDirection;

    private WeaponData currentWeapon;
    private int currentAmmo;
    private const float WallPushOffset = 0.05f;
    private const float UnitSeparationOffset = 0.02f;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Start()
    {
        if (spriteTransform == null) spriteTransform = transform; 
        originalSpriteScale = spriteTransform.localScale;

        if (unitData != null)
        {
            currentHealth = unitData.health;
            speed = unitData.movementSpeed;
        }
        
        // Скрываем визуал оружия на старте
        if (weaponVisual != null) weaponVisual.gameObject.SetActive(false);
        
        LaunchInRandomDirection();
    }

    void Update()
    {
        if (hasWeapon && currentWeapon != null)
        {
            Unit target = FindClosestEnemy();
            
            if (target != null)
            {
                // Поворачиваем локальную ось оружия в сторону врага
                Vector3 targetDir = target.transform.position - weaponPivot.position;
                float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
                weaponPivot.rotation = Quaternion.Euler(0, 0, angle);

                // Флипаем спрайт, чтобы пистолет не становился «вверх ногами» при взгляде влево
                if (weaponVisual != null)
                {
                    weaponVisual.flipY = Mathf.Abs(angle) > 90f;
                }

                // Считаем время до выстрела
                aimTimer -= Time.deltaTime;
                if (aimTimer <= 0f)
                {
                    float distance = Vector2.Distance(transform.position, target.transform.position);

                    if (distance <= currentWeapon.fireRange)
                        ExecutePhysicalShot(targetDir.normalized);
                    else
                        aimTimer = 0.1f;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isRecoiling)
        {
            recoilTimer -= Time.fixedDeltaTime;
            if (recoilTimer <= 0f)
            {
                isRecoiling = false;
                //LaunchInRandomDirection(); // После отдачи летим в случайную сторону
            }
            return; 
        }

        rb.linearVelocity = moveDirection * speed;

        spriteTransform.localScale = Vector3.Lerp(spriteTransform.localScale, originalSpriteScale, restoreSpeed * Time.fixedDeltaTime);
    }


    private void LaunchInRandomDirection()
    {
        float randomAngle = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        moveDirection = new Vector2(Mathf.Cos(randomAngle), Mathf.Sin(randomAngle)).normalized;
    }

    public bool TryEquipWeapon(WeaponData newWeapon)
    {
        if (hasWeapon || newWeapon == null) return false;

        currentWeapon = newWeapon;
        currentAmmo = newWeapon.ammo;
        hasWeapon = true;

        if (weaponVisual != null)
        {
            weaponVisual.sprite = currentWeapon.weaponSprite;
            weaponVisual.gameObject.SetActive(true);
        }

        aimTimer = currentWeapon.aimDuration;

        _pickup.pitch = Random.Range(0.8f, 1.2f);
        if (_pickup != null) _pickup.PlayOneShot(_pickup.clip);
        
        Debug.Log($"{gameObject.name} подобрал {newWeapon.weaponName} и целится на ходу!");
        return true;
    }

    private void ExecutePhysicalShot(Vector2 shootDirection)
    {
        if (currentWeapon == null || currentWeapon.bulletPrefab == null) return;

        // Динамически вычисляем точку вылета снаряда
        float weaponLengthOffset = 0.5f; // Базовое смещение вперед от центра пивота
        Vector3 spawnPos = weaponPivot.position + (Vector3)shootDirection * weaponLengthOffset;

        GameObject bulletObj = Instantiate(currentWeapon.bulletPrefab, spawnPos, Quaternion.identity);

        if (CameraJuiceManager.Instance != null) CameraJuiceManager.Instance.ShakeOnShot();
        
        if (bulletObj.TryGetComponent<Bullet>(out Bullet bullet))
        {
            bullet.Setup(shootDirection, currentWeapon.bulletSpeed, currentWeapon.damage, this.team);
        }

        currentAmmo--;
        Debug.Log($"{gameObject.name} выстрелил из {currentWeapon.weaponName}! Патронов осталось: {currentAmmo}");

        // Отдача толкает назад (для РПГ можно в будущем сделать отдачу сильнее, умножив на коэффициент)
        //isRecoiling = true;
        //recoilTimer = 0.2f; 
        //rb.linearVelocity = -shootDirection * (speed * 2f);

        if (currentAmmo > 0)
        {
            aimTimer = currentWeapon.aimDuration;
        }
        else
        {
            RemoveWeapon();
        }
    }



    private Unit FindClosestEnemy()
    {
        Unit[] allUnits = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        Unit closest = null;
        float minDistance = Mathf.Infinity;

        foreach (Unit p in allUnits)
        {
            if (p == this || p.team == this.team) continue;

            float dist = Vector2.Distance(transform.position, p.transform.position); //ПРОВЕРИТЬ 
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
    if (!isRecoiling)
    {
        Vector2 normal = collision.contacts[0].normal;
        Vector2 reflectDirection = Vector2.Reflect(moveDirection, normal);

        float randomAngle = Random.Range(-10f, 10f);
        Quaternion rotation = Quaternion.Euler(0, 0, randomAngle);
        
        moveDirection = (rotation * reflectDirection).normalized;

        // ЭФФЕКТ СЖАТИЯ (Squash): Вычисляем, с какой стороны был удар, и сжимаем спрайт
        // normal.x указывает на удар слева/справа, normal.y — сверху/снизу
        float squashX = originalSpriteScale.x * (1f - Mathf.Abs(normal.x) * squashAmount);
        float squashY = originalSpriteScale.y * (1f - Mathf.Abs(normal.y) * squashAmount);
        
        // В момент удара мгновенно сплющиваем спрайт
        spriteTransform.localScale = new Vector3(squashX, squashY, originalSpriteScale.z);

        //Проигрываем звук
        if (_hitWall != null)
        {
            _hitWall.pitch = Random.Range(0.8f, 1.2f);
            _hitWall.PlayOneShot(_hitWall.clip);
        }
    }

   // if (collision.gameObject.TryGetComponent<Unit>(out Unit otherPawn))
   // {
    //    if (otherPawn.team != this.team)
     //   {
    //        otherPawn.TakeDamage(unitData.damage);
    //        Debug.Log($"{gameObject.name} ударил {otherPawn.gameObject.name} в ближнем бою!");
   //     }
   // }
}



    private void RemoveWeapon()
    {
        hasWeapon = false;
        currentWeapon = null;
        if (weaponVisual != null)
        {
            weaponVisual.gameObject.SetActive(false);
            weaponVisual.sprite = null;
        }
    }


    public void TakeDamage(int damageAmount)
    {
        if (_hitUnit != null)
        {
            _hitUnit.pitch = Random.Range(0.8f, 1.2f);
            _hitUnit.PlayOneShot(_hitUnit.clip);
        }

        currentHealth -= damageAmount;
        if (currentHealth <= 0) Die();
    }

    private void Die()
        {Destroy(gameObject);}
        
        
    public int GetCurrentHealth()
        {return currentHealth;}
}
