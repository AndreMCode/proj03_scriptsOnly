using System.Collections;
using UnityEngine;

public class EnemyLife : MonoBehaviour
{
    [SerializeField] EnemySight enemySight;
    [SerializeField] EnemyMovement enemyMovement;
    [SerializeField] Rigidbody body;
    [SerializeField] GameObject itemDropPrefab;

    [SerializeField] AudioSource soundSource;
    [SerializeField] GameObject soundSourceFinal;
    [SerializeField] AudioClip enemyHitSFX;
    [SerializeField] AudioClip enemyFinalSFX;
    private Vector3 hitFrom;
    private GameObject itemDrop;

    public float enemyHitSFXPitch;
    public float enemyHitSFXVolume;
    public float enemyFinalSFXPitch;
    public float enemyFinalSFXVolume;
    public float baseHealth;
    public float health;
    public bool isAlive;

    void Start()
    {
        isAlive = true;
        health = baseHealth;
    }

    void Update()
    {
        if (isAlive && health <= 0)
        { // Defeat sequence
            PlayFinalSFX();

            isAlive = false;
            enemySight.SetEnemySight(isAlive);
            enemyMovement.SetEnemyAlive(isAlive);

            int chance = Random.Range(0, 10);
            if (chance < 3)
            {
                itemDrop = Instantiate(itemDropPrefab);
                itemDrop.transform.localPosition = transform.position + Vector3.up * 1;
            }

            // Remove rigidbody constraints
            body.constraints = RigidbodyConstraints.None;

            // Apply knockback from projectile position
            body.AddExplosionForce(400.0f, hitFrom, 2.0f);

            Messenger<int>.Broadcast(GameEvent.PLAYER_SCORE_UPDATE, ((int)baseHealth));

            StartCoroutine(DestroyTimer());
        }
    }

    public void ReactToHit(float damage, Vector3 position)
    {
        // Set sound attributes, play sound
        soundSource.pitch = Random.Range(enemyHitSFXPitch - 0.1f, enemyHitSFXPitch + 0.1f);
        soundSource.PlayOneShot(enemyHitSFX, enemyHitSFXVolume);

        // Decrement health
        health -= damage;

        // Retrieve damage source position
        hitFrom = position;
    }

    public float GetEnemyHealth()
    {
        return health;
    }

    private void PlayFinalSFX()
    {
        // Instantiate sound object
        GameObject soundObject = Instantiate(soundSourceFinal, transform.position, Quaternion.identity);
        // Pull sound object script
        AudioSource source = soundObject.GetComponent<AudioSource>();

        // Set clip and attributes, play sound
        source.clip = enemyFinalSFX;
        source.pitch = Random.Range(enemyFinalSFXPitch - 0.1f, enemyFinalSFXPitch + 0.1f);
        source.volume = enemyFinalSFXVolume;
        source.Play();

        // Destroy the sound object
        Destroy(soundObject, enemyFinalSFX.length);
    }

    private IEnumerator DestroyTimer()
    {
        // Wait time and destroy self
        yield return new WaitForSeconds(1.5f);

        Debug.Log("EnemyDestroyed");
        Destroy(this.gameObject);
    }
}
