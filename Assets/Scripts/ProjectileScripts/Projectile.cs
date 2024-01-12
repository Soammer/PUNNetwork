using Photon.Realtime;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 20f;
    public int damage = 1;
    public Rigidbody2D RB;
    public PlayerController Owner;

    void Start()
    {
        Destroy(gameObject, 8);
    }

    public void InitializeBullet(PlayerController owner, Vector2 target)
    {
        RB = GetComponent<Rigidbody2D>();
        Owner = owner;
       
        Vector2 dir = target - (Vector2)transform.position;
        RB.velocity = dir.normalized * speed;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerController other = collision.GetComponent<PlayerController>();
            if (other == Owner) return;
            Debug.Log("触碰玩家");
            other.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
