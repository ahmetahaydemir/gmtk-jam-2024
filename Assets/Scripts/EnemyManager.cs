using UnityEngine;
using DG.Tweening;
public class EnemyManager : MonoBehaviour
{
    public GameObject enemySmallPrefab;
    public GameObject enemyMediumPrefab;
    public GameObject enemyLargePrefab;
    public GameObject enemyBigPrefab;
    public EnemyData[] enemies;
    //
    private float targetDistanceCache;
    private float totalMass;
    //
    public void Awake()
    {
        PlayerManager.EnemyHit += KillEnemy;
    }
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
                if (!enemies[i].dead)
                {
                    enemies[i].agent.SetDestination(playerLocation);
                }
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

                GameObject goCache = null;
                int rand = Mathf.RoundToInt(Random.Range(0f, 100f));
                if (rand < 59)
                {
                    goCache = Instantiate(enemySmallPrefab, spawnPosCache, Quaternion.identity);
                }
                else if (rand < 69)
                {
                    goCache = Instantiate(enemyLargePrefab, spawnPosCache, Quaternion.identity);
                }
                else if (rand == 69)
                {
                    goCache = Instantiate(enemyBigPrefab, spawnPosCache, Quaternion.identity);
                }
                else if (rand >= 70)
                {
                    goCache = Instantiate(enemyMediumPrefab, spawnPosCache, Quaternion.identity);
                }

                // GameObject goCache = Instantiate(enemyPrefab, spawnPosCache, Quaternion.identity);

                enemies[i] = goCache.GetComponent<EnemyData>();
                enemies[i].gameObject.name = "Enemy-" + i;
                enemies[i].mesh.localScale = Random.Range(0.8f, 1.2f) * Vector3.one;
                Debug.Log("Spawn Enemy Index-" + i);
                return;
            }
        }
        //
        Debug.LogWarning("Spawn Enemy Request Warning - No Remaining Slot");
    }
    public void KillEnemy(int index)
    {
        enemies[index].animator.Play("Death", 0);
        enemies[index].capsuleCollider.enabled = false;
        enemies[index].dead = true;
        enemies[index].agent.enabled = false;
        enemies[index].mesh.DOMove(enemies[index].transform.position - Vector3.up * (enemies[index].transform.position.y + 0.5f), 4f).SetEase(Ease.InOutSine);
        enemies[index].mesh.DOShakeRotation(5f, 30, 6).OnComplete(() =>
        {
            RemoveEnemyObject(index);
        });
        enemies[index].audioSource.Play();
        enemies[index].deathVFX.SetActive(true);
        totalMass += enemies[index].mass * enemies[index].mesh.localScale.x;
    }
    public float GetTotalMass() { return totalMass; }
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