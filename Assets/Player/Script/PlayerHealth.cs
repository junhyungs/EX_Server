using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : Singleton<PlayerHealth>, IDamage
{
    [Header("HealthBar")]
    public TextMesh TextMesh_HealthBar;

    //[SyncVar]
    //�� ������ ��Ʈ��ũ�� ���� ����ȭ �Ǿ�� ���� ��Ÿ����. �� ���� ����
    //����Ǹ� ��Ʈ��ũ�� ���� Ŭ�󰣿� ����ȭ�ȴ�.
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

    [Command]
    public void UnRegisterLocalPlayer()
    {
        RpcOnDeadUI();
        StartCoroutine(UnRegisterPlayer());
    }

    private IEnumerator UnRegisterPlayer()
    {
        yield return new WaitForSeconds(0.1f);
        gameObject.SetActive(false);
    }


    [ServerCallback]
    public void TakeDamage(int damage)
    {
        PlayerHp -= damage;

        if(PlayerHp <= 0)
        {
            UnRegisterLocalPlayer();
        }
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
