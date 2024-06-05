using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour, IDamage
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

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        var AtkGenObject = other.GetComponent<AttackSpawnObject>();

        if (AtkGenObject == null)
        {
            return;
        }

        PlayerHp--;

        if (PlayerHp <= 0)
            NetworkServer.Destroy(this.gameObject);
    }

    public void TakeDamage(float damage)
    {
        
    }
}
