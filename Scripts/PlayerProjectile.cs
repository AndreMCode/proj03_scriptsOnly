using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private Vector3 startPosition;
    public Vector3 direction;
    public float baseSpeed;
    public float speed;
    public float distance;
    public float travelLimit;

    public float damage;

    void Start()
    {
        speed = baseSpeed;
        startPosition = transform.position;
    }

    void Update()
    {
        transform.Translate(0, 0, speed * Time.deltaTime);

        distance = Vector3.Distance(startPosition, transform.position);
        if (distance > travelLimit)
        {
            Destroy(this.gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Floor"))
        {
            Destroy(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Breakable"))
        {
            Breakable breakable = collision.gameObject.GetComponent<Breakable>();

            if (breakable != null)
            {
                breakable.ReactToHit(damage);
            }

            Destroy(this.gameObject);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyLife enemy = collision.gameObject.GetComponent<EnemyLife>();

            if (enemy != null)
            {
                Vector3 offsetDir = Vector3.zero;

                // Add variance to position for enemy reaction dependent on travel direction
                if (direction.x != 0)
                {
                    // offsetDir = Random.Range(0, 2) == 0 ? Vector3.forward : Vector3.back;
                    offsetDir = Vector3.forward;
                    offsetDir *= Random.Range(0.4f, 0.6f);
                }
                if (direction.z != 0)
                {
                    // offsetDir = Random.Range(0, 2) == 0 ? Vector3.right : Vector3.left;
                    offsetDir = Vector3.back;
                    offsetDir *= Random.Range(0, 0.2f);
                }

                // Apply position offset and damage
                Vector3 hitPosition = transform.position + offsetDir;
                enemy.ReactToHit(damage, hitPosition);

                Debug.Log("Hit position (with offset): " + hitPosition);
            }

            Destroy(this.gameObject);
        }
    }

    public void SetDirection(Vector3 projectileDirection)
    {
        direction = projectileDirection;
    }
}
