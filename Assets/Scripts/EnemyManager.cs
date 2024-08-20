using UnityEngine;
using DG.Tweening;
using System;
using ProjectDawn.Navigation;
using System.Linq.Expressions;
public class EnemyManager : MonoBehaviour
{
    [Header("Shallow Coast")]
    public float phaseOneSpawnDepth;
    public EnemySpawnProb[] phaseOneSpawnPool;
    [Header("Uninvited Current")]
    public float phaseTwoSpawnDepth;
    public EnemySpawnProb[] phaseTwoSpawnPool;
    [Header("Change of Scales")]
    public float phaseThreeSpawnDepth;
    public EnemySpawnProb[] phaseThreeSpawnPool;
    [Header("Far From Surface")]
    public float phaseFourSpawnDepth;
    public EnemySpawnProb[] phaseFourSpawnPool;
    [Header("Deep End")]
    public float phaseFiveSpawnDepth;
    public GameObject phaseFiveSpawn;
    //
    [Header("Runtime Pool")]
    public EnemyData[] enemies;
    public EnemyData bossEnemy;
    //
    private Vector3 targetDistanceVector;
    private float targetDistanceMagnitude;
    private float totalMass;
    //
    public void Awake()
    {
        totalMass = 0.5f;
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
                            if (totalMass < enemies[i].mass * 2f) { enemies[i].enemyBehaviour = EnemyBehaviour.Chase; }
                            //
                            Vector3 escapePos = (enemies[i].transform.position - playerLocation).normalized * 6f;
                            escapePos.y = Mathf.Clamp(escapePos.y, waterBaseLevel, waterBaseLevel * (0.35f + enemies[i].sizeRandMult * 0.3f));
                            enemies[i].agent.SetDestination(escapePos);
                            break;
                        case EnemyBehaviour.Chase:
                            enemies[i].transform.rotation = Quaternion.Lerp(enemies[i].transform.rotation,
                                Quaternion.LookRotation((playerLocation - enemies[i].transform.position).normalized, Vector3.up),
                                4f * 3f * Time.deltaTime);
                            if (totalMass > enemies[i].mass * 4f) { enemies[i].enemyBehaviour = EnemyBehaviour.Escape; }
                            //
                            if (targetDistanceMagnitude < enemies[i].attackDistance)
                            {
                                if (enemies[i].attackTimer < 0.4f)
                                {
                                    AgentBody attackBody = enemies[i].agent.EntityBody;
                                    attackBody.Destination = playerLocation + targetDistanceVector.normalized * (enemies[i].attackDistance * 0.5f + 2.25f);
                                    attackBody.Force = targetDistanceVector.normalized * 0.25f;
                                    attackBody.Velocity = targetDistanceVector.normalized * 0.25f;
                                    attackBody.IsStopped = false;
                                    enemies[i].agent.EntityBody = attackBody;
                                    if (!enemies[i].attackToken)
                                    {
                                        if (enemies[i].isBoss)
                                        {
                                            enemies[i].mesh.transform.DOPunchPosition((enemies[i].mesh.transform.forward - Vector3.up * 0.1f) * 0.1f, 0.5f).SetEase(Ease.InOutSine);
                                            enemies[i].mesh.transform.DOPunchScale(Vector3.one * 0.1f, 0.5f).SetEase(Ease.InOutSine);
                                        }
                                        else
                                        {
                                            enemies[i].mesh.transform.DOPunchPosition((enemies[i].mesh.transform.forward - Vector3.up * 0.05f) * 0.2f, 0.5f).SetEase(Ease.InOutSine);
                                        }
                                    }
                                    enemies[i].attackToken = true;
                                    enemies[i].attackTimer += Time.deltaTime;
                                }
                                else
                                {
                                    AgentBody attackBody = enemies[i].agent.EntityBody;
                                    if (enemies[i].isBoss)
                                    {
                                        attackBody.Destination = playerLocation + targetDistanceVector.normalized * (enemies[i].attackDistance * 0.8f + 8f);
                                        attackBody.Force = targetDistanceVector.normalized * 32f;
                                        attackBody.Velocity = targetDistanceVector.normalized * 32f;
                                    }
                                    else
                                    {
                                        attackBody.Destination = playerLocation + targetDistanceVector.normalized * (enemies[i].attackDistance * 0.45f + 2.25f);
                                        attackBody.Force = targetDistanceVector.normalized * (enemies[i].attackDistance * 0.4f + 8f);
                                        attackBody.Velocity = targetDistanceVector.normalized * (enemies[i].attackDistance * 0.4f + 8f);
                                    }
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
                            if (enemies[i].isBoss)
                            {
                                if (targetDistanceMagnitude < 6f && enemies[i].attackToken)
                                {
                                    enemies[i].attackToken = false;
                                    totalMass = Mathf.Max(0f, totalMass - enemies[i].attackMassAmount * enemies[i].sizeRandMult);
                                    GameManager.Instance.OnPlayerHit(enemies[i], totalMass);
                                }
                            }
                            else
                            {
                                if (targetDistanceMagnitude < (0.25f + enemies[i].attackMassAmount * 0.1f) && enemies[i].attackToken)
                                {
                                    enemies[i].attackToken = false;
                                    totalMass = Mathf.Max(0f, totalMass - enemies[i].attackMassAmount * enemies[i].sizeRandMult);
                                    GameManager.Instance.OnPlayerHit(enemies[i], totalMass);
                                }
                            }
                            if (enemies[i].attackTimer > enemies[i].attackRecoveryTime)
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
    public void SpawnEnemyObject(Vector3 playerLocation, float waterBaseLevel)
    {
        for (int i = 0; i < enemies.Length - 1; i++)
        {
            if (enemies[i] == null)
            {
                Vector3 spawnPosCache = playerLocation + UnityEngine.Random.Range(-12f, 12f) * Vector3.right
                    + UnityEngine.Random.Range(-12f, 12f) * Vector3.forward + UnityEngine.Random.Range(-4f, -1f) * Vector3.up;

                GameObject goCache = phaseOneSpawnPool[0].spawnPrefab;
                int rand = Mathf.RoundToInt(UnityEngine.Random.Range(0f, 100f));
                if (waterBaseLevel >= phaseOneSpawnDepth)
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
                else if (waterBaseLevel >= phaseTwoSpawnDepth)
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
                else if (waterBaseLevel >= phaseThreeSpawnDepth)
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
                else if (waterBaseLevel >= phaseFourSpawnDepth)
                {
                    // Phase 4
                    for (int j = 0; j < phaseFourSpawnPool.Length; j++)
                    {
                        if (rand <= phaseFourSpawnPool[j].spawnChance)
                        {
                            goCache = Instantiate(phaseFourSpawnPool[j].spawnPrefab, spawnPosCache, Quaternion.identity);
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
                float sizeRand = UnityEngine.Random.Range(0.75f, 1.25f);
                enemies[i].mesh.localScale = sizeRand * Vector3.one;
                enemies[i].mass *= sizeRand;
                enemies[i].healthMass = enemies[i].mass;
                enemies[i].sizeRandMult = sizeRand;
                enemies[i].getHitAudioSource.volume = 0.33f;
                return;
            }
        }
        //
        Debug.LogWarning("Spawn Enemy Request Warning - No Remaining Slot");
    }
    public void SpawnBossObject(Vector3 playerLocation, float waterBaseLevel)
    {
        Vector3 spawnPosCache = playerLocation + UnityEngine.Random.Range(-12f, -6f) * Vector3.right
                            + UnityEngine.Random.Range(-12f, -6f) * Vector3.forward + UnityEngine.Random.Range(-5f, 1f) * Vector3.up;
        spawnPosCache.y = Mathf.Clamp(spawnPosCache.y, waterBaseLevel + 1, -1f);

        GameObject goCache = Instantiate(phaseFiveSpawn, spawnPosCache, Quaternion.identity);

        for (int i = 0; i < enemies.Length; i++)
        {
            if (enemies[i] == null)
            {
                enemies[i] = goCache.GetComponent<EnemyData>();
                bossEnemy = enemies[i];
                enemies[i].gameObject.name = "Boss-" + i;
                if (enemies[i].animator != null)
                {
                    enemies[i].animator.Play("Idle_A");
                }
                enemies[i].healthMass = enemies[i].mass;
                enemies[i].sizeRandMult = 1f;
                enemies[i].getHitAudioSource.volume = 0.33f;
                return;
            }
        }
    }
    public void OnEnemyHit(int index, float charge)
    {
        if (totalMass * charge > enemies[index].healthMass)
        {
            // Killed
            if (enemies[index].animator != null)
            {
                enemies[index].animator.Play("Death", 0);
            }
            else
            {
                enemies[index].transform.DORotate(180f * Vector3.forward, 1.25f).SetEase(Ease.OutSine);
            }
            enemies[index].dead = true;
            enemies[index].capsuleCollider.enabled = false;
            enemies[index].agent.enabled = false;
            enemies[index].healthMass = 0f;
            enemies[index].mesh.DOMove(enemies[index].transform.position - Vector3.up * (enemies[index].transform.position.y + 0.5f), 4f).SetEase(Ease.InOutSine);
            enemies[index].mesh.DOScale(0f, 3f).SetEase(Ease.InOutSine);
            enemies[index].mesh.DOShakeRotation(4.5f, 25, 5).OnComplete(() =>
            {
                RemoveEnemyObject(index);
            });
            enemies[index].deathAudioSource.Play();
            enemies[index].deathVFX.SetActive(true);
            //
            totalMass += enemies[index].mass * enemies[index].mesh.localScale.x * 0.175f;
            GameManager.Instance.OnPlayerGrow((enemies[index].mass * enemies[index].mesh.localScale.x * 0.175f) / totalMass);
            //
            if (enemies[index] == bossEnemy)
            {
                GameManager.Instance.OnGameCompleted();
            }
        }
        else
        {
            // Just Hit
            enemies[index].healthMass -= totalMass * charge;
            enemies[index].mesh.DOPunchRotation(5f * Vector3.forward, 0.5f).SetEase(Ease.OutSine);
            enemies[index].getHitAudioSource.Play();
            enemies[index].getHitVFX.SetActive(false);
            enemies[index].getHitVFX.SetActive(true);
        }
        //
        if (enemies[index] == bossEnemy)
        {
            GameManager.Instance.UpdateBossState();
        }
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