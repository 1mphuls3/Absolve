using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class EchoPickup : MonoBehaviour
{
    [SerializeField] public List<EchoAbillity> echoesList;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Player player;
    [SerializeField] private TextMeshProUGUI nameUI;
    [SerializeField] private TextMeshProUGUI descUI;
    [SerializeField] private float fadeSpeed = 1f;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private ParticleSystem particles;
    [SerializeField] private Light2D light2D;
    [SerializeField] private float lightFadeSpeed = 1f;
    
    public EchoAbillity echoData;
    public Vector3 nameOffset = Vector3.zero;
    public Vector3 descOffset = Vector3.zero;

    public bool playerInside = false;
    public float timer = 0f;

    private void Update()
    {
        timer += Time.deltaTime;

        spriteRenderer.gameObject.transform.position = new Vector3(0f, Mathf.Sin(timer * floatSpeed) /4f, 0f) + gameObject.transform.position;

        bool interact = Input.GetButtonDown("Interact");
        if (interact && playerInside)
        {
            TryPickup();
        }
    }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        nameUI = GameObject.FindWithTag("UI1").GetComponent<TextMeshProUGUI>();
        descUI = GameObject.FindWithTag("UI3").GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        echoData = echoesList[Random.Range(0, echoesList.Count)];
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            nameUI.transform.position = transform.position + nameOffset;
            descUI.transform.position = transform.position + descOffset;
            playerInside = true;
            ShowText();
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInside = false;
            HideText();
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (playerInside && nameUI != null && descUI != null)
        {
            // Position the text relative to the collider
            nameUI.transform.position = transform.position + nameOffset;
            descUI.transform.position = transform.position + descOffset;
        }
    }

    private void TryPickup()
    {
        var echoManager = player.GetComponent<EchoManager>();
        if (echoManager.EquipEcho(echoData))
        {
            particles.Stop();
            HideText();
            Destroy(gameObject);
            StartCoroutine(FadeLight());
        }
    }

    void ShowText()
    {
        if (nameUI != null && descUI != null)
        {
            nameUI.text = echoData.echoName;
            descUI.text = echoData.description;

            StartCoroutine(FadeText(true));
        }
    }

    void HideText()
    {
        if (nameUI != null && descUI != null)
        {
            StartCoroutine(FadeText(false));
        }
    }

    private IEnumerator FadeText(bool fadeIn)
    {
        if (fadeIn)
        {
            nameUI.color = new Color(1, 1, 1, 0);
            descUI.color = new Color(1, 1, 1, 0);
            nameUI.gameObject.SetActive(true);
            descUI.gameObject.SetActive(true);
        }

        float elapsed = fadeIn ? 0f : 1f;
        while (fadeIn ? elapsed < fadeSpeed : elapsed > 0f)
        {
            AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
            elapsed += fadeIn ? Time.deltaTime / fadeSpeed : -(Time.deltaTime / fadeSpeed);
            float a = curve.Evaluate(elapsed);
            nameUI.color = new Color(1, 1, 1, a);
            descUI.color = new Color(1, 1, 1, a);
            yield return null;
        }

        if (!fadeIn)
        {
            nameUI.color = new Color(1, 1, 1, 0);
            descUI.color = new Color(1, 1, 1, 0);
            nameUI.gameObject.SetActive(false);
            descUI.gameObject.SetActive(false);
        }
    }

    private IEnumerator FadeLight()
    {
        float elapsed = 1.5f;
        while (elapsed > 0f)
        {
            AnimationCurve curve = AnimationCurve.Linear(0, 0, 1, 1);
            elapsed -= (Time.deltaTime / lightFadeSpeed);
            float a = curve.Evaluate(elapsed);
            light2D.intensity = a;
            yield return null;
        }
    }
}
