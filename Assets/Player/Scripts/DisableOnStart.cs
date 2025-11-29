using System.Collections;
using UnityEngine;

public class DisableOnStart : MonoBehaviour
{
    [SerializeField] public bool onGenerate = true;

    // After dungeon gen, wait 0.2 seconds before disabling
    public void DelayDisable()
    {
        StartCoroutine(SpawnWait());
    }

    private void Start()
    {
        if(!onGenerate)
            StartCoroutine(SpawnWait());
    }

    void OnEnable()
    {
        RoomManager.OnGenerationComplete += DelayDisable;
    }
    void OnDisable()
    {
        RoomManager.OnGenerationComplete -= DelayDisable;
    }
    private IEnumerator SpawnWait()
    {
        yield return new WaitForSeconds(0.2f);
        gameObject.SetActive(false);
    }
}
