using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : Singleton<ParticleManager>
{
    public GameObject GunShotParticle;
    public GameObject BulletShellsParticle;

    public GameObject GunShotPoint;
    public GameObject BulletShellsPoint;

    private ParticleSystem BulletShellsSystem;


    void Start()
    {
        InitParticle();
    }

    private void InitParticle()
    {
        GameObject gunshotParticle = Instantiate(GunShotParticle);
        gunshotParticle.transform.SetParent(GunShotPoint.transform);
        GunShotPoint.SetActive(false);
        
        GameObject bulletshellsParticle = Instantiate(BulletShellsParticle);
        bulletshellsParticle.transform.SetParent(BulletShellsPoint.transform);
        BulletShellsSystem = bulletshellsParticle.GetComponent<ParticleSystem>();
        BulletShellsPoint.SetActive(false);
    }

    public void OnEable_GunShot()
    {
        if (GunShotParticle == null)
            return;

        GunShotPoint.SetActive(true);
    }

    public void OnEable_BulletShells()
    {
        if (BulletShellsParticle == null)
            return;

        var main = BulletShellsSystem.main;
        main.loop = true;
        BulletShellsPoint.SetActive (true);
    }

    public void OnDisable_BulletShells()
    {
        var main = BulletShellsSystem.main;
        main.loop = false;
        BulletShellsPoint.SetActive (false);
    }

    public void OnDisable_GunShot()
    {
        GunShotPoint.SetActive(false);
    }

    
}
