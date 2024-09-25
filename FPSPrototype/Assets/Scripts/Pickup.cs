using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] GunStats gun;

    private void Start()
    {
        gun.ammoCur = gun.ammoMax;
    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            GameManager.GetInstance().playerScript.GetGunStats(gun);
            Destroy(gameObject);
        }
    }
}
