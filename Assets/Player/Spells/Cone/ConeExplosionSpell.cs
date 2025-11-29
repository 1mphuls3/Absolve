using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ConeExplosionSpell : MonoBehaviour
{
    [SerializeField] private float chargingDuration;
    [SerializeField] private float explosionDuration;
    [SerializeField] private float animationDuration;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Collider2D hitCollider;
    [SerializeField] private int damageAmount = 20;
    [SerializeField] private float lifestealPercent = 0.5f;
    [SerializeField] private Light2D light2D;

    [SerializeField] private AudioManager audioManager;
    [SerializeField] private AudioClip explosionSound;

    private PlayerHealth health;
    public Player player;

    void Start()
    {
        audioManager = player.audioManager;
        light2D.transform.localScale = new Vector3(spriteRenderer.flipX ? 1 : -1, 1, 1);
        health = player.health;
        hitCollider.enabled = false;
        StartCoroutine(SpellSequence());
    }

    private IEnumerator SpellSequence()
    {
        light2D.intensity = 0.5f;
        yield return new WaitForSeconds(chargingDuration);
        gameObject.transform.parent = null;
        light2D.intensity = 1f;
        yield return new WaitForSeconds(explosionDuration);
        audioManager.PlaySFXOneShot(explosionSound);
        hitCollider.enabled = true;
        light2D.intensity = 0.5f;
        yield return new WaitForSeconds(animationDuration);
        hitCollider.enabled = false;

        Destroy(spriteRenderer.gameObject);
        StartCoroutine(DimLight());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
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
        }
    }

    private IEnumerator DimLight()
    {   
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            light2D.intensity = Mathf.Lerp(0.8f, 0f, t);
            yield return null;
        }

        Destroy(gameObject);
    }
}
