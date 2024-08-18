using UnityEngine;
using DG.Tweening;
using System;
public class EnemyManager : MonoBehaviour
{
    public int initialSpawnCount;
    public EnemySpawnProb[] enemySpawnProbs;
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
    public void UpdateEnemyAction(Vector3 playerLocation, float waterBaseLevel)
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
                    switch (enemies[i].enemyBehaviour)
                    {
                        case EnemyBehaviour.Neutral:
                            Vector3 neutralPos = (enemies[i].transform.position - playerLocation).normalized * 3f;
                            neutralPos.y = Mathf.Clamp(neutralPos.y, waterBaseLevel, waterBaseLevel * (0.35f + enemies[i].sizeRandMult * 0.3f));
                            enemies[i].agent.SetDestination(neutralPos);
                            break;
                        case EnemyBehaviour.Hostile:
                            if (enemies[i].mass + 2f > totalMass)
                            {
                                enemies[i].agent.SetDestination(playerLocation);
                            }
                            else
                            {
                                Vector3 escapePos = (enemies[i].transform.position - playerLocation).normalized * 10f;
                                escapePos.y = Mathf.Clamp(escapePos.y, waterBaseLevel, waterBaseLevel * (0.35f + enemies[i].sizeRandMult * 0.3f));
                                enemies[i].agent.SetDestination(escapePos);
                            }
                            break;
                    }
                }
            }
        }
    }
    public void SpawnEnemyObject(Vector3 playerLocation, EnemyBehaviour behaviour)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null)
            {
                Vector3 spawnPosCache = playerLocation + UnityEngine.Random.Range(-5, 5) * Vector3.right
                    + UnityEngine.Random.Range(-5, 5) * Vector3.forward;

                GameObject goCache = enemySpawnProbs[0].spawnPrefab;
                int rand = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 100f));
                for (int j = 0; j < enemySpawnProbs.Length; j++)
                {
                    if (rand <= enemySpawnProbs[j].spawnChance)
                    {
                        goCache = Instantiate(enemySpawnProbs[j].spawnPrefab, spawnPosCache, Quaternion.identity);
                        break;
                    }
                }
                // GameObject goCache = Instantiate(enemyPrefab, spawnPosCache, Quaternion.identity);

                enemies[i] = goCache.GetComponent<EnemyData>();
                enemies[i].gameObject.name = "Enemy-" + i;
                enemies[i].enemyBehaviour = behaviour;
                float sizeRand = UnityEngine.Random.Range(0.75f, 1.25f);
                enemies[i].mesh.localScale = sizeRand * Vector3.one;
                enemies[i].mass *= sizeRand;
                enemies[i].sizeRandMult = sizeRand;
                enemies[i].audioSource.volume = 0.3f;
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
[Serializable]
public class EnemySpawnProb
{
    public GameObject spawnPrefab;
    [Range(0, 100)] public float spawnChance;
}