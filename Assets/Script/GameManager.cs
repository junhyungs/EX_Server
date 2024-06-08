using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkManager
{
    public static GameManager Instance;

    private bool GameOver = false;
    private int Score = 0;

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

}
