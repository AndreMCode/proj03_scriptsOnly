using UnityEngine;

public class Item_HealthUp : MonoBehaviour
{
    public float rotationSpeed;
    public float recoveryValue;

    void Start()
    {
        
    }

    void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ReactivePlayer player = other.gameObject.GetComponent<ReactivePlayer>();

            player.ReactToRecovery(recoveryValue);

            Destroy(this.gameObject);
        }
    }
}
