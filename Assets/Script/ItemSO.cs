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
            PoolManager.Instance.StartCoroutine(IsExplosion());
        }
    }

    private IEnumerator IsExplosion()
    {
        PoolManager.Instance.IsExplosion = true;
        yield return new WaitForSeconds(5.0f);
        PoolManager.Instance.IsExplosion = false;
    }


    public enum ItemStat
    {
        none,
        Health,
        Explosion
    };
}
