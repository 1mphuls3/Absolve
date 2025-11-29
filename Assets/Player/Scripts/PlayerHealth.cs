using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public enum DamageSource
{
    Enemy,
    Environment,
    Self
}

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] public SpriteRenderer spriteRenderer;
    [SerializeField] private AnimationCurve lerpCurve;
    [SerializeField] private float flashDuration;
    [SerializeField] private HealthBar healthBar;
    [SerializeField] private float iFrameTime = 0.5f;

    [SerializeField] public Player player;

    [SerializeField] private TransitionFadeScript fade;

    [SerializeField] private CameraFollow playerCamera;

    [SerializeField] private Light2D ambientLight;
    private bool isDimmed;

    [SerializeField] private AudioClip damageSound;
    private AudioManager audioManager;

    public int maxHealth;
    public int currentHealth { get; private set; }
    public bool canDamage = true;

    void Start()
    {
        audioManager = player.audioManager;
        currentHealth = maxHealth;
    }

    private void Update()
    {
        if((float)currentHealth/(float)maxHealth < 0.2f)
        {
            playerCamera.isCameraShake = true;
            playerCamera.cameraShakeSpeed = 2f;
            if(!isDimmed) StartCoroutine(DimLight());
        }
        else
        {
            playerCamera.isCameraShake = false;
            if (isDimmed) StartCoroutine(BrightenLight());
        }
    }

    public void Damage(int amount, DamageSource source)
    {
        DamageData damage = new DamageData()
        {
            baseDamage = amount,
            multiplier = 1f
        };

        if (canDamage == false && source != DamageSource.Self)
            return;

        if (source != DamageSource.Self)
        {
            audioManager.PlaySFXOneShot(damageSound);

            StartCoroutine(playerCamera.ShakeCamera(1f, 2f));

            player.BroadcastTakeDamage(ref damage);
            StartCoroutine(DamageCheck());
            StartCoroutine(FlashCoroutine());
        }
        else
        {
            player.BroadcastTakeSelfDamage(ref damage);
        }

        currentHealth -= Mathf.RoundToInt(damage.Resolve());
        healthBar.UpdateHealthBar();

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        healthBar.UpdateHealthBar();

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    void Die()
    {
        fade.FadeToScene(3);
    }

    /*private IEnumerator IFrameFlash()
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        Color originalColor = spriteRenderer.color;

        float elapsed = 0f;
        while (elapsed < iFrameTime)
        {
            float t = elapsed / iFrameTime;
            float alpha = Mathf.Abs(Mathf.Sin(elapsed * 20f));
            spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            yield return null;
        }
        spriteRenderer.color = originalColor;
    }*/

    private IEnumerator DamageCheck()
    {
        if (canDamage != true) yield break;

        canDamage = false;
        yield return new WaitForSeconds(iFrameTime);

        canDamage = true;
    }

    private IEnumerator FlashCoroutine()
    {
        spriteRenderer.material.SetColor("_FlashColor", Color.white);
        float elapsedTime = 0f;
        while (elapsedTime < flashDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / flashDuration;
            spriteRenderer.material.SetFloat("_FlashAmount", Mathf.Lerp(1f, 0f, lerpCurve.Evaluate(t)));
            yield return null;
        }
    }

    private IEnumerator DimLight()
    {
        isDimmed = true;
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed;
            ambientLight.intensity = Mathf.Lerp(0.2f, 0.02f, t);
            yield return null;
        }
    }

    private IEnumerator BrightenLight()
    {
        isDimmed = false;
        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            float t = elapsed;
            ambientLight.intensity = Mathf.Lerp(0.02f, 0.2f, t);
            yield return null;
        }
    }
}
