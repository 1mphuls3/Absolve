using NUnit.Framework.Constraints;
using System.Collections;
using System.Net.NetworkInformation;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;
    [SerializeField] public Rigidbody2D rigidBody;
    [SerializeField] public ParticleSystem dashParticles;
    [SerializeField] public ParticleSystem speedParticles;
    [SerializeField] private ParticleSystem doubleJumpParticles;

    [SerializeField] public Player player;

    // Private movement variables
    [SerializeField] private AnimationCurve dashCurve;
    [SerializeField] private float coyoteGracePeriod = 0.1f;
    [SerializeField] private float jumpBounds = 0.65f;
    [SerializeField] private float wallClingBounds = 0.7f;
    [SerializeField] private float wallFriction = 0.1f;
    [SerializeField] private float gravityNormal = 2f;
    [SerializeField] private float gravityFalling = 2.6f;
    [SerializeField] private float gravityDown = 3f;

    [SerializeField] private AudioClip walkSound;
    [SerializeField] private AudioClip jumpSound;
    [SerializeField] private AudioClip clingSound;
    [SerializeField] private AudioClip dashSound;
    private AudioManager audioManager;

    private ParticleSystem.MainModule particleMain;
    private bool isDebugOn;
    private Collider2D playerCollider;

    private float coyoteTimer;
    private int jumpCount;

    // Public movement variables
    public Vector2 facingDirX;
    public Vector2 facingDirY;

    public int maxJumpCount = 1;
    public float jumpHeight = 4f;
    public float walkSpeed = 8.5f;

    public bool isRunning;
    public bool isGrounded;
    public bool isFalling;
    public bool isJumping;

    public bool isWallClung;
    public bool canWallCling = true;
    public float wallJumpCooldown = 0.08f;

    public float dashDistance = 2.5f;
    public float dashDuration = 0.15f;
    public float dashCooldown = 1f;
    public int dashOvercastCost = 5;
    public int maxDashCount = 1;
    public bool canDashUp;
    public int dashCount;
    public bool canDash = true;

    public bool canPlayWalk = true;
    public bool canPlayCling = true;
    private void Start()
    {
        audioManager = player.audioManager;

        particleMain = dashParticles.main;
        particleMain.startLifetime = dashCooldown;

        isDebugOn = false;
        playerCollider = GetComponent<Collider2D>();
        dashParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void Update()
    {
        particleMain.startLifetime = dashCooldown;
        if (Input.GetKeyDown(KeyCode.P))
        {
            isDebugOn = !isDebugOn;
        }
        if (isDebugOn)
        {
            rigidBody.bodyType = RigidbodyType2D.Kinematic;
            playerCollider.enabled = false;
        }
        else
        {
            rigidBody.bodyType = RigidbodyType2D.Dynamic;
            playerCollider.enabled = true;
        }

        // Get horizontal directional movement from input device
        float inputX = Input.GetAxisRaw("Horizontal");
        float inputY = Input.GetAxisRaw("Vertical");
        isGrounded = GetIsGrounded();
        isWallClung = isGrounded ? false : GetIsWallClung();
        facingDirY = inputY == 0 ? Vector2.zero : inputY > 0 ? Vector2.up : Vector2.down;
        // Get whether player is holding shift to decide whether to sprint or walk
        bool dash = Input.GetKeyDown(KeyCode.LeftShift);

        // Dash stuff
        if (dash && dashCount > 0)
        {
            // If dash is on cooldown and you have no dashes left in the air, spend spirit to overcast
            if (!canDash)
            {
                player.health.Damage(dashOvercastCost, DamageSource.Self);
            }

            Vector2 dashDir = new Vector2(inputX, inputY);
            if (dashDir == Vector2.zero || (!canDashUp && dashDir.y > 0)) dashDir = facingDirX;

            --dashCount;
            dashParticles.Play();

            player.BroadcastDashStart();
            StartCoroutine(Dash(dashDir));
        }

        // Set the X axis movement equal to input direction multiplied by player speed
        // deltaTime used to make sure speed is consistent across framerates
        float targetSpeed = inputX * walkSpeed;
        float targetSpeedY = inputY * walkSpeed;

        // Apply movement to the player rigidbody
        if(canWallCling)
            rigidBody.linearVelocity = new Vector2(targetSpeed, isDebugOn ? targetSpeedY : rigidBody.linearVelocity.y);

        if (inputX >= 0.04f || inputX <= -0.04f)
        {
            animator.SetBool("isRunning", true);
            isRunning = true;

            if (canPlayWalk && isGrounded)
            {
                StartCoroutine(WalkSound());
            }
        }
        else if (animator.GetBool("isRunning"))
        {
            animator.SetBool("isRunning", false);
            isRunning = false;
        }

        // Jump stuff
        bool jump = Input.GetButtonDown("Jump");

        if (isGrounded)
        {
            animator.SetBool("isFalling", false);
            animator.SetBool("isJumping", false);
            isJumping = false;
            isFalling = false;
            coyoteTimer = coyoteGracePeriod;
            jumpCount = maxJumpCount;
            dashCount = maxDashCount;
        }
        else
        {
            rigidBody.gravityScale = gravityDown;
            coyoteTimer -= Time.deltaTime;

            if (rigidBody.linearVelocityY < -0.1f)
            {
                animator.SetBool("isFalling", true);
                animator.SetBool("isJumping", false);
                isFalling = true;
                isJumping = false;
                if (inputY < 0f)
                {
                    rigidBody.gravityScale = gravityDown;
                }
                else
                {
                    rigidBody.gravityScale = gravityFalling;
                }
            }
            if (rigidBody.linearVelocityY > 0)
            {
                animator.SetBool("isJumping", true);
                animator.SetBool("isFalling", false);
                isJumping = true;
                isFalling = false;
                rigidBody.gravityScale = gravityNormal;

            }
        }


        if (!isWallClung && jump && (coyoteTimer > 0f || jumpCount > 0))
        {
            if(!(coyoteTimer > 0f)) --jumpCount;
            if(maxJumpCount > 0 && !isGrounded)
            {
                StartCoroutine(PlayParticlesForSeconds(doubleJumpParticles, 0.2f));
            }
            coyoteTimer = 0f;
            Jump();
        }

        // Wall-Jump stuff
        if (!isGrounded && isWallClung && canWallCling)
        {
            if(canPlayCling)
                StartCoroutine(ClingSound());

            jumpCount = maxJumpCount;
            dashCount = maxDashCount;
            if (rigidBody.linearVelocity.y <= 0)
            {
                rigidBody.linearVelocity = new Vector2(rigidBody.linearVelocity.x, Mathf.Clamp(rigidBody.linearVelocity.y, -wallFriction, float.MaxValue));
            }
            if (jump)
            {
                coyoteTimer = 0f;
                WallJump(facingDirX);
            }
        }
        if(!isWallClung)
        {
            canPlayCling = true;
        }

        // Flip the player sprite depending on movement direction
        FlipPlayerSprite(inputX);
    }

    private void Jump()
    {
        audioManager.PlaySFXOneShot(jumpSound);
        rigidBody.linearVelocityY = 0;
        rigidBody.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
    }
    private void WallJump(Vector2 facing)
    {
        audioManager.PlaySFXOneShot(jumpSound);
        rigidBody.linearVelocity = Vector2.zero;
        rigidBody.AddForce(new Vector2(-facing.x*3f, jumpHeight), ForceMode2D.Impulse);
        StartCoroutine(WallCooldown(wallJumpCooldown));
    }

    private IEnumerator Dash(Vector2 direction)
    {
        StartCoroutine(DashCooldown(dashCooldown));

        audioManager.PlaySFXOneShot(dashSound);

        rigidBody.gravityScale = 0;
        rigidBody.linearVelocity = Vector2.zero;

        Vector2 startPos = rigidBody.position;
        Vector2 targetPos = startPos + (direction.normalized * dashDistance);

        float elapsed = 0f;
        while (elapsed < dashDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / dashDuration;
            rigidBody.MovePosition(Vector2.Lerp(startPos, targetPos, dashCurve.Evaluate(t)));
            yield return null;
        }

        rigidBody.gravityScale = 2;
        rigidBody.AddForce(direction*4f, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.2f);
        dashParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        player.BroadcastDashComplete();

        /*if (direction.y < 0)
        {
            playerAttack.TriggerSlam(direction);
        }*/
    }

    private bool GetIsGrounded()
    {
        Debug.DrawRay(transform.position + (Vector3.left * 0.2f), Vector2.down * jumpBounds, Color.blue);
        bool leftRay = Physics2D.Raycast(transform.position + (Vector3.left * 0.2f), Vector2.down, jumpBounds, LayerMask.GetMask("Ground"));

        Debug.DrawRay(transform.position + (Vector3.right * 0.2f), Vector2.down * jumpBounds, Color.blue);
        bool rightRay = Physics2D.Raycast(transform.position + (Vector3.right * 0.2f), Vector2.down, jumpBounds, LayerMask.GetMask("Ground"));
        return leftRay || rightRay;
    }

    private bool GetIsWallClung()
    {
        Debug.DrawRay(transform.position + (Vector3.up * 0.75f), facingDirX * wallClingBounds, Color.blue);
        Debug.DrawRay(transform.position - (Vector3.up * 0.5f), facingDirX * wallClingBounds, Color.blue);
        bool topRay = Physics2D.Raycast(transform.position + (Vector3.up * 0.75f), facingDirX, wallClingBounds, LayerMask.GetMask("Wall"));
        bool bottomRay = Physics2D.Raycast(transform.position - (Vector3.up * 0.5f), facingDirX, wallClingBounds, LayerMask.GetMask("Wall"));
        return topRay || bottomRay;
    }

    private void FlipPlayerSprite(float input)
    {
        if (input > 0)
        {
            // If moving right, current x > previous x, do not flip sprite
            spriteRenderer.flipX = false;
            facingDirX = Vector2.right;
        }
        else if (input < 0)
        {
            // If moving left, current x < prev x, flip sprite on the x axis
            spriteRenderer.flipX = true;
            facingDirX = Vector2.left;
        }
    }

    private IEnumerator WallCooldown(float seconds)
    {
        if (canWallCling == false) yield break;

        canWallCling = false;
        yield return new WaitForSeconds(seconds);

        canWallCling = true;
    }

    private IEnumerator DashCooldown(float seconds)
    {
        if (canDash == false) yield break;

        canDash = false;
        yield return new WaitForSeconds(seconds);

        canDash = true;
    }

    private IEnumerator WalkSound()
    {
        canPlayWalk = false;
        audioManager.PlaySFXOneShot(walkSound);
        yield return new WaitForSeconds(0.333f);
        canPlayWalk = true;
    }

    private IEnumerator ClingSound()
    {
        canPlayCling = false;
        audioManager.PlaySFXOneShot(clingSound);
        yield return null;
    }

    private IEnumerator PlayParticlesForSeconds(ParticleSystem particle, float seconds)
    {
        particle.Play();
        yield return new WaitForSeconds(seconds);
        particle.Stop();
    }
}
