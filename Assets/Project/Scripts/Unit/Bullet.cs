using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    private int ownerTeam;
    private Vector2 direction;
    private float speed;

    public void Setup(Vector2 dir, float bulletSpeed, int dmg, int team)
    {
        direction = dir.normalized;
        speed = bulletSpeed;
        damage = dmg;
        ownerTeam = team;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        Destroy(gameObject, 3f);
    }

    void FixedUpdate()
    {
        transform.Translate(Vector2.right * speed * Time.fixedDeltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Pawn pawn = other.GetComponent<Pawn>();
        if (pawn != null)
        {
            if (pawn.team != ownerTeam)
            {
                pawn.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        else if (!other.isTrigger) 
        {
            Destroy(gameObject);
        }
    }
}

