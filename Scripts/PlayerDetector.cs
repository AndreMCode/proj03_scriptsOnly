using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] EnemyMovement movement;
    [SerializeField] EnemyShooter shooter;
    [SerializeField] EnemySight sight;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            movement.enabled = true;
            shooter.enabled = true;
            sight.enabled = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            movement.enabled = false;
            shooter.enabled = false;
            sight.enabled = false;
        }
    }

    void Start()
    {
        // Optional: Handle case where player is already inside trigger at scene start
        Collider[] hits = Physics.OverlapBox(transform.position, GetComponent<BoxCollider>().bounds.extents);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                movement.enabled = true;
                shooter.enabled = true;
                sight.enabled = true;
                return;
            }
        }

        // Default to inactive if player isn't near
        movement.enabled = false;
        shooter.enabled = false;
        sight.enabled = false;
    }
}
