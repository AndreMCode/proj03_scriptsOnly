using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] AudioSource soundSource;
    [SerializeField] AudioClip projectileLaunchSFX;
    public float projectileLaunchSFXVolume;
    public float projectileLaunchSFXPitch;
    [SerializeField] GameObject playerProjectile;
    private GameObject projectile;
    private Vector3 projectileDirection;
    public float projectileForwardOffset;
    public float projectileHeightOffset;

    private float fireTime;
    public float fireCooldown;
    private bool firing;

    void Start()
    {
        projectileDirection = Vector3.right;
    }

    void Update()
    {
        float horizontalDirection = Input.GetAxisRaw("Horizontal");
        if (horizontalDirection != 0)
        {
            projectileDirection = horizontalDirection > 0 ? Vector3.right : Vector3.left;
        }

        float perpendicularDirection = Input.GetAxisRaw("Vertical");
        if (perpendicularDirection > 0)
        { // Vertical overrides horizontal so that the player can strafe and fire projectiles
            projectileDirection = perpendicularDirection > 0 ? Vector3.forward : Vector3.back;

            animator.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

        if (firing && Time.time >= fireTime)
        {
            firing = false;
        }

        if (!firing)
        {
            if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.JoystickButton2))
            {
                FireProjectile();

                // soundSource.pitch = projectileLaunchSFXPitch;
                // soundSource.PlayOneShot(projectileLaunchSFX, projectileLaunchSFXVolume);

                // projectile = Instantiate(playerProjectile);

                // Vector3 spawnPosition = transform.position
                //                   + projectileDirection * projectileForwardOffset
                //                   + Vector3.up * projectileHeightOffset;

                // projectile.transform.SetPositionAndRotation(spawnPosition, Quaternion.LookRotation(projectileDirection));
                // PlayerProjectile projectileLook = projectile.GetComponent<PlayerProjectile>();
                // projectileLook.SetDirection(projectileDirection);
            }
        }
    }

    private void FireProjectile()
    {
        fireTime = Time.time + fireCooldown;
        firing = true;

        soundSource.pitch = projectileLaunchSFXPitch;
        soundSource.PlayOneShot(projectileLaunchSFX, projectileLaunchSFXVolume);

        projectile = Instantiate(playerProjectile);

        Vector3 spawnPosition = transform.position
                          + projectileDirection * projectileForwardOffset
                          + Vector3.up * projectileHeightOffset;

        projectile.transform.SetPositionAndRotation(spawnPosition, Quaternion.LookRotation(projectileDirection));
        PlayerProjectile projectileLook = projectile.GetComponent<PlayerProjectile>();
        projectileLook.SetDirection(projectileDirection);
    }
}
