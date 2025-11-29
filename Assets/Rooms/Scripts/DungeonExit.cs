using UnityEngine;

public class DungeonExit : MonoBehaviour
{
    [SerializeField] private TransitionFadeScript fade;
    private bool playerInside = false;
    private void Awake()
    {
        fade = GameObject.FindAnyObjectByType<TransitionFadeScript>();
    }

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
            if(enter) fade.FadeToScene(4);
        }
    }
}
