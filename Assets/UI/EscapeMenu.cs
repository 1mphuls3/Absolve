using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EscapeMenu : MonoBehaviour
{
    [SerializeField] private List<Image> images = new List<Image>();
    [SerializeField] private List<RawImage> rawImages = new List<RawImage>();
    [SerializeField] private List<TextMeshProUGUI> texts = new List<TextMeshProUGUI>();
    [SerializeField] private Image darkenImage;
    [SerializeField] private TransitionFadeScript fade;

    private bool isMenuOpen = false;

    public void StartGame()
    {
        fade.FadeToScene(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void Tutorial()
    {
        fade.FadeToScene(1);
    }

    void Start()
    {
        foreach (var image in images)
        {
            image.gameObject.SetActive(false);
        }
        foreach (var image in rawImages)
        {
            image.gameObject.SetActive(false);
        }
        foreach (var text in texts)
        {
            text.gameObject.SetActive(false);
        }
        darkenImage.gameObject.SetActive(false);
    }

    void Update()
    {
        bool esc = Input.GetKeyDown(KeyCode.Escape);
        if (esc && !isMenuOpen)
        {
            isMenuOpen = true;
            StartCoroutine(FadeIn());
        }
        else if (esc && isMenuOpen)
        {
            isMenuOpen = false;
            StartCoroutine(FadeOut());
        }
    }

    private IEnumerator FadeIn()
    {
        images.ForEach(image => { image.gameObject.SetActive(true); });
        rawImages.ForEach(image => { image.gameObject.SetActive(true); });
        texts.ForEach(text => { text.gameObject.SetActive(true); });
        darkenImage.gameObject.SetActive(true);

        float elapsed = 0f;
        while (elapsed < 1f)
        {
            elapsed += Time.deltaTime;
            foreach (var image in images)
            {
                Color color = image.color;
                image.color = new Color(color.r, color.g, color.b, elapsed);
            }
            foreach (var image in rawImages)
            {
                Color color = image.color;
                image.color = new Color(color.r, color.g, color.b, elapsed);
            }
            foreach (var text in texts)
            {
                Color color = text.color;
                text.color = new Color(color.r, color.g, color.b, elapsed);
            }
            darkenImage.color = new Color(0, 0, 0, elapsed/1.5f);
            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 1f;
        while (elapsed > 0f)
        {
            elapsed -= Time.deltaTime;
            foreach (var image in images)
            {
                Color color = image.color;
                image.color = new Color(color.r, color.g, color.b, elapsed);
            }
            foreach (var image in rawImages)
            {
                Color color = image.color;
                image.color = new Color(color.r, color.g, color.b, elapsed);
            }
            foreach (var text in texts)
            {
                Color color = text.color;
                text.color = new Color(color.r, color.g, color.b, elapsed);
            }
            darkenImage.color = new Color(0, 0, 0, elapsed / 1.5f);
            yield return null;
        }

        images.ForEach(image => { image.gameObject.SetActive(false); });
        rawImages.ForEach(image => { image.gameObject.SetActive(false); });
        texts.ForEach(text => { text.gameObject.SetActive(false); });
        darkenImage.gameObject.SetActive(false);
    }
}
