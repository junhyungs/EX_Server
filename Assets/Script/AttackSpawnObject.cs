using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;


public class AttackSpawnObject : NetworkBehaviour //ÃÑ¾Ë
{
    private float m_ActiveTime = 5.0f;
    private float m_ForcePower = 2000f;
    private int m_BulletDamage = 1;

    private bool m_Explosion;
    private float m_ExplosionRadius = 3.0f;
    private int m_ExplosionDamage = 2;

    public Rigidbody m_Rigid;
    public static AttackSpawnObject Instance;

    public int BulletDamage
    {
        get { return m_BulletDamage; }
        set { m_BulletDamage = value; }
    }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {   
        m_Rigid.AddForce(transform.forward * m_ForcePower);
    }

    public override void OnStartServer()
    {
        Invoke(nameof(DisableObject), m_ActiveTime);
    }

    public void SetBullet(int damage, bool Explosion )
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Zombie"))
        {
            IDamage damage = other.GetComponent<IDamage>();

            if(damage != null)
            {
                if (m_Explosion)
                {
                    damage.TakeDamage(BulletDamage);

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
