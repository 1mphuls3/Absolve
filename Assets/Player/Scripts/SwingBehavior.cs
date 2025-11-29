using UnityEngine;

public class SwingBehavior : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float knockbackForce = 2f;
    [SerializeField] private int damageAmount = 10;
    [SerializeField] private float lifestealPercent = 2f;

    private PlayerHealth health;
    private void Start()
    {
        health = player.gameObject.GetComponent<PlayerHealth>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyHealth enemy = collision.gameObject.GetComponent<EnemyHealth>();

            DamageData damage = new DamageData()
            {
                baseDamage = damageAmount,
                multiplier = 1f
            };

            player.BroadcastHitEnemy(enemy, ref damage);

            enemy.Damage(Mathf.RoundToInt(damage.Resolve()));
            health.Heal(Mathf.RoundToInt(damage.Resolve() * lifestealPercent));

            KnockbackPlayer();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            KnockbackPlayer();
        }
    }

    private void KnockbackPlayer()
    {
        player.movement.rigidBody.AddForce((-1 * player.movement.facingDirX) * knockbackForce, ForceMode2D.Impulse);
    }
}
