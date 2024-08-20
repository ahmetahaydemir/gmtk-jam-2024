using ProjectDawn.Navigation.Hybrid;
using UnityEngine;
public class EnemyData : MonoBehaviour
{
    public AgentAuthoring agent;
    public Transform mesh;
    public Animator animator;
    public float mass;
    public float sizeRandMult;
    public CapsuleCollider capsuleCollider;
    public GameObject deathVFX;
    public GameObject getHitVFX;
    public AudioSource getHitAudioSource;
    public AudioSource deathAudioSource;
    public bool dead;
    public EnemyBehaviour enemyBehaviour;
    public float attackTimer;
    public bool attackToken;
    public float attackMassAmount;
    public float attackRecoveryTime;
    public AudioSource attackAudioSource;
    public float healthMass;
    //
    public float GetHealthRatio()
    {
        return (healthMass * 1f) / (mass * 1f);
    }
}
public enum EnemyBehaviour
{
    Neutral,
    Escape,
    Chase,
    Attack,
    Boss
}