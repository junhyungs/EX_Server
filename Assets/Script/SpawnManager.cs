using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    public Transform[] ZombieSpawnPoint;
    public GameObject[] ZombieObject;
    public Transform TestTrans;

    public float SpawnTimer = 5.0f;
    public float SpawnSpeed = 1f;

    [SyncVar]
    public int GameLevel = 1;
    [SyncVar]
    private float EnemyHp = 1.0f;
    [SyncVar]
    private float EnemySpeed = 5.0f;
    [SyncVar]
    private float EnemyAtk = 1.0f;


    public override void OnStartServer()
    {
        StartCoroutine(ZombieSpawn());
    }

    private IEnumerator ZombieSpawn()
    {
        while (!GameManager.Instance.IsGameOver)
        {
            SpawnEnemy();
            yield return new WaitForSeconds(SpawnTimer);
        }
    }

    public void LevelUp(int Up)
    {

    }

    
    //private void Spawn()
    //{
    //    SpawnEnemy();
    //}

    public void SpawnEnemy()
    {
        int randomZombie = Random.Range(0, ZombieObject.Length);
        int randomPoint = Random.Range(0, ZombieSpawnPoint.Length);

        GameObject zombie = Instantiate(ZombieObject[randomZombie], ZombieSpawnPoint[randomPoint]);
        Zombie zombieOBject = zombie.GetComponent<Zombie>();
        zombieOBject.SetZombie(EnemyHp, EnemySpeed, EnemyAtk, TestTrans);

        NetworkServer.Spawn(zombie);
    }

}
