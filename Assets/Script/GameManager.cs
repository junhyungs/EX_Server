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
                Debug.Log("게임오버");
                GameOver = true;
            }
        }
    }

    public Transform GetRandomLocalPlayerTransform()
    {
        if (LocalPlayer_Transform.Count == 0)
            return null;
        //랜덤한 수 반환
        int randomTrans = Random.Range(0, LocalPlayer_Transform.Count);
        Debug.Log(randomTrans);
        foreach(var localPlayerTrans in LocalPlayer_Transform.Values)
        {//딕셔너리에 있는 플레이어 Trans 선택
            if (randomTrans == 0)
            {
                return localPlayerTrans;
            }

            randomTrans--; //randomTrans가 0이 되었을 때 해당 Transform 반환.
        }//randomTrans가 2라면 딕셔너리의 1번째 값을 가져오도록 함.

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
