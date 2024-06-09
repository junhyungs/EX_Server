using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;



public class GameManager : NetworkManager
{
    public static GameManager Instance;

    public Transform RespawnPosition;

    private Dictionary<NetworkConnection, Transform>LocalPlayer_Transform = new Dictionary<NetworkConnection, Transform>();
    private Dictionary<NetworkIdentity, Zombie> ZombieDic = new Dictionary<NetworkIdentity, Zombie>();

    private bool GameOver = false;
    private int Score = 0;

    public int GameLevel = 1;
    
    public bool IsGameOver
    {
        get { return GameOver; }
        set { GameOver = value; }
    }
    public int GameScore
    {
        get { return Score; }
        set { Score = value; }
    }

    public override void Awake()
    {
        base.Awake();

        Instance = this;
    }

    //플레이어 트랜스폼 동기화(플레이어가 갑자기 접속을 끊거나, 스폰 중에 접속하여 발생하는 동기화 문제를 방지하기 위한 코드)
    private readonly object playerTransformLock = new object();

    public override void OnStartServer()
    {
        StartCoroutine(WaitForPlayer());
    }

    private IEnumerator WaitForPlayer()
    {
        while (LocalPlayer_Transform.Count == 0)
        {
            yield return new WaitForSeconds(1.0f);
        }
        
        StartCoroutine(SpawnManager.Instance.ZombieSpawn());
    }

    public void RegisterPlayerTransform(NetworkConnection conn, Transform playerTrans)
    {
        lock(playerTransformLock)
        {
            if (!LocalPlayer_Transform.ContainsKey(conn))
            {
                LocalPlayer_Transform.Add(conn, playerTrans);
            }

            UiManager.Instance.GameLevelUI_ActiveTrue();
            UiManager.Instance.CurrentBulletUI_ActiveTrue();
        }
    }
    
    public void UnRegisterPlayer(NetworkConnection conn)
    {
        lock(playerTransformLock)
        {
            if (LocalPlayer_Transform.ContainsKey(conn))
            {
                LocalPlayer_Transform.Remove(conn);

                if (LocalPlayer_Transform.Count == 0)
                {
                    Debug.Log("게임오버");
                    GameOver = true;
                }
            }
        }
    }

    public Transform GetRandomLocalPlayerTransform()
    {
        lock (playerTransformLock)
        {
            if (LocalPlayer_Transform.Count == 0)
            {
                return null;
            }
            //랜덤한 수 반환
            int randomTrans = Random.Range(0, LocalPlayer_Transform.Count);

            foreach (var localPlayerTrans in LocalPlayer_Transform.Values)
            {//딕셔너리에 있는 플레이어 Trans 선택
                if (randomTrans == 0)
                {
                    Debug.Log("정상적으로 Transform 반환");
                    return localPlayerTrans;
                }

                randomTrans--; //randomTrans가 0이 되었을 때 해당 Transform 반환.
            }//randomTrans가 2라면 딕셔너리의 1번째 값을 가져오도록 함.

            return null;
        }
        
    }

    public void RegisterZombie(Zombie zombie)
    {
        if (!ZombieDic.ContainsKey(zombie.netIdentity))
        {
            ZombieDic.Add(zombie.netIdentity, zombie);
        }
    }

    public void UnRegisterZombie(Zombie zombie)
    {
        if (ZombieDic.ContainsKey(zombie.netIdentity))
        {
            ZombieDic.Remove(zombie.netIdentity);

            if(ZombieDic.Count == 0)
            {
                GameLevel++;
                GameLevelUI(GameLevel);
                SpawnManager.Instance.LevelUp();
            }
        }
    }

    private void GameLevelUI(int gameLevel)
    {
        UiManager.Instance.RpcLevelText(gameLevel);
    }

    
    public void RespawnPlayer(NetworkConnection conn, GameObject player)
    {
        if(player != null)
        {
            player.transform.position = RespawnPosition.position;

            PlayerHealth Hp = player.GetComponent<PlayerHealth>();

            Hp.HP = 10;

            player.SetActive(true);
        }
    }
}
