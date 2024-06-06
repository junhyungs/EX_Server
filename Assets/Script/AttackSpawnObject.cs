using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class AttackSpawnObject : NetworkBehaviour
{
    private float m_ActiveTime = 5.0f;
    private float m_ForcePower;
    private float m_BulletDamage;

    private bool m_Explosion;
    private float m_ExplosionRadius = 3.0f;
    private float m_ExplosionDamage = 2.0f;
    private Rigidbody m_Rigid;

    private void Start()
    {
        m_Rigid = GetComponent<Rigidbody>();
        m_Rigid.useGravity = false;
    }

    private void OnEnable()
    {
        if(isServer)
            Invoke(nameof(ReturnObject), m_ActiveTime);

    }

    public void SetBullet(float damage, float forcePower, bool Explosion )
    {
        m_ForcePower = forcePower;
        m_BulletDamage = damage;
        m_Explosion = Explosion;
    }

    private void FixedUpdate()
    {
        OnBulletMoveUpdate(m_ForcePower);
    }

    
    private void OnBulletMoveUpdate(float force)
    {
        BulletMove(force);
    }

    
    private void BulletMove(float force)
    {
        Vector3 Move = transform.forward * force;
        m_Rigid.AddForce(Move);
    }

    [Command]
    private void ReturnObject()
    {
        DisableObject();
    }

    
    private void DisableObject()
    {
        NetworkServer.UnSpawn(this.gameObject);
        this.gameObject.SetActive(false);
        //PoolManager.Instance.RetuenBullet(this.gameObject);
    }


    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            IDamage damage = other.GetComponent<IDamage>();

            if(damage != null)
            {
                if (m_Explosion)
                {
                    Collider[] hitColl = Physics.OverlapSphere(other.transform.position, m_ExplosionRadius, LayerMask.GetMask("Zombie"));

                    for(int i = 0; i < hitColl.Length; i++)
                    {
                        IDamage hit = hitColl[i].GetComponent<IDamage>();
                        hit.TakeDamage(m_ExplosionDamage);
                    }

                    DisableObject();
                }
                else
                {
                    damage.TakeDamage(m_BulletDamage);
                    DisableObject();
                }
            }
        }
    }


}
