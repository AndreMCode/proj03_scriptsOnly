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
    [SerializeField] AudioClip breakableContentsSFX;

    private GameObject gem;

    public float breakableHitSFXPitch;
    public float breakableHitSFXVolume;
    public float breakableFinalSFXPitch;
    public float breakableFinalSFXVolume;
    public float breakableContentsSFXVolume;

    public float health;
    public int value;

    public void ReactToHit(float damage)
    {
        // Set sound attributes, play sound
        soundSource.pitch = Random.Range(breakableHitSFXPitch - 0.1f, breakableHitSFXPitch + 0.1f);
        soundSource.PlayOneShot(breakableHitSFX, breakableHitSFXVolume);

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
        GameObject soundObject1 = Instantiate(soundSourceFinal, transform.position, Quaternion.identity);
        // Pull sound object script
        AudioSource source1 = soundObject1.GetComponent<AudioSource>();

        // Set clip and attributes, play sound
        source1.clip = breakableContentsSFX;
        source1.volume = breakableContentsSFXVolume;
        source1.Play();

        // Destroy the sound object
        Destroy(soundObject1, breakableContentsSFX.length);

        // Instantiate sound object
        GameObject soundObject2 = Instantiate(soundSourceFinal, transform.position, Quaternion.identity);
        // Pull sound object script
        AudioSource source2 = soundObject2.GetComponent<AudioSource>();

        // Set clip and attributes, play sound
        source2.clip = breakableFinalSFX;
        source2.volume = breakableFinalSFXVolume;
        source2.pitch = Random.Range(breakableFinalSFXPitch - 0.1f, breakableFinalSFXPitch + 0.1f);
        source2.Play();

        // Destroy the sound object
        Destroy(soundObject2, breakableFinalSFX.length);
    }
}