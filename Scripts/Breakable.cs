using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Breakable : MonoBehaviour
{
    [SerializeField] GameObject gemPrefab;

    [SerializeField] AudioSource soundSource;
    [SerializeField] GameObject soundSourceFinal;
    [SerializeField] AudioClip breakableHitSFX;
    [SerializeField] AudioClip breakableFinalSFX;

    private GameObject gem;

    public float breakableHitSFXPitch;
    public float breakableFinalSFXPitch;

    public float health;
    public int value;

    public void ReactToHit(float damage)
    {
        // Set sound attributes, play sound
        soundSource.pitch = Random.Range(breakableHitSFXPitch - 0.1f, breakableHitSFXPitch + 0.1f);
        soundSource.PlayOneShot(breakableHitSFX, 0.5f);

        // Decrement health
        health -= damage;

        if (health <= 0)
        {
            Messenger<int>.Broadcast(GameEvent.PLAYER_SCORE_UPDATE, value);
            PlayFinalSFX();
            
            int gems = value / 10;

            for (int i = 0; i < gems; i++)
            {
                gem = Instantiate(gemPrefab);
                gem.transform.localPosition = transform.position;
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
        source.clip = breakableFinalSFX;
        source.pitch = Random.Range(breakableFinalSFXPitch - 0.1f, breakableFinalSFXPitch + 0.1f);
        source.Play();

        // Destroy the sound object
        Destroy(soundObject, breakableFinalSFX.length);
    }
}