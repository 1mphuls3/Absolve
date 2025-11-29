using UnityEngine;
using UnityEditor;
using System.Collections;
using Unity.VisualScripting;

public class Fireball : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidBody;
    [SerializeField] private LayerMask groundLayerMask;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private int damageAmount = 15;
    [SerializeField] private float lifestealPercent = 0.5f;

    private PlayerHealth health;
    public Player player;
    private void Start()
    {
        health = player.health;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            DamageData damage = new DamageData()
            {
                baseDamage = damageAmount,
                multiplier = 1f
            };

            EnemyHealth enemyHealth = collision.gameObject.GetComponent<EnemyHealth>();

            player.BroadcastHitEnemy(enemyHealth, ref damage);

            enemyHealth.Damage(Mathf.RoundToInt(damage.Resolve()));
            health.Heal(Mathf.RoundToInt(Mathf.RoundToInt(damage.Resolve()) * lifestealPercent));

            particles.GetComponent<ParticleDetach>().DetachParticles();
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("Ground"))
        {
            particles.GetComponent<ParticleDetach>().DetachParticles();
            Destroy(gameObject);
        }
    }
}
