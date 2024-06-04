using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AttackSpawnObject : NetworkBehaviour
{
    public float m_DestroyAfter = 5.0f;
    public float m_Force = 1000f;
    public Rigidbody m_Rigid;

    public override void OnStartServer()
    {
        Invoke(nameof(DestroySelf), m_DestroyAfter);
    }

    private void Start()
    {
        m_Rigid.useGravity = false;
        m_Rigid.AddForce(transform.forward * m_Force);
    }

    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(this.gameObject);
    }

    [ServerCallback]

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Zombie"))
            DestroySelf();
    }


}
