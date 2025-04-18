using System.Collections;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class ReactivePlayer : MonoBehaviour
{
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip playerHitSFX;
    public float playerHitSFXVolume;
    public float playerHitSFXPitch;
    [SerializeField] SkinnedMeshRenderer playerRenderer;
    public float baseHealth;
    public float health;
    public float hitCooldown;
    public float flickerInterval;
    public bool invincible;
    public bool isAlive;
    
    void Start()
    {
        health = baseHealth;
        isAlive = true;
        invincible = false;

        Messenger<float>.Broadcast("PLAYER_HEALTH_UPDATE", health);
    }

    void Update()
    {
        if (isAlive && health <= 0)
        {
            Debug.Log("Player Died!");
            isAlive = false;
        }
    }

    public void ReactToHit(float damage)
    {
        if (!invincible)
        {
            soundSource.pitch = Random.Range(playerHitSFXPitch - 0.1f, playerHitSFXPitch + 0.1f);
            soundSource.PlayOneShot(playerHitSFX, playerHitSFXVolume);

            health -= damage;
            Messenger<float>.Broadcast("PLAYER_HEALTH_UPDATE", health);

            Debug.Log("Player Received " + damage + " damage");
            StartCoroutine(PlayerFlicker());
        }
    }

    public void ReactToRecovery(float value)
    {
        health += value;
        health = Mathf.Clamp(health, 0, baseHealth);

        Messenger<float>.Broadcast("PLAYER_HEALTH_UPDATE", health);
    }

    private IEnumerator PlayerFlicker()
    {
        invincible = true;
        float elapsed = 0f;

        while (elapsed < hitCooldown)
        {
            playerRenderer.enabled = false;
            yield return new WaitForSeconds(flickerInterval);
            playerRenderer.enabled = true;
            yield return new WaitForSeconds(flickerInterval);
            elapsed += flickerInterval * 2;
        }

        invincible = false;
    }
}
