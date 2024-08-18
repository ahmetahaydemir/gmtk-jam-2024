using ProjectDawn.Navigation.Hybrid;
using UnityEngine;
public class EnemyData : MonoBehaviour
{
    public AgentAuthoring agent;
    public Transform mesh;
    public Animator animator;
    public float mass;
    public CapsuleCollider capsuleCollider;
    public GameObject deathVFX;
    public AudioSource audioSource;
    public bool dead;
}