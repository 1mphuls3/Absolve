using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TransitionFadeScript : MonoBehaviour
{
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private AnimationCurve curveIn;
    [SerializeField] private AnimationCurve curveOut;
    [SerializeField] public AudioManager audioManager;
    void Start()
    {
        fadeImage.color = new Color(0, 0, 0, 1);
        StartCoroutine(FadeIn());
    }

    public void FadeToScene(int scene)
    {
        StartCoroutine(FadeOut(scene));
    }

    private IEnumerator FadeIn()
    {
        StartCoroutine(audioManager.FadeMusicIn());
        yield return new WaitForSeconds(0.5f);
        float t = 1f;
        while (t > 0f)
        {
            t -= Time.deltaTime / fadeSpeed;
            float alpha = curveIn.Evaluate(t);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
    }

    private IEnumerator FadeOut(int scene)
    {
        Coroutine coroutine = StartCoroutine(audioManager.FadeMusicOut());
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime / fadeSpeed;
            float alpha = curveOut.Evaluate(t);
            fadeImage.color = new Color(0, 0, 0, alpha);
            yield return null;
        }
        yield return coroutine;
        SceneManager.LoadScene(scene);
    }
}
