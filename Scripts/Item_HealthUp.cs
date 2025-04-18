using UnityEngine;

public class Item_HealthUp : MonoBehaviour
{
    [SerializeField] GameObject soundSourceFinal;
    [SerializeField] AudioClip recoverySFX;
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

            if (player != null)
            {
                player.ReactToRecovery(recoveryValue);

                PlayFinalSFX();
            }

            Destroy(this.gameObject);
        }
    }

    private void PlayFinalSFX()
    {
        // Instantiate sound object
        GameObject soundObject = Instantiate(soundSourceFinal, transform.position, Quaternion.identity);
        // Pull sound object script
        AudioSource source = soundObject.GetComponent<AudioSource>();

        // Set clip and attributes, play sound
        source.clip = recoverySFX;
        source.Play();

        // Destroy the sound object
        Destroy(soundObject, recoverySFX.length);
    }
}
