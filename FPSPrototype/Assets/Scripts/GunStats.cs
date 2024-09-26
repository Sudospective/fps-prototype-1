using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GunStats : ScriptableObject
{
    public GameObject gunModel;
    public int shootDamage;
    public float shootRate;
    public int shootDist;
    public int ammoCur;
    public int ammoMax;


    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootVol;

    internal void InitializeAmmo()
    {
        throw new NotImplementedException();
    }
}
