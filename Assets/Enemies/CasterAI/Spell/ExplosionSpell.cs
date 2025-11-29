using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ExplosionSpell : MonoBehaviour
{
    [SerializeField] private float startupDuration;
    [SerializeField] private float chargingDuration;
    [SerializeField] private float explosionDuration;
    [SerializeField] private float animationDuration;
    [SerializeField] private Animator animator;
    [SerializeField] private Collider2D hitCollider;
    [SerializeField] private Light2D light2D;

    void Start()
    {
        hitCollider.enabled = false;
        StartCoroutine(SpellSequence());
    }

    private IEnumerator SpellSequence()
    {
        yield return new WaitForSeconds(startupDuration);

        animator.SetBool("isCharging", true);
        yield return new WaitForSeconds(chargingDuration);

        animator.SetBool("isCharging", false);
        animator.SetBool("isExploding", true);
        hitCollider.enabled = true;
        light2D.intensity = 5f;
        yield return new WaitForSeconds(explosionDuration);
        hitCollider.enabled = false;

        StartCoroutine(DimLight());
    }

    private IEnumerator DimLight()
    {
        float elapsed = 0f;
        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / animationDuration;
            light2D.intensity = Mathf.Lerp(5f, 0f, t);
            yield return null;
        }

        Destroy(gameObject);
    }
}
