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

    void Start()
    {
        BulletPool = new Queue<GameObject>[Bullet.Length];
        ZombiePool = new Queue<GameObject>[Zombie.Length];
        InitPool();
    }

    private void InitPool()
    {
        for(int i = 0; i < Bullet.Length; i++)
        {
            BulletPool[i] = new Queue<GameObject>();
        }

        for(int i = 0; i < Zombie.Length; i++)
        {
            ZombiePool[i] = new Queue<GameObject>();
        }

        for(int i = 0; i < 200; i++)
        {
            GameObject bulletPrefab = Instantiate(Bullet[0], BulletSpawnTransform[0]);
            bulletPrefab.SetActive(false);
            BulletPool[0].Enqueue(bulletPrefab);
        }

        for(int i = 0; i < 50; i++)
        {
            GameObject zombiePrefab = Instantiate(Zombie[0], ZombieSpawnTransform[0]);
            zombiePrefab.SetActive(false);
            ZombiePool[0].Enqueue(zombiePrefab);
        }

        for(int i = 0; i < 50; i++)
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
        bullet.SetActive(true);
        return bullet;  
    }

    public GameObject GetZombie()
    {
        int PoolNumber = Random.Range(0, 4);

        GameObject zombie = ZombiePool[PoolNumber].Dequeue();
        ZombiePool[PoolNumber].Enqueue(zombie);
        zombie.SetActive(true);
        return zombie; 
    }
}
