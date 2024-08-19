using UnityEngine;
using DG.Tweening;
using System;
using ProjectDawn.Navigation;
using Unity.Entities.UniversalDelegates;
public class EnemyManager : MonoBehaviour
{
    public int initialSpawnCount;
    public EnemySpawnProb[] phaseOneSpawnPool;
    public EnemySpawnProb[] phaseTwoSpawnPool;
    public EnemySpawnProb[] phaseThreeSpawnPool;
    public GameObject phaseFiveSpawn;
    //
    [Header("Runtime Pool")]
    public EnemyData[] enemies;
    //
    private Vector3 targetDistanceVector;
    private float targetDistanceMagnitude;
    private float totalMass;
    //
    public void Awake()
    {
        totalMass = 0.1f;
    }
    //
    public void UpdateEnemyAction(Vector3 playerLocation, float waterBaseLevel)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] != null)
            {
                targetDistanceVector = playerLocation - enemies[i].transform.position;
                targetDistanceMagnitude = targetDistanceVector.sqrMagnitude;
                //
                if (!enemies[i].dead)
                {
                    //
                    switch (enemies[i].enemyBehaviour)
                    {
                        case EnemyBehaviour.Neutral:
                            Vector3 neutralPos = (enemies[i].transform.position - playerLocation).normalized * 6f;
                            neutralPos.y = Mathf.Clamp(neutralPos.y, waterBaseLevel, waterBaseLevel * (0.35f + enemies[i].sizeRandMult * 0.3f));
                            enemies[i].agent.SetDestination(neutralPos);
                            break;
                        case EnemyBehaviour.Escape:
                            if (totalMass < enemies[i].mass * 0.75f) { enemies[i].enemyBehaviour = EnemyBehaviour.Chase; }
                            //
                            Vector3 escapePos = (enemies[i].transform.position - playerLocation).normalized * 6f;
                            escapePos.y = Mathf.Clamp(escapePos.y, waterBaseLevel, waterBaseLevel * (0.35f + enemies[i].sizeRandMult * 0.3f));
                            enemies[i].agent.SetDestination(escapePos);
                            break;
                        case EnemyBehaviour.Chase:
                            enemies[i].transform.rotation = Quaternion.Lerp(enemies[i].transform.rotation,
                                Quaternion.LookRotation((playerLocation - enemies[i].transform.position).normalized, Vector3.up),
                                4f * 3f * Time.deltaTime);
                            if (totalMass > enemies[i].mass * 2f) { enemies[i].enemyBehaviour = EnemyBehaviour.Escape; }
                            //
                            if (targetDistanceMagnitude < 1f)
                            {
                                if (enemies[i].attackTimer < 0.33f)
                                {
                                    AgentBody attackBody = enemies[i].agent.EntityBody;
                                    attackBody.Destination = playerLocation + targetDistanceVector.normalized * 2f;
                                    attackBody.Force = targetDistanceVector.normalized;
                                    attackBody.Velocity = targetDistanceVector.normalized;
                                    attackBody.IsStopped = false;
                                    enemies[i].agent.EntityBody = attackBody;
                                    enemies[i].attackToken = true;
                                    enemies[i].attackTimer += Time.deltaTime;
                                }
                                else
                                {
                                    AgentBody attackBody = enemies[i].agent.EntityBody;
                                    attackBody.Destination = playerLocation + targetDistanceVector.normalized * 2f;
                                    attackBody.Force = targetDistanceVector.normalized * 7.5f;
                                    attackBody.Velocity = targetDistanceVector.normalized * 7.5f;
                                    attackBody.IsStopped = false;
                                    enemies[i].agent.EntityBody = attackBody;
                                    enemies[i].attackToken = true;
                                    enemies[i].attackTimer = 0f;
                                    enemies[i].enemyBehaviour = EnemyBehaviour.Attack;
                                    enemies[i].attackAudioSource.Play();
                                }
                            }
                            else
                            {
                                enemies[i].attackTimer = 0f;
                                enemies[i].agent.SetDestination(playerLocation);
                            }
                            break;
                        case EnemyBehaviour.Attack:
                            enemies[i].attackTimer += Time.deltaTime;
                            if (targetDistanceMagnitude < 0.25f && enemies[i].attackToken)
                            {
                                enemies[i].attackToken = false;
                                totalMass = Mathf.Max(0f, totalMass - 0.1f);
                                GameManager.Instance.OnPlayerHit(enemies[i], totalMass);
                            }
                            if (enemies[i].attackTimer > 1.66f)
                            {
                                enemies[i].enemyBehaviour = EnemyBehaviour.Chase;
                                enemies[i].attackToken = false;
                                enemies[i].attackTimer = 0f;
                            }
                            break;
                    }
                }
            }
        }
    }
    public void SpawnEnemyObject(Vector3 playerLocation, EnemyBehaviour behaviour, float waterBaseLevel)
    {
        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null)
            {
                Vector3 spawnPosCache = playerLocation + UnityEngine.Random.Range(-8f, 8f) * Vector3.right
                    + UnityEngine.Random.Range(-8f, 8f) * Vector3.forward + UnityEngine.Random.Range(0f, 2f) * Vector3.up;

                GameObject goCache = null;
                int rand = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 100f));
                if (waterBaseLevel >= -10f)
                {
                    // Phase 1
                    for (int j = 0; j < phaseOneSpawnPool.Length; j++)
                    {
                        if (rand <= phaseOneSpawnPool[j].spawnChance)
                        {
                            goCache = Instantiate(phaseOneSpawnPool[j].spawnPrefab, spawnPosCache, Quaternion.identity);
                            break;
                        }
                    }
                }
                else if (waterBaseLevel >= -50f)
                {
                    // Phase 2
                    for (int j = 0; j < phaseTwoSpawnPool.Length; j++)
                    {
                        if (rand <= phaseTwoSpawnPool[j].spawnChance)
                        {
                            goCache = Instantiate(phaseTwoSpawnPool[j].spawnPrefab, spawnPosCache, Quaternion.identity);
                            break;
                        }
                    }
                }
                else if (waterBaseLevel >= -100f)
                {
                    // Phase 3
                    for (int j = 0; j < phaseThreeSpawnPool.Length; j++)
                    {
                        if (rand <= phaseThreeSpawnPool[j].spawnChance)
                        {
                            goCache = Instantiate(phaseThreeSpawnPool[j].spawnPrefab, spawnPosCache, Quaternion.identity);
                            break;
                        }
                    }
                }
                //
                enemies[i] = goCache.GetComponent<EnemyData>();
                enemies[i].gameObject.name = "Enemy-" + i;
                if (enemies[i].animator != null)
                {
                    enemies[i].animator.Play("Idle_A");
                }
                enemies[i].enemyBehaviour = behaviour;
                float sizeRand = UnityEngine.Random.Range(0.75f, 1.25f);
                enemies[i].mesh.localScale = sizeRand * Vector3.one;
                enemies[i].mass *= sizeRand;
                enemies[i].sizeRandMult = sizeRand;
                enemies[i].getHitAudioSource.volume = 0.33f;
                return;
            }
        }
        //
        Debug.LogWarning("Spawn Enemy Request Warning - No Remaining Slot");
    }
    public void KillEnemy(int index)
    {
        if (enemies[index].animator != null)
        {
            enemies[index].animator.Play("Death", 0);
        }
        else
        {
            enemies[index].transform.DORotate(180f * Vector3.forward, 1.25f).SetEase(Ease.OutSine);
        }
        enemies[index].capsuleCollider.enabled = false;
        enemies[index].dead = true;
        enemies[index].agent.enabled = false;
        enemies[index].mesh.DOMove(enemies[index].transform.position - Vector3.up * (enemies[index].transform.position.y + 0.5f), 4f).SetEase(Ease.InOutSine);
        enemies[index].mesh.DOShakeRotation(5f, 30, 6).OnComplete(() =>
        {
            RemoveEnemyObject(index);
        });
        enemies[index].getHitAudioSource.Play();
        enemies[index].deathVFX.SetActive(true);
        //
        totalMass += enemies[index].mass * enemies[index].mesh.localScale.x * 0.25f;
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