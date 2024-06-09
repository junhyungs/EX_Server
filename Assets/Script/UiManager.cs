using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;


public class UiManager : NetworkBehaviour
{
    public static UiManager Instance;

    [Header("NextWaveUI")]
    [SerializeField] Text Nextwave_Text;

    [Header("LocalPlayerUI")]
    [SerializeField] Text LocalPlayer_Text;

    [Header("GameLevelUI")]
    [SerializeField] Text GameLevel_Text;

    [Header("DeadObject")]
    [SerializeField] GameObject DeadObject;

    [Header("RespawnButton")]
    [SerializeField] Button Respawn;

    [Header("Exit")]
    [SerializeField] Button Exit;

    [Header("GameOver")]
    [SerializeField] Text GameOver_Text;

    private NetworkIdentity playerIdentity;
    private GameObject player;
    

    private void Awake()
    {
        Instance = this;
    }

    //DeadUI----------------------------------------------------------------------------
    public void DeadUI()
    {
        DeadObject.SetActive(true);
    }

    public void HideDeadUI()
    {
        DeadObject.SetActive(false);
    }

    public void OnRespawnButton()
    {
        HideDeadUI();
        GameManager.Instance.RespawnPlayer(playerIdentity.connectionToClient, player);
    }

    public void OnExitButton()
    {
        NetworkServer.Destroy(player);
        Application.Quit();
    }


    public void SetPlayerIdentity(NetworkIdentity playerIdentity, GameObject player)
    {
        this.playerIdentity = playerIdentity;
        this.player = player;
    }

    //----------------------PlayerText---------------------------------------------------


    public void UpdateBulletText(float bulletCount, float maxBulletCount)
    {
        LocalPlayer_Text.text = bulletCount + " / " + maxBulletCount;
    }


    public void CurrentBulletUI_ActiveTrue()
    {
        if (LocalPlayer_Text == null)
        {
            return;
        }

        BulletText(true);
    }

    public void CurrentBulletUI_ActiveFalse()
    {
        BulletText(false);
    }

    private void BulletText(bool active)
    {
        RpcBulletText(active);
    }

    [ClientRpc]
    private void RpcBulletText(bool active)
    {
        BulletText_Active(active);
    }

    private void BulletText_Active(bool active)
    {
        LocalPlayer_Text.gameObject.SetActive(active);
    }


    //----------------------GameLevelUI---------------------------------------------------


    [ClientRpc]
    public void RpcLevelText(int level)
    {
        ChangeLevelText(level);
    }

    private void ChangeLevelText(int level)
    {
        GameLevel_Text.text = "Level " + level;
    }

    public void GameLevelUI_ActiveTrue()
    {
        if (GameLevel_Text == null)
        {
            return;
        }

        LevelUI(true);
    }

    public void GameLevelUI_ActiveFalse()
    {
        LevelUI(false);
    }

    private void LevelUI(bool active)
    {
        RpcLevelUI(active);
    }

    [ClientRpc]
    private void RpcLevelUI(bool active)
    {
        LevelUI_Active(active);
    }

    private void LevelUI_Active(bool active)
    {
        GameLevel_Text.gameObject.SetActive(active);
    }
    //----------------------GameLevelUI---------------------------------------------------
    //----------------------NextUI---------------------------------------------------
    public void NextWaveCount(float count)
    {
        if (Nextwave_Text == null)
        {
            return;
        }

        NextUI(true, count);        
    }

    public void NextWave_ActiveFalse()
    {
        NextUI(false, 0f);
    }

    private void NextUI(bool active, float count)
    {
        RpcNextUI(active, count);
    }

    [ClientRpc]
    private void RpcNextUI(bool active, float count)
    {
        NextWave_Active(active, count);
    }

    private void NextWave_Active(bool active , float count)
    {
        Nextwave_Text.gameObject.SetActive(active);
        Nextwave_Text.text = "Next Wave : "+ count;
    }

    //----------------------NextUI---------------------------------------------------

}
