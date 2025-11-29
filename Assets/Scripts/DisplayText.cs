using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DisplayText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textUI;
    [SerializeField] private float fadeSpeed;
    public Vector3 textPosition = Vector3.zero;
    public string message = "NULL";

    private bool playerInside = false;
    void Start()
    {
        if (textUI == null)
        {
            textUI = GameObject.FindWithTag("UI1").GetComponent<TextMeshProUGUI>();
        }
        else
        {
            textUI.gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textUI.transform.position = transform.position + textPosition;
            playerInside = true;
            ShowText();
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            textUI.transform.position = Vector3.zero;
            playerInside = false;
            HideText();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (playerInside && textUI != null)
        {
            // Position the text relative to the collider
            textUI.transform.position = transform.position + textPosition;
        }
    }

    void ShowText()
    {
        if (textUI != null)
        {
            textUI.text = message;
            StartCoroutine(FadeText(true));
        }
    }

    void HideText()
    {
        if (textUI != null)
            StartCoroutine(FadeText(false));
    }

    private IEnumerator FadeText(bool fadeIn)
    {
        if (fadeIn)
        {
            textUI.gameObject.SetActive(true);
        }

        float elapsed = fadeIn ? 0f : 1f;
        while (fadeIn ? elapsed < fadeSpeed : elapsed > fadeSpeed)
        {
            AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
            elapsed += fadeIn ? Time.deltaTime / fadeSpeed : -(Time.deltaTime / fadeSpeed);
            float a = curve.Evaluate(elapsed);
            textUI.color = new Color(1, 1, 1, a);
            yield return null;
        }

        if (!fadeIn)
        {
            textUI.gameObject.SetActive(false);
        }
    }
}
