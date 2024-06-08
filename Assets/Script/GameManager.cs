using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;



public class GameManager : NetworkManager
{
    public static GameManager Instance;
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

    public void RegisterPlayerTransform(NetworkConnection conn, Transform playerTrans)
    {
        if (!LocalPlayer_Transform.ContainsKey(conn))
        {
            LocalPlayer_Transform.Add(conn, playerTrans);
        }
    }
    
    public void UnRegisterPlayer(NetworkConnection conn)
    {
        if (LocalPlayer_Transform.ContainsKey(conn))
        {
            LocalPlayer_Transform.Remove(conn);

            if(LocalPlayer_Transform.Count == 0)
            {
                Debug.Log("���ӿ���");
                GameOver = true;
            }
        }
    }

    public Transform GetRandomLocalPlayerTransform()
    {
        if (LocalPlayer_Transform.Count == 0)
            return null;
        //������ �� ��ȯ
        int randomTrans = Random.Range(0, LocalPlayer_Transform.Count);
        Debug.Log(randomTrans);
        foreach(var localPlayerTrans in LocalPlayer_Transform.Values)
        {//��ųʸ��� �ִ� �÷��̾� Trans ����
            if (randomTrans == 0)
            {
                return localPlayerTrans;
            }

            randomTrans--; //randomTrans�� 0�� �Ǿ��� �� �ش� Transform ��ȯ.
        }//randomTrans�� 2��� ��ųʸ��� 1��° ���� ���������� ��.

        return null;
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
                SpawnManager.Instance.LevelUp();
            }
        }
    }


}
