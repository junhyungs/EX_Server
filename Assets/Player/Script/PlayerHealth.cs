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
    private int PlayerHp = 4;

    public int HP
    {
        get { return PlayerHp; }
        set { PlayerHp = value; }
    }

    void Update()
    {
        SetHPBarOnUpdate(PlayerHp);
    }

    private void SetHPBarOnUpdate(int health)
    {
        if (TextMesh_HealthBar == null)
            return;

        TextMesh_HealthBar.text = new string('-', health);
    }

    //[ServerCallback]
    //private void OnTriggerEnter(Collider other)
    //{
    //    var AtkGenObject = other.GetComponent<AttackSpawnObject>();

    //    if (AtkGenObject == null)
    //    {
    //        return;
    //    }

    //    PlayerHp--;

    //    if (PlayerHp <= 0)
    //        NetworkServer.Destroy(this.gameObject);
    //}

    [ServerCallback]
    public void TakeDamage(float damage)
    {
        Debug.Log("����");
    }
}
