using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemSO : ScriptableObject
{
    public string m_ItemName;
    public int m_AmountHP;
    public bool m_Explosion;
    public ItemStat m_ItemStat = new ItemStat();

    public void UseItem()
    {
        if(m_ItemStat == ItemStat.Health)
        {
            if (PlayerHealth.Instance.HP == 4)
                return;
            else
            {
                PlayerHealth.Instance.HP += m_AmountHP;
            }
        }
        else if(m_ItemStat == ItemStat.Explosion)
        {
            AttackSpawnObject.Instance.StartCoroutine(IsExplosion());
        }
    }

    private IEnumerator IsExplosion()
    {
        AttackSpawnObject.Instance.SetBullet(2f, true);
        yield return new WaitForSeconds(5.0f);
        AttackSpawnObject.Instance.SetBullet(1f, false);
    }


    public enum ItemStat
    {
        none,
        Health,
        Explosion
    };
}
