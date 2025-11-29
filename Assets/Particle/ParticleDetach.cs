using System.Collections;
using UnityEngine;

public class ParticleDetach : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles;
    public void DetachParticles()
    {
        particles.transform.parent = null;

        StartCoroutine(DestroyParticleSystem(particles));
    }

    private IEnumerator DestroyParticleSystem(ParticleSystem particles)
    {
        particles.Play();
        yield return new WaitForSeconds(0.2f);
        particles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        while (particles.IsAlive())
        {
            yield return null;
        }
        Destroy(particles.gameObject);
    }
}
