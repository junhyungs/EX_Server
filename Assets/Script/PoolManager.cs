using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolManager : Singleton<PoolManager>
{
    public GameObject[] Bullet;
    public GameObject[] Zombie;

    public Transform[] BulletSpawnTransform;
    public Transform[] ZombieSpawnTransform;

    private Queue<GameObject>[] BulletPool;
    private Queue<GameObject>[] ZombiePool;

    private float BulletDamage = 1f;
    private float BulletForcePower = 100f;
    private bool Explosion = false;

    public bool IsExplosion
    {
        get { return Explosion; }
        set { Explosion = value; }
    }

    public override void OnStartClient()
    {
        InitBulletPool();
    }

    public override void OnStartServer()
    {
        InitZombiePool();
    }

    private void InitZombiePool()
    {
        ZombiePool = new Queue<GameObject>[Zombie.Length];

        for (int i = 0; i < Zombie.Length; i++)
        {
            ZombiePool[i] = new Queue<GameObject>();
        }

        CreateZombie();
    }
    private void InitBulletPool()
    {
        BulletPool = new Queue<GameObject>[Bullet.Length];

        for (int i = 0; i < Bullet.Length; i++)
        {
            BulletPool[i] = new Queue<GameObject>();
        }

        CreateBullet();
    }
   
    private void CreateBullet()
    {
        for (int i = 0; i < 200; i++)
        {
            GameObject bulletPrefab = Instantiate(Bullet[0], BulletSpawnTransform[0]);
            bulletPrefab.SetActive(false);
            BulletPool[0].Enqueue(bulletPrefab);
        }
    }

    [Server]
    private void CreateZombie()
    {
        for (int i = 0; i < 50; i++)
        {
            GameObject zombiePrefab = Instantiate(Zombie[0], ZombieSpawnTransform[0]);
            zombiePrefab.SetActive(false);
            ZombiePool[0].Enqueue(zombiePrefab);
        }

        for (int i = 0; i < 50; i++)
        {
            GameObject zombiePrefab = Instantiate(Zombie[1], ZombieSpawnTransform[1]);
            zombiePrefab.SetActive(false);
            ZombiePool[1].Enqueue(zombiePrefab);
        }

        for (int i = 0; i < 50; i++)
        {
            GameObject zombiePrefab = Instantiate(Zombie[2], ZombieSpawnTransform[2]);
            zombiePrefab.SetActive(false);
            ZombiePool[2].Enqueue(zombiePrefab);
        }

        for (int i = 0; i < 50; i++)
        {
            GameObject zombiePrefab = Instantiate(Zombie[3], ZombieSpawnTransform[3]);
            zombiePrefab.SetActive(false);
            ZombiePool[3].Enqueue(zombiePrefab);
        }
    }
      
    public GameObject GetBullet()
    {
        GameObject bullet = BulletPool[0].Dequeue();
        BulletPool[0].Enqueue(bullet);
        InitBullet(bullet);
        bullet.SetActive(true);        
        return bullet;  
    }

    [Server]
    public GameObject GetZombie()
    {
        int PoolNumber = Random.Range(0, 4);

        GameObject zombie = ZombiePool[PoolNumber].Dequeue();   
        //여기도 풀넘버 보내주는 메소드 작성해야함
        zombie.SetActive(true);
        NetworkServer.Spawn(zombie);
        return zombie; 
    }

   
    //public void RetuenBullet(GameObject bullet)
    //{
    //    bullet.SetActive(false);
    //    NetworkServer.UnSpawn(bullet);
    //    BulletPool[0].Enqueue(bullet);
    //}

    //public void ReturnZombie(GameObject zombie)
    //{
    //    int ZombieNumber = zombie.; 나중에 좀비 스크립트안에 풀 넘버 먹이는 메소드 추가할 예정;

    //    zombie.SetActive(false);
    //    NetworkServer.UnSpawn(zombie);
    //    ZombiePool[ZombieNumber].Enqueue(zombie);
    //}

    private void InitBullet(GameObject bullet)
    {
        Rigidbody bulletRd = bullet.GetComponent<Rigidbody>();
        bulletRd.velocity = Vector3.zero;
        bullet.transform.rotation = Quaternion.identity;    
        AttackSpawnObject AtkObject = bulletRd.GetComponent<AttackSpawnObject>();
        AtkObject.SetBullet(BulletDamage, BulletForcePower, Explosion);
    }

    

    private void InitZombie(GameObject zombie)
    {

    }

    
}
