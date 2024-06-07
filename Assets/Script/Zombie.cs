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
    private float ZombieAtk;

    private void Start()
    {
        ZombieAgent = GetComponent<NavMeshAgent>();
        ZombieAnimator = GetComponent<Animator>();
    }

    public void SetZombie(float zombieHp, float zombieSpeed, float zombieAtk, Transform target)
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

    [Command]
    private void ZombieMove(float moveSpeed)
    {
        SetMove(moveSpeed);
    }

    [ClientRpc]
    private void SetMove(float moveSpeed)
    {
        if(targetTrans != null)
        {
            if(ZombieAgent.remainingDistance <= ZombieAgent.stoppingDistance)
            {
                ZombieAgent.SetDestination(transform.position);
                ZombieAnimator.SetTrigger("Attack");
                return;
            }

            ZombieAgent.speed = moveSpeed;
            ZombieAgent.SetDestination(targetTrans.position);
        }
    }

    [ServerCallback]
    public void TakeDamage(float damage)
    {
        Debug.Log("ÃÑ¿¡ ¸ÂÀ½");
    }
}
