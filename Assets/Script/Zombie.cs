using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.AI;

public class Zombie : NetworkBehaviour, IDamage
{    
    NavMeshAgent ZombieAgent;
    Animator ZombieAnimator;
    private Transform targetTrans;
    private float ZombieHp;
    private float ZombieSpeed;
    private int ZombieAtk;
    private bool Attack = true;

    private void Start()
    {
        ZombieAgent = GetComponent<NavMeshAgent>();
        ZombieAnimator = GetComponent<Animator>();
    }

    public void SetZombie(float zombieHp, float zombieSpeed, int zombieAtk, Transform target)
    {
        ZombieHp = zombieHp;
        ZombieSpeed = zombieSpeed;
        ZombieAtk = zombieAtk;
        targetTrans = target;
    }

    private void Update()
    {
        if (!GameManager.Instance.IsGameOver)
        {
            ZombieMove(ZombieSpeed);
        }
    }

    
    private void ZombieMove(float moveSpeed)
    {
        SetMove(moveSpeed);
    }

    private void SetMove(float moveSpeed)
    {
        if (targetTrans != null)
        {
            ZombieAgent.speed = moveSpeed;

            ZombieAgent.SetDestination(targetTrans.position);

            if (Vector3.Distance(transform.position, targetTrans.position) <= ZombieAgent.stoppingDistance)
            {
                ZombieAgent.SetDestination(transform.position);
                //ZombieAnimator.SetTrigger("Attack");
            }
        }
        else
            Debug.Log("ÇöÀç Å¸°ÙÀÌ null");
    }

    public void OnAttack()
    {
        Attack = true;
    }

    public void OffAttack()
    {
        Attack = false;
    }

    [ServerCallback]
    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player") && Attack)
        {
            IDamage Hit = other.gameObject.GetComponent<IDamage>();

            Hit.TakeDamage(ZombieAtk);
        }
    }

    [ServerCallback]
    public void TakeDamage(int damage)
    {
        ZombieHp -= damage;

        //ZombieAnimator.SetTrigger("Hit");

        if(ZombieHp <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        //ZombieAnimator.SetTrigger("Die");
        GameManager.Instance.UnRegisterZombie(this);
        NetworkServer.Destroy(this.gameObject);
    }
}
