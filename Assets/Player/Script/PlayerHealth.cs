using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : NetworkBehaviour, IDamage
{
    [Header("HealthBar")]
    public TextMesh TextMesh_HealthBar;

    //[SyncVar]
    //이 변수가 네트워크를 통해 동기화 되어야 함을 나타낸다. 이 변수 값이
    //변경되면 네트워크를 통해 클라간에 동기화된다.
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
