using UnityEngine;
using ProjectDawn.Navigation.Hybrid;
public class EnemyManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public EnemyData[] enemies;
    public float movementSpeed = 2f;
    public float rotationSpeed = 2f;
    //
    private float targetDistanceCache;
    //
    public void UpdateEnemyAction(Vector3 playerLocation)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                // targetDistanceCache = (playerLocation - enemies[i].transform.position).sqrMagnitude;
                // if (targetDistanceCache > 1f)
                // {
                //     enemies[i].transform.position += movementSpeed * Time.deltaTime * (playerLocation - enemies[i].transform.position).normalized;
                //     enemies[i].transform.rotation = Quaternion.Lerp(enemies[i].transform.rotation, Quaternion.LookRotation((playerLocation - enemies[i].transform.position).normalized, Vector3.up), rotationSpeed * 3f * Time.deltaTime);
                // }
                enemies[i].agent.SetDestination(playerLocation);
            }
        }
    }
    public void SpawnEnemyObject(Vector3 playerLocation)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null)
            {
                Vector3 spawnPosCache = playerLocation + Random.Range(-5, 5) * Vector3.right
                    + Random.Range(-5, 5) * Vector3.forward;

                GameObject goCache = Instantiate(enemyPrefab, spawnPosCache, Quaternion.identity);

                // enemies[i] = goCache.transform;
                enemies[i] = goCache.GetComponent<EnemyData>();
                enemies[i].mesh.localScale = Random.Range(4f, 12f) * Vector3.one;
                Debug.Log("Spawn Enemy Index - " + i);
                return;
            }
        }
        //
        Debug.LogWarning("Spawn Enemy Request Warning - No Remaining Slot");
    }
    public void RemoveEnemyObject(int index)
    {
        if (enemies[index] != null)
        {
            Destroy(enemies[index].gameObject);
            //
            enemies[index] = null;
        }
    }
}