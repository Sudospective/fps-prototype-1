using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{

    [SerializeField] enum DamageType {projectile, stationary, melee};
    [SerializeField] DamageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] public int damageAmount;
    [SerializeField] public int speed;
    [SerializeField] public int destroyTime;

    // Start is called before the first frame update
    void Start()
    {
        if (type == DamageType.projectile)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Activates damage on collision, destroys bullet after collision
        IDamage damage = other.GetComponent<IDamage>();

        if (damage != null)
        {
            damage.TakeDamage(damageAmount);
        }

        if (type == DamageType.projectile)
        {
            Destroy(gameObject);
        }
    }
}
