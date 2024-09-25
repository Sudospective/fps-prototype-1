using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour,
    // Interfaces
    IDamage
{
    public enum EnemyType { Tank, Agile, Ranged }

    [Header("Components")]

    [Tooltip("The type of enemy determines their behavior, stats, weapon, and attack animation")]
    public EnemyType enemyType;

    [Tooltip("Component used to manage enemy movement")]
    [SerializeField] NavMeshAgent navAgent;

    [Tooltip("Component used to control animations")]
    [SerializeField] Animator animator;

    [Tooltip("")]
    [SerializeField] int animSpeedTime;

    [Header("Health")]

    [Tooltip("Max amount of HP")]
    [SerializeField] int HP;

    [Tooltip("Image used to fill enemy HP bar")]
    [SerializeField] Image HPBarImage;

    [Header("Shooting")]

    [Tooltip("The point where the enemy draws an arrow")]
    [SerializeField] Transform drawPos;
    [SerializeField] Transform headPos;

    //[Tooltip("The collider on the melee object")]
    //[SerializeField] Collider meleeCollider;

    [Tooltip("The collider on the arrow")]
    [SerializeField] Collider arrowCollider;

    [SerializeField] GameObject arrow;

    [Tooltip("The hand that holds the arrow")]
    [SerializeField] GameObject hand;

    [Tooltip("Controls how fast the enemy shoots")]
    [SerializeField] float shootRate;

    [Tooltip("Controls how fast the enemy turns to the target")]
    [SerializeField] int faceTargetSpeed;

    [Header("Damage Flash")]

    [Tooltip("The model used for rendering the enemy")]
    [SerializeField] Renderer model;

    [Tooltip("The color of the damage flash")]
    [SerializeField] Color flashColor;

    [Tooltip("The duration of the damage flash")]
    [Range(0.1f, 0.5f)]
    [SerializeField] float flashDuration;

    bool isShooting;
    bool playerInRange;
    
    
    Vector3 playerDirection;

    Color colorOriginal;

    void Start()
    {
        colorOriginal = model.material.color;

        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        GameManager.GetInstance().UpdateGameGoal(1);
    }

    void Update()
    {
        float agentSpeed = navAgent.velocity.normalized.magnitude;
        float animSpeed = animator.GetFloat("Speed");
        animator.SetFloat("Speed", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * animSpeedTime));

        playerDirection = GameManager.GetInstance().player.transform.position - headPos.position;
        navAgent.SetDestination(GameManager.GetInstance().player.transform.position);

        if (navAgent.remainingDistance < navAgent.stoppingDistance)
            FaceTarget();

        if (enemyType == EnemyType.Ranged && !isShooting)
            StartCoroutine(Shoot());
    }

    //////////////////////////////
    ///     ENEMY BEHAVIOR     ///   
    //////////////////////////////

    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    ///////////////////////////////
    ///         COMBAT          ///   
    ///////////////////////////////


    IEnumerator Shoot()
    {
        isShooting = true;

        animator.SetTrigger("Shoot");

        yield return new WaitForSeconds(shootRate);

        isShooting = false;
    }

    public void CreateArrow()
    {
        if (hand != null && arrow != null)
        {
            hand.SetActive(false);

            Instantiate(arrow, drawPos.position, transform.rotation);
        }
    }

    public void ReleaseArrow()
    {
        if (hand != null)
            hand.SetActive(true);
    }

    public void ArrowColliderOFF()
    {
        arrowCollider.enabled = false;
    }

    public void ArrowColliderON()
    {
        arrowCollider.enabled = true;
    }

    ///////////////////////////////
    ///         DAMAGE          ///   
    ///////////////////////////////

    public void TakeDamage(int damage)
    {
        HP -= damage;
        StartCoroutine(FlashColor());
        GameManager.GetInstance().StartCoroutine(GameManager.GetInstance().FlashHitMarker());
        
        if (HP <= 0)
        {
            GameManager.GetInstance().UpdateGameGoal(-1);

            Destroy(gameObject);           
        }
    }

    IEnumerator FlashColor()
    {
        model.material.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        model.material.color = colorOriginal;
    }
}
