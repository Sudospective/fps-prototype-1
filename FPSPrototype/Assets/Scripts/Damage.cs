using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{

    [SerializeField] enum damgaeType {bullet, stationary, melee};
    [SerializeField] damgaeType type;
    [SerializeField] Rigidbody rb;

    [SerializeField] int damageAmount;
    [SerializeField] int speed;
    [SerializeField] int destroyTime;
    // Start is called before the first frame update
    void Start()
    {
        if(type == damgaeType.bullet)
        {
            rb.velocity = transform.forward * speed;
            Destroy(gameObject, destroyTime);
        }


    }

    // Update is called once per frame

    
    void Update()
    {
        
    }
}
