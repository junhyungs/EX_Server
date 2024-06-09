using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class Zombie : NetworkBehaviour, IDamage
{    
    public NavMeshAgent ZombieAgent;
    public Animator ZombieAnimator;
    private Transform targetTrans;
    private float ZombieHp;
    private float ZombieSpeed;
    private int ZombieAtk;
   
       

    public void SetZombie(float zombieHp, float zombieSpeed, int zombieAtk, Transform target)
    {
        ZombieHp = zombieHp;
        ZombieSpeed = zombieSpeed;
        ZombieAtk = zombieAtk;
        targetTrans = target;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameOver && gameObject.layer == LayerMask.NameToLayer("Zombie"))
        {
            ZombieMove(ZombieSpeed);
        }
    }

    
    private void ZombieMove(float moveSpeed)
    {
        if (targetTrans != null)
        {

            if (Vector3.Distance(transform.position, targetTrans.position) <= ZombieAgent.stoppingDistance)
            {
                ZombieAgent.SetDestination(transform.position);
                RpcAttack(true);
            }
            else
            {
                RpcAttack(false);

            }

            ZombieAgent.SetDestination(targetTrans.position);
            ZombieAgent.speed = moveSpeed;
        }
        else
            SetTarget();
    }

    private void SetTarget()
    {
        targetTrans = GameManager.Instance.GetRandomLocalPlayerTransform();
    }

    [ClientRpc]
    private void RpcAttack(bool isAttack)
    {
        ZombieAnimator.SetBool("Attack", isAttack);
    }


    [ServerCallback]
    public void OnTriggerEnter(Collider other)
    {
        
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && gameObject.layer == LayerMask.NameToLayer("Zombie"))
        {
            IDamage Hit = other.gameObject.GetComponent<IDamage>();

            Hit.TakeDamage(ZombieAtk);
        }
    }

    [ServerCallback]
    public void TakeDamage(int damage)
    {
        ZombieAgent.isStopped = true;

        ZombieHp -= damage;

        if(ZombieHp <= 0)
        {
            gameObject.layer = LayerMask.NameToLayer("DeadZombie");
            ZombieAgent.isStopped = true;
            ZombieAgent.ResetPath();
            Die();
        }
        else
        {
            RpcHit();
            StartCoroutine(HitAniamation());
        }
            
    }

    [ClientRpc]
    private void RpcHit()
    {
        ZombieAnimator.SetTrigger("Hit");
    }

    private IEnumerator HitAniamation()
    {
        yield return new WaitForSeconds(2.0f);
        ZombieAgent.isStopped = false;
    }

    public void Die()
    {
        RpcDie();
        StartCoroutine(DieAnimation());
    }

    [ClientRpc]
    private void RpcDie()
    {
        ZombieAnimator.SetTrigger("Die");
    }
   

    private IEnumerator DieAnimation()
    {
        GameManager.Instance.UnRegisterZombie(this);

        yield return new WaitForSeconds(3.1f);

        NetworkServer.Destroy(this.gameObject);
    }
}
