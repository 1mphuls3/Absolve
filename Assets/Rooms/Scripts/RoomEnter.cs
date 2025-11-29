using System;
using System.Collections;
using UnityEngine;

public class RoomEnter : MonoBehaviour
{
    [SerializeField] private SpawnEnemy[] spawners;
    private Collider2D roomTrigger;
    public bool canSpawn = false;
    public bool hasSpawned = false;

    private void Awake()
    {
        canSpawn = false;
    }

    public void DelayActivate()
    {
        StartCoroutine(SpawnWait());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canSpawn) return;
        if(collision.gameObject.CompareTag("Player") && hasSpawned == false && canSpawn)
        {
            foreach(SpawnEnemy spawner in spawners)
            {
                spawner.Spawn();
            }
            hasSpawned = true;
        }
    }
    void OnEnable()
    {
        RoomManager.OnGenerationComplete += DelayActivate;
    }
    void OnDisable()
    {
        RoomManager.OnGenerationComplete -= DelayActivate;
    }
    private IEnumerator SpawnWait()
    {
        yield return new WaitForSeconds(0.2f);
        canSpawn = true;
    }
}
