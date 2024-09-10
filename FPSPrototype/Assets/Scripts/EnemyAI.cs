using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent navAgent;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform headPos;

    [SerializeField] int hitpoints;

    Color colorOriginal;
    Color colorHit;

    bool isShooting;

    [SerializeField] GameObject bullet;
    [SerializeField] float shootRate;

    void Start()
    {
        colorOriginal = model.material.color;
        gameManager.instance.updateGameGoal(1);
    }

    void Update()
    {
        navAgent.SetDestination(gameManager.instance.player.transform.position);

        if(!isShooting)
        {
            StartCoroutine(Shoot());
        }
    }

    IEnumerator Shoot()
    {
        isShooting = true;
        Instantiate(bullet, shootPos.position, transform.rotation);
        yield return new WaitForSeconds(shootRate);
        isShooting = false;
    }

    public void TakeDamage(int damage)
    {
        hitpoints -= damage;
        StartCoroutine(FlashColor());
        
        if (hitpoints <= 0)
        {
            gameManager.instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator FlashColor()
    {
        model.material.color = colorHit;
        yield return new WaitForSeconds(0.1f);
        model.material.color = colorOriginal;
    }
}
