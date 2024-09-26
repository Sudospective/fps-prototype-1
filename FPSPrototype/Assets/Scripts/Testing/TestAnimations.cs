using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NewBehaviourScript : MonoBehaviour
{
    public enum EnemyType { Tank, Agile, Ranged }

    [SerializeField] EnemyType enemyType;

    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] int animSpeedTime;

    [SerializeField] int attackRate;
    [SerializeField] int throwingDist;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] GameObject spear;
    [SerializeField] Transform throwPos;

    [SerializeField] Transform bowPos;
    [SerializeField] Transform headPos;

    [SerializeField] Transform shootPos;
    [SerializeField] GameObject arrow;
    
    bool isAttacking;
    bool isRanged;

    float stoppingDistanceOrig;

    Vector3 targetDirection;

    void Start()
    {
        stoppingDistanceOrig = agent.stoppingDistance;
    }

    // Update is called once per frame
    void Update()
    {
        float agentSpeed = agent.velocity.normalized.magnitude;
        float animSpeed = animator.GetFloat("Speed");
        animator.SetFloat("Speed", Mathf.Lerp(animSpeed, agentSpeed, Time.deltaTime * animSpeedTime));
        
        if (enemyType == EnemyType.Ranged)
        {
            targetDirection = TestingManager.Instance.target.transform.position - bowPos.position;
        }
        else
        {
            targetDirection = TestingManager.Instance.target.transform.position - headPos.position;
        }

        agent.SetDestination(TestingManager.Instance.target.transform.position);
        
        Debug.Log("Remaining Distance: " + agent.remainingDistance.ToString());

        if (agent.remainingDistance < agent.stoppingDistance)
            FaceTarget();

        if (enemyType == EnemyType.Tank || enemyType == EnemyType.Agile)
        {
            if (enemyType == EnemyType.Tank && agent.remainingDistance >= throwingDist
                && !isAttacking)
            {
                StartCoroutine(ThrowSpear());
            }
            else if (agent.remainingDistance != 0 && agent.remainingDistance <= agent.stoppingDistance && !isAttacking)
            {
                agent.stoppingDistance = stoppingDistanceOrig;
                StartCoroutine(Attack());
            }
        }
        else if (enemyType == EnemyType.Ranged && agent.remainingDistance <= agent.stoppingDistance && !isAttacking)
            StartCoroutine(Shoot());
    }
    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(targetDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    IEnumerator Attack()
    {
        isAttacking = true;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackRate);

        isAttacking = false;
    }

    IEnumerator ThrowSpear()
    {
        isAttacking = true;

        animator.SetTrigger("Throw");

        yield return new WaitForSeconds(attackRate);

        isAttacking = false;
    }

    public void ReleaseSpear()
    {
        if (spear != null)
        {
            Instantiate(spear, throwPos.position, throwPos.rotation);
        }
    }

    IEnumerator Shoot()
    {
        isAttacking = true;

        animator.SetTrigger("Shoot");

        yield return new WaitForSeconds(attackRate);

        isAttacking = false;
    }

    public void CreateArrow()
    {
        Instantiate(arrow, shootPos.position, shootPos.rotation);
    }
}
