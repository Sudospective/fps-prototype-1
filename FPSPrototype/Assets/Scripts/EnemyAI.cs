using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;
    [SerializeField] int faceTargetSpeed;

    [SerializeField] int hitpoints;

    Color colorOriginal;
    Color colorHit;

    bool isShooting;

    Vector3 playerDirection;
    bool playerInRange;

    [SerializeField] GameObject bullet;
    [SerializeField] int attackDamage;
    [SerializeField] float shootRate;

    void Start()
    {
        // This is causing errors because we
        // are using a prefab instead of a
        // game object. Comment out for now. ~Ami

        colorHit = Color.red;
        colorOriginal = model.material.color;
        GameManager.GetInstance().updateGameGoal(1);
    }

    void Update()
    {
        playerDirection = GameManager.GetInstance().player.transform.position - headPos.position;
        navAgent.SetDestination(GameManager.GetInstance().player.transform.position);

        if (navAgent.remainingDistance < navAgent.stoppingDistance)
            FaceTarget();

        if (!isShooting)
        {
            StartCoroutine(Shoot());
        }
    }
    void FaceTarget()
    {
        Quaternion rot = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * faceTargetSpeed);
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        GameObject b = Instantiate(bullet, shootPos.position, transform.rotation);
        // Get the damage component and multiply by the attack damage of the enemy
        Damage dmg = b.GetComponent<Damage>();
        if (dmg != null)
        {
            dmg.damageAmount = attackDamage;
        }
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int damage)
    {
        hitpoints -= damage;
        StartCoroutine(FlashColor());
        GameManager.GetInstance().StartCoroutine(GameManager.GetInstance().FlashHitMarker());
        
        if (hitpoints <= 0)
        {
            Destroy(gameObject);
            GameManager.GetInstance().updateGameGoal(-1);
            
        }
    }

    IEnumerator FlashColor()
    {
        // This is causing errors because we
        // are using a prefab instead of a
        // game object. Comment out for now. ~Ami

        model.material.color = colorHit;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }
}
