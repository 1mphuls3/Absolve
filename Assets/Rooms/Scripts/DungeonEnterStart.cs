using UnityEngine;

public class DungeonEnterStart : MonoBehaviour
{
    [SerializeField] private TransitionFadeScript fade;
    private bool playerInside = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
        }
    }

    private void Update()
    {
        if (playerInside)
        {
            bool enter = Input.GetButtonDown("Interact");
            if(enter) fade.FadeToScene(2);
        }
    }
}
