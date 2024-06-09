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

    //�÷��̾� Ʈ������ ����ȭ(�÷��̾ ���ڱ� ������ ���ų�, ���� �߿� �����Ͽ� �߻��ϴ� ����ȭ ������ �����ϱ� ���� �ڵ�)
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
                    Debug.Log("���ӿ���");
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
            //������ �� ��ȯ
            int randomTrans = Random.Range(0, LocalPlayer_Transform.Count);

            foreach (var localPlayerTrans in LocalPlayer_Transform.Values)
            {//��ųʸ��� �ִ� �÷��̾� Trans ����
                if (randomTrans == 0)
                {
                    Debug.Log("���������� Transform ��ȯ");
                    return localPlayerTrans;
                }

                randomTrans--; //randomTrans�� 0�� �Ǿ��� �� �ش� Transform ��ȯ.
            }//randomTrans�� 2��� ��ųʸ��� 1��° ���� ���������� ��.

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
