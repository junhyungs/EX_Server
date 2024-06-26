using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Singleton<PlayerHealth>, IDamage
{
    [Header("HealthBar")]
    public TextMesh TextMesh_HealthBar;

    //[SyncVar]
    //이 변수가 네트워크를 통해 동기화 되어야 함을 나타낸다. 이 변수 값이
    //변경되면 네트워크를 통해 클라간에 동기화된다.
    [Header("PlayerHealth")]
    [SerializeField]
    [SyncVar]
    private int PlayerHp = 10;

    public int HP
    {
        get { return PlayerHp; }
        set { PlayerHp = value; }
    }

    void Update()
    {
        if(PlayerHp >= 0)
        {
            SetHPBarOnUpdate(PlayerHp);
        }
    }

    private void SetHPBarOnUpdate(int health)
    {
        if (TextMesh_HealthBar == null)
            return;

        TextMesh_HealthBar.text = new string('-', health);
    }


    public void UnRegisterLocalPlayer()
    {
        //StartCoroutine(UnRegisterPlayer());
        EnablePlayer();
    }

    [Command]
    private void EnablePlayer()
    {
        gameObject.SetActive(false);
    }

    private IEnumerator UnRegisterPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        
    }


    [ServerCallback]
    public void TakeDamage(int damage)
    {
        PlayerHp -= damage;

        if(PlayerHp <= 0)
        {
            UnRegisterLocalPlayer();
            TestDeadUI();
        }
    }

    private void TestDeadUI()
    {
        RpcOnDeadUI();
    }

    [ClientRpc]
    private void RpcOnDeadUI()
    {
        if (isLocalPlayer)
        {
            UiManager.Instance.DeadUI();
            UiManager.Instance.SetPlayerIdentity(netIdentity, gameObject);
        }
    }
}
