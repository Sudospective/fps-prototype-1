using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{

    [SerializeField] enum damageType {bullet, stationary, melee};
    [SerializeField] damageType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;

    // Start is called before the first frame update
    void Start()
    {
        if(type == damageType.bullet)
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

        if (type == damageType.bullet)
        {
            Destroy(gameObject);
        }
    }
}
