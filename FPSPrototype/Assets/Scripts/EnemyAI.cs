using sfw.net;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour,
    // Interfaces
    IDamage
{
    public enum EnemyType { Melee, Ranged }

    [Header("Components")]

    [Tooltip("The type of enemy determines their behavior and attack animation")]
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

    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] GameObject arrow;
 
    [SerializeField] float shootRate;
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

        GameManager.GetInstance().UpdateGameGoal(1);
    }

    void Update()
    {
        float agentSpeed = navAgent.velocity.normalized.magnitude;
        float animSpeed = animator.GetFloat("Speed");
        animator.SetFloat("Speed", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * animSpeedTime));

        playerDirection = GameManager.GetInstance().player.transform.position - headPos.position;
        //navAgent.SetDestination(GameManager.GetInstance().player.transform.position);

        //if (navAgent.remainingDistance < navAgent.stoppingDistance)
            //FaceTarget();
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

        CreateArrow();

        yield return new WaitForSeconds(shootRate);

        isShooting= false;
    }

    public void CreateArrow()
    {
        if (arrow != null)
            Instantiate(arrow, shootPos.position, transform.rotation);
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
