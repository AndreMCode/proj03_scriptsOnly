using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Bodies, components, effects
    [SerializeField] CharacterController body;
    [SerializeField] PlayerSight sight;
    [SerializeField] ParticleSystem dashSparks;
    [SerializeField] ParticleSystem dashDust;
    [SerializeField] Animator animator;
    public float dashDustParticleHeightOffset;

    // Audio
    [SerializeField] AudioSource dashSFXAudioSource;
    [SerializeField] AudioClip dashSFXClip;
    public float dashSFXVolume;
    public float dashSFXPitch;

    // Environment
    private Vector3 velocity;
    public float gravity;

    // Movement, jump
    public bool moveable; // deprecated **
    public bool grounded;
    public float moveSpeed;
    private float movement;
    private float lastMovement;
    public float jumpStrength;
    public float jumpCancelForce;
    public bool canCancelJump;

    // Dash
    public bool dashing;
    public bool dashJumping;
    public float dashSpeed;
    public float dashTime;
    public float lastDashTime;
    public float dashDuration;
    public float dashCooldown;
    private float dashDirection;

    // Wall cling, wall-jump
    public bool isWallAttached;
    public bool isWallJumping;
    public float wallSlideSpeed;
    public float wallJumpTime;
    public float wallJumpCooldown;
    private Vector3 wallJumpDirection;

    void Start()
    {
        moveable = true;
        grounded = true;
        dashing = false;
        dashJumping = false;
        canCancelJump = false;
        isWallAttached = false;
        isWallJumping = false;
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

        if (grounded && isWallJumping)
        {
            isWallJumping = false;
        }

        // Determine wall action
        if (isWallAttached && (grounded || movement == 0 || movement == -lastMovement))
        {
            isWallAttached = false;
            ReturnIsTouchingWall(isWallAttached);
        }

        // Collect movement
        movement = Input.GetAxisRaw("Horizontal");
        if (Input.GetKey(KeyCode.RightArrow))
        {
            movement = 1;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            movement = -1;
        }
        Vector3 moveDirection = new Vector3(movement, 0, 0).normalized;

        if (movement != 0 && lastMovement == -movement)
        {
            // Send direction out for raycast handler
            UpdateSight(moveDirection);
        }
        if (movement != 0)
        {
            lastMovement = movement;
        }

        // Collect input
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
            isWallJumping = false;
        }

        if (!grounded && canCancelJump && (Input.GetKeyUp(KeyCode.X) || Input.GetKeyUp(KeyCode.JoystickButton0)))
        { // Jump cancel
            velocity.y *= 1f / jumpCancelForce;
            canCancelJump = false;
            // isWallJumping = false;
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

        if (isWallAttached && (Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.JoystickButton0)))
        { // Walljump initialize
            isWallJumping = true;
            wallJumpDirection = movement > 0 ? Vector3.left : Vector3.right;
            wallJumpTime = Time.time + wallJumpCooldown;

            velocity.y = Mathf.Sqrt(2 * (jumpStrength * 0.67f) * gravity);

            isWallAttached = false;
            ReturnIsTouchingWall(isWallAttached);
        }

        // Apply movement
        if (dashJumping)
        { // Move at dash speed
            body.Move(dashSpeed * Time.deltaTime * moveDirection);
        }
        else if (isWallJumping && Time.time <= wallJumpTime)
        { // Lerp from moveSpeed to zero
            float wallJumpProgress = 1f - ((wallJumpTime - Time.time) / wallJumpCooldown);
            float currentSpeed = Mathf.Lerp(moveSpeed, 2f, wallJumpProgress);
            body.Move(currentSpeed * Time.deltaTime * wallJumpDirection);
        }
        else if (isWallJumping)
        { // Move at boosted speed
            body.Move((moveSpeed + 5f) * Time.deltaTime * moveDirection);
        }
        else if (!dashing)
        { // Apply normal movement speed
            body.Move(moveSpeed * Time.deltaTime * moveDirection);
        }

        // Determine gravitational influence
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
        else if (isWallAttached)
        {
            velocity.y = -wallSlideSpeed;
        }
        else
        {
            // Apply gravity if not dashing or attached to wall
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

    private void UpdateSight(Vector3 directionUpdate)
    {
        sight.UpdateDirection(directionUpdate);
    }

    private void ReturnIsTouchingWall(bool update)
    {
        // Debug.Log("Detached from wall.");
        sight.SetIsTouchingWall(update);
    }

    public void FreezeAnimatorBody()
    {
        animator.SetBool("jumping", false);
        animator.SetFloat("speed", 0);
        dashSparks.Stop();
    }

    public void SetIsTouchingWall(bool update)
    {
        // Debug.Log("recv'd wall detect.");
        if (update && movement != 0 && !body.isGrounded && velocity.y < 0)
        {
            if (dashJumping)
            {
                dashJumping = false;
            }
            // Debug.Log("Attached to wall");
            isWallAttached = update;
        }
        else
        { // Continue checking
            isWallAttached = false;
            sight.SetIsTouchingWall(isWallAttached);
        }
    }

    void LateUpdate()
    { // Lock player to z-axis
        Vector3 position = transform.position;
        position.z = 0;
        transform.position = position;
    }
}
