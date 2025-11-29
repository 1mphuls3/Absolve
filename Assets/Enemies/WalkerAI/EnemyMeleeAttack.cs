using UnityEngine;

public class EnemyMeleeAttack : MonoBehaviour
{
    [SerializeField] private int damage;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerHealth>().Damage(damage, DamageSource.Enemy);
        }
    }
}
