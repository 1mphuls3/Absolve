using System.Collections;
using UnityEditor;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] public Player player;
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private Collider2D attackHitbox;
    [SerializeField] private GameObject fireballPrefab;
    [SerializeField] private GameObject explosionPrefab;

    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip fireballSound;

    [SerializeField] private int fireballCost;
    [SerializeField] private int explosionCost;
    private AudioManager audioManager;
    public bool canCastSpells = true;

    void Start()
    {
        audioManager = player.audioManager;
        attackHitbox.enabled = false;
    }

    void Update()
    {
        SyncAnimation();
        bool attack = Input.GetButtonDown("Fire1");
        bool attack2 = Input.GetButtonDown("Fire2");
        bool attack3 = Input.GetButtonDown("Fire3");
        bool isAttacking = animator.GetBool("isAttacking");

        float inputY = Input.GetAxis("Vertical");
        Vector2 playerPos = player.transform.position;
        Vector2 mouseScreenPos = Input.mousePosition;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 cursorDirection = (mouseWorldPos - playerPos);
        cursorDirection.Normalize();

        animator.SetBool("isRunning", player.movement.isRunning);
        animator.SetBool("isJumping", player.movement.isJumping);
        animator.SetBool("isFalling", player.movement.isFalling);

        if (!isAttacking)
        {
            spriteRenderer.flipX = player.movement.facingDirX == Vector2.left;
        }

        if (attack && !isAttacking)
        {
            if(inputY != 0)
            {
                float facingY = player.movement.facingDirY.y;
                float facingX = player.movement.facingDirX.x;
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, facingY * 90f * facingX);
                attackHitbox.transform.rotation = Quaternion.Euler(0, 0, facingY * 90f * facingX);
            }
            else
            {
                spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
                attackHitbox.transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            StartCoroutine(DoAttack());
        }
        else if (attack2 && canCastSpells)
        {
            float speedBonus = Mathf.Min(Vector2.Dot(player.movement.rigidBody.linearVelocity, cursorDirection), 0f);
            SpawnFireball((15f+(speedBonus/2f))*cursorDirection);
            StartCoroutine(SpellCooldown(0.2f));
        }
        else if (attack3 && canCastSpells)
        {
            SpawnExplosion(playerPos, player.movement.facingDirX);
            StartCoroutine(SpellCooldown(0.2f));
        }
    }

    private IEnumerator DoAttack()
    {
        int isWallClung = player.movement.isWallClung ? -1 : 1;

        audioManager.PlaySFXOneShot(attackSound);

        player.BroadcastAttack();

        animator.SetBool("isAttacking", true);
        attackHitbox.enabled = true;
        attackHitbox.offset = new Vector2(0.8625f * player.movement.facingDirX.x * isWallClung, attackHitbox.offset.y);
        yield return new WaitForSeconds(0.15f);
        attackHitbox.enabled = false;
        yield return new WaitForSeconds(0.337f);
        animator.SetBool("isAttacking", false);
        spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, 0);
        attackHitbox.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void SpawnFireball(Vector3 direction)
    {
        audioManager.PlaySFXOneShot(fireballSound);

        Vector3 pos = new Vector3(player.movement.facingDirX.x * 0.2f, 0.2f);
        Vector3 offset = player.movement.isWallClung ? -pos : pos;
        GameObject fireball = Instantiate(fireballPrefab, transform.position + offset, Quaternion.identity);
        fireball.GetComponent<Fireball>().player = player;
        fireball.GetComponent<Rigidbody2D>().linearVelocity = direction; // adjust speed

        playerHealth.Damage(fireballCost, DamageSource.Self);
    }
    private void SpawnExplosion(Vector2 pos, Vector2 facingDir)
    {
        GameObject wave = Instantiate(explosionPrefab, pos + facingDir, Quaternion.identity);
        wave.transform.SetParent(player.transform, true);
        wave.GetComponent<ConeExplosionSpell>().player = player;
        wave.transform.localScale = new Vector3(facingDir.x, 1, 1);

        playerHealth.Damage(explosionCost, DamageSource.Self);
    }

    private IEnumerator SpellCooldown(float seconds)
    {
        if (canCastSpells == false) yield break;

        canCastSpells = false;
        yield return new WaitForSeconds(seconds);

        canCastSpells = true;
    }

    private void SyncAnimation()
    {
        AnimatorStateInfo playerState = playerAnimator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo weaponState = playerAnimator.GetCurrentAnimatorStateInfo(0);

        if (playerState.IsName(weaponState.shortNameHash.ToString()))
        {
            animator.Play(weaponState.shortNameHash, 0, playerState.normalizedTime % 1);
        }

    }
}
