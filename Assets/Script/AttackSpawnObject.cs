using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;


public class AttackSpawnObject : NetworkBehaviour
{
    private float m_ActiveTime = 5.0f;
    private float m_ForcePower = 2000f;
    private float m_BulletDamage;

    private bool m_Explosion;
    private float m_ExplosionRadius = 3.0f;
    private float m_ExplosionDamage = 2.0f;
    public Rigidbody m_Rigid;

    private void Start()
    {   
        m_Rigid.AddForce(transform.forward * m_ForcePower);
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DisableObject), m_ActiveTime);
    }

    public void SetBullet(float damage, bool Explosion )
    {
        m_BulletDamage = damage;
        m_Explosion = Explosion;
    }

    [Server]
    private void DisableObject()
    {
        NetworkServer.Destroy(this.gameObject);
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
