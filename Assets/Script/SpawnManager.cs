using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : NetworkBehaviour
{
    public Transform[] ZombieSpawnPoint;
    public GameObject[] ZombieObject;
    private Transform PlayerTrans;
    public static SpawnManager Instance;

    public float SpawnTimer = 3.0f;
    public float SpawnSpeed = 1f;
    public float SpawnCount = 20f;
    private float SaveSpawnCount;
    private float WaveCount = 4f;

    [SyncVar]
    private float EnemyHp = 1.0f;
    [SyncVar]
    private float EnemySpeed = 2.0f;
    [SyncVar]
    private int EnemyAtk = 1;

    private void Awake()
    {
        Instance = this;
    }

    public IEnumerator ZombieSpawn()
    {
        SaveSpawnCount = SpawnCount;

        while (!GameManager.Instance.IsGameOver)
        {    
            if (SpawnCount > 0)
            {
                SpawnEnemy();
                SpawnCount--;
            }
            else
                break;

            yield return new WaitForSeconds(SpawnTimer);
        }
        
        SpawnCount = SaveSpawnCount;
    }

    public void LevelUp()
    {
        Debug.Log("레벨업!");
        EnemyHp += 0.5f;
        EnemySpeed += 0.5f;
        SpawnCount += 5f;

        if(EnemyAtk < 5)
        {
            EnemyAtk++;
        }

        if (SpawnTimer < 1)
        {
            SpawnTimer = 1f;
            return;
        }

        SpawnTimer -= 0.5f;
        WaveCount = 4f;

        StartCoroutine(ReadyTime());
    }

    private IEnumerator ReadyTime()
    {
        Debug.Log("다음 레벨 준비시간");

        for(float i = WaveCount; i > 0; i--)
        {
            UiManager.Instance.NextWaveCount(i);
            yield return new WaitForSeconds(1.0f);
        }

        UiManager.Instance.NextWave_ActiveFalse();
        StartCoroutine(ZombieSpawn());
    }

    public void SpawnEnemy()
    {
        int randomZombie = Random.Range(0, ZombieObject.Length);
        int randomPoint = Random.Range(0, ZombieSpawnPoint.Length);

        GameObject zombie = Instantiate(ZombieObject[randomZombie], ZombieSpawnPoint[randomPoint]);

        PlayerTrans = GameManager.Instance.GetRandomLocalPlayerTransform();
            
        Zombie zombieOBject = zombie.GetComponent<Zombie>();

        GameManager.Instance.RegisterZombie(zombieOBject);

        zombieOBject.SetZombie(EnemyHp, EnemySpeed, EnemyAtk, PlayerTrans);

        NetworkServer.Spawn(zombie);
    }

}
