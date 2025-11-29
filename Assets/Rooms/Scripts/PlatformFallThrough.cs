using UnityEngine;
using UnityEngine.Tilemaps;

public class PlatformFallThrough : MonoBehaviour
{
    [SerializeField] private CompositeCollider2D compositeCollider;
    [SerializeField] private float resetDelay = 0.5f;
    private bool isDropping = false;
    private Collider2D playerCollider;

    private void Start()
    {
        playerCollider = GameObject.FindWithTag("Player").GetComponent<Collider2D>();
    }

    void Update()
    {
        float inputY = Input.GetAxisRaw("Vertical");
        if (inputY < 0)
        {
            StartCoroutine(Drop());
        }
    }

    private System.Collections.IEnumerator Drop()
    {
        if (isDropping) yield break;
        isDropping = true;

        Physics2D.IgnoreCollision(compositeCollider, playerCollider, true);

        yield return new WaitForSeconds(resetDelay);

        Physics2D.IgnoreCollision(compositeCollider, playerCollider, false);
        isDropping = false;
    }
}
