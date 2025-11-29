using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    //[SerializeField] private ParticleSystem particleSystem;

    public void Spawn()
    {
        GameObject enemy = Instantiate(enemyPrefab, this.transform.position, Quaternion.identity);
        enemy.GetComponent<EnemyAI>().player = GameObject.FindGameObjectWithTag("Player").transform;
    }
}
