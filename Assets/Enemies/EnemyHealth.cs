using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private AnimationCurve lerpCurve;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private ParticleDetach particleDetach;
    [SerializeField] private Player player;

    [SerializeField] private AudioClip damageSound;
    private AudioManager audioManager;

    public float flashDuration;

    public int maxHealth;
    public int currentHealth { get; private set; }
    void Start()
    {
        player = GetComponent<EnemyAI>().player.GetComponent<Player>();
        audioManager = player.audioManager;
        currentHealth = maxHealth;
    }

    public void Damage(int amount)
    {
        StartCoroutine(FlashCoroutine());
        SpawnParticles();
        currentHealth -= amount;

        audioManager.PlaySFXOneShot(damageSound);

        //OnTakeDamage.Invoke(amount);
        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    void Die()
    {
        StartCoroutine(WaitStop());
        particleDetach.DetachParticles();
        Destroy(this.gameObject);
    }

    private IEnumerator FlashCoroutine()
    {
        spriteRenderer.material.SetColor("_FlashColor", Color.white);
        float elapsedTime = 0f;
        while(elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / flashDuration;
            spriteRenderer.material.SetFloat("_FlashAmount", Mathf.Lerp(1f, 0f, lerpCurve.Evaluate(t)));
            yield return null;
        }
    }

    private void SpawnParticles()
    {
        Vector2 facing = player.movement.facingDirX + player.movement.facingDirY;
        Vector3 rotation = new Vector3(0, 0, 67.5f + facing.y * 90 - facing.x * 90);
        print(rotation);
        particles.transform.rotation = Quaternion.Euler(rotation);
        particles.Play();
        StartCoroutine(WaitStop());
    }

    private IEnumerator WaitStop()
    {
        yield return new WaitForSeconds(0.25f);
        particles.Stop();
    }
}
