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

    private int originalAmmoCur;
    private int originalAmmoMax;

    public ParticleSystem hitEffect;
    public AudioClip[] shootSound;
    public float shootVol;


    public int OriginalAmmoCur
    {
        get { return originalAmmoCur; }
    }

    public int OriginalAmmoMax
    {
        get { return originalAmmoMax; }
    }

    // Llamar esto en el inicio del juego para guardar los valores originales
    public void InitializeAmmo()
    {
        originalAmmoCur = ammoCur;
        originalAmmoMax = ammoMax;
    }

    // Restaurar los valores originales cuando sea necesario
    public void ResetAmmo()
    {
        ammoCur = originalAmmoCur;
        ammoMax = originalAmmoMax;
    }
}
