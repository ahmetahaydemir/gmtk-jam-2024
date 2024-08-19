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
    public AudioSource getHitAudioSource;
    public bool dead;
    public EnemyBehaviour enemyBehaviour;
    public float attackTimer;
    public bool attackToken;
    public AudioSource attackAudioSource;
}
public enum EnemyBehaviour
{
    Neutral,
    Escape,
    Chase,
    Attack,
    Boss
}