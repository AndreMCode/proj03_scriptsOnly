using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] CharacterController body;
    [SerializeField] ParticleSystem dashSparks;
    [SerializeField] ParticleSystem dashDust;
    [SerializeField] Animator animator;
    public float dashDustParticleHeightOffset;

    // Audio
    [SerializeField] AudioSource dashSFXAudioSource;
    [SerializeField] AudioClip dashSFXClip;
    public float dashSFXVolume;
    public float dashSFXPitch;

    private Vector3 velocity;
    public float gravity;

    public bool moveable; // for attatching as child obj for wall slide action (transform.translate)
    public bool grounded;
    public float moveSpeed;
    public float jumpStrength;
    public float jumpCancelForce;
    public bool canCancelJump;

    public bool dashing;
    public bool dashJumping;
    public float dashSpeed;
    public float dashTime;
    public float lastDashTime;
    public float dashDuration;
    public float dashCooldown;
    private float dashDirection;
    

    void Start()
    {
        moveable = true;
        grounded = true;
        dashing = false;
        dashJumping = false;
        canCancelJump = false;
        dashSparks.Stop();
        dashDust.Stop();
    }

    void Update()
    {
        // Determine grounded condition
        grounded = body.isGrounded;
        if (grounded && dashJumping)
        {
            dashJumping = false;
            dashDust.Stop();
        }

        // Collect movement
        float movement = Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement = -1;
        }
        Vector3 moveDirection = new Vector3(movement, 0, 0).normalized;

        if (dashJumping)
        { // Apply dash airborne speed during dash jump time
            body.Move(dashSpeed * Time.deltaTime * moveDirection);
        }
        else if (!dashing)
        { // Apply normal movement speed
            body.Move(moveSpeed * Time.deltaTime * moveDirection);
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        { // Game Popup key
            Messenger.Broadcast(GameEvent.TOGGLE_GAME_POPUP);
        }

        if (grounded && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton0)))
        { // Jump
            velocity.y = Mathf.Sqrt(2 * jumpStrength * gravity);
            canCancelJump = true;
        }
        if (canCancelJump && velocity.y < 0)
        { // Falling check
            canCancelJump = false;
        }

        if (!grounded && canCancelJump && (Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.JoystickButton0)))
        { // Jump cancel
            velocity.y *= 1f / jumpCancelForce;
            canCancelJump = false;
        }

        if (grounded && !dashing && !dashJumping)
        { // Dash initialize
            if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.JoystickButton5))
                && Time.time > lastDashTime + dashCooldown && movement != 0)
            {
                dashing = true;
                dashTime = Time.time + dashDuration;
                dashDirection = movement > 0 ? 1 : -1;

                dashSFXAudioSource.pitch = dashSFXPitch;
                dashSFXAudioSource.PlayOneShot(dashSFXClip, dashSFXVolume);
                dashSparks.Play();
                dashDust.Play();

                lastDashTime = Time.time;
            }
        }

        // Dash movement
        if (dashing)
        {
            // Allow changing directions while dashing
            dashDirection = movement;

            body.Move(new Vector3(dashDirection * dashSpeed * Time.deltaTime, 0, 0));

            // Cancel if release C or timeout
            if (Time.time >= dashTime || Input.GetKeyUp(KeyCode.A)
                || Input.GetKeyUp(KeyCode.JoystickButton5))
            {
                dashing = false;
                dashSparks.Stop();
                dashDust.Stop();
            }

            if (grounded && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton0)))
            {
                dashing = false;
                dashJumping = true;
                dashSparks.Stop();
                velocity.y = Mathf.Sqrt(2 * jumpStrength * gravity);
                canCancelJump = true;
            }
        }
        else
        {
            // Apply gravity
            if (moveable)
            {
                if (!grounded)
                {
                    velocity.y -= gravity * Time.deltaTime;
                }
                else if (velocity.y < 0)
                {
                    velocity.y = -2f;
                }
            }
        }

        // Apply vertical movement
        body.Move(velocity * Time.deltaTime);

        // Flip model based on movement direction
        if (movement > 0)
            animator.transform.localRotation = Quaternion.Euler(0, 90, 0); // Facing right
        else if (movement < 0)
            animator.transform.localRotation = Quaternion.Euler(0, -90, 0); // Facing left

        // Animator parameters
        animator.SetFloat("speed", Mathf.Abs(movement)); // Use abs so it works in both directions
        animator.SetBool("jumping", !grounded && velocity.y > 0);

    }

    public void FreezeAnimatorBody()
    {
        animator.SetBool("jumping", false);
        animator.SetFloat("speed", 0);
        dashSparks.Stop();
    }

    void LateUpdate()
    { // Lock player to z-axis
        Vector3 position = transform.position;
        position.z = 0;
        transform.position = position;
    }
}
