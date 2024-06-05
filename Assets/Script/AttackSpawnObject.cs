using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class AttackSpawnObject : NetworkBehaviour
{
    private float m_ActiveTime = 5.0f;
    private float m_ForcePower = 100f;
    private float m_BulletDamage = 1.0f;
    private Rigidbody m_Rigid;

    private void Start()
    {
        m_Rigid = GetComponent<Rigidbody>();
        m_Rigid.useGravity = false;
    }

    private void OnEnable()
    {
        Invoke(nameof(ReturnObject), m_ActiveTime);
    }

    public void InitBullet(float damage, float forcePower)
    {
        m_ForcePower = forcePower;
        m_BulletDamage = damage;
    }

    private void FixedUpdate()
    {
        if (isServer)
        {
            OnBulletMoveUpdate(m_ForcePower);
        }
    }

    [Server]
    private void OnBulletMoveUpdate(float force)
    {
        BulletMove(force);
    }

    [ClientRpc]
    private void BulletMove(float force)
    {
        Vector3 Move = transform.forward * force;
        m_Rigid.AddForce(Move);
    }


    [Server]
    private void ReturnObject()
    {
        DisableObject();
    }

    [ClientRpc]
    private void DisableObject()
    {
        this.gameObject.SetActive(false);
    }

    //[ServerCallback]
    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.gameObject.layer == LayerMask.NameToLayer("Zombie"))
    //        DestroySelf();
    //}


}
