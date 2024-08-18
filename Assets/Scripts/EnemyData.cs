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
    public AudioSource audioSource;
    public bool dead;
    public EnemyBehaviour enemyBehaviour;
}
public enum EnemyBehaviour
{
    Neutral,
    Hostile
}