using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "SpectralMomentum", menuName = "Scriptable Objects/SpectralMomentum")]
public class SpectralMomentum : EchoAbillity
{
    [SerializeField] private float speedMult;
    [SerializeField] private float speedTime;
    private bool isSpeed;

    // When you hit an enemy, give a speed boost for a set amount of time
    public override void OnHitEnemy(Player player, ref DamageData damage)
    {
        // If you already have a speed boost, don't apply it again
        if (isSpeed) return;

        float walkSpeed = player.movement.walkSpeed;
        player.movement.walkSpeed *= speedMult;

        // Set speed back to previous saved speed after the timer
        player.StartCoroutine(SpeedCooldown(player, walkSpeed));
    }

    private IEnumerator SpeedCooldown(Player player, float walkSpeed)
    {
        isSpeed = true;
        player.movement.speedParticles.Play();
        yield return new WaitForSeconds(speedTime);

        player.movement.speedParticles.Stop();
        player.movement.walkSpeed = walkSpeed;
        isSpeed = false;
    }
}
