using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private Vector3 startPosition;
    private float speed;
    private float damage;
    private float distance;
    public float travelLimit;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        transform.position += speed * Time.deltaTime * transform.forward;

        distance = Vector3.Distance(startPosition, transform.position);
        if (distance > travelLimit)
        {
            Destroy(this.gameObject);
        }
    }

    void OnTriggerEnter(Collider collision)
    { // Using trigger rather than collision
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
        {
            Destroy(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            ReactivePlayer player = collision.gameObject.GetComponent<ReactivePlayer>();

            if (player != null)
            {
                player.ReactToHit(damage);
            }

            Destroy(this.gameObject);
        }
    }

    public void SetProjectileSpeed(float value)
    {
        speed = value;
    }

    public void SetProjectileDamage(float value)
    {
        damage = value;
    }
}
