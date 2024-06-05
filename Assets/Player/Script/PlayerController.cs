using Org.BouncyCastle.Asn1.Sec;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;



public class PlayerController : Singleton<PlayerController>
{
    [Header("Speed")]
    [SerializeField] private float WalkSpeed;

    [Header("SprintSpeed")]
    [SerializeField] private float SprintSpeed;

    [Header("ChangeSpeed")]
    [SerializeField] private float ChangeSpeed;

    [Header("NetType")]
    public TextMesh TextMesh_NetType;

    [Header("Attack")]
    public GameObject m_prefab_AtkObject;
    public Transform m_Transform_AtkSpawnPos;
    
    [Header("PlayerAnimator")]
    public Animator m_Animator;

    private CharacterController m_Controller;
    private PlayerInput m_Input;
    public ParticleSystem m_Prefab;

    [SyncVar]
    private float targetSpeed;
    [SyncVar]
    private float m_Speed;
    

    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_Input = GetComponent<PlayerInput>();
    }

    private void Update()
    {
        //â(���� ��)�� ���õǾ����� �ʴٸ� �Ʒ� �޼ҵ带 �������� �ʰڴ�.
        //�� ���ǹ��� �������� �׻� �����ؾ��ϴ� �޼ҵ�� â�� ����(�÷���)�߿��� �����ؾ��ϴ� �޼ҵ带 ���� �� �ִµ�.
        if (CheckIsFocusedOnUpdate() == false)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            CommandAtk();
        }

        Move();
    }
    private bool CheckIsFocusedOnUpdate()
    {
        return Application.isFocused;
    }

    private float Sprint()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            return SprintSpeed;
        }
        else
        {
            return WalkSpeed;
        }
            
    }
    
    private void Move()
    {
        if (this.isLocalPlayer == false)
            return;

        targetSpeed = Sprint();
        
        if (m_Input.inputValue == Vector2.zero)
        {
            targetSpeed = 0;
        }

        float currentHorizontalSpeed = new Vector3(m_Controller.velocity.x, 0, m_Controller.velocity.z).magnitude;
        
        float speedOffset = 0.1f;

        if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            m_Speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * ChangeSpeed);

            m_Speed = Mathf.Round(m_Speed * 1000f) / 1000f;
        }
        else
            m_Speed = targetSpeed;

        RotatePlayer();
        AnimatorPlay(m_Speed,m_Input.inputValue);
    }


    private void RotatePlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Vector3 lookRotate = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            float dis = Vector3.Distance(transform.position, hit.point);
            if(dis > 0.1f)
                transform.LookAt(lookRotate);

        }
    }

    [Command]
    private void AnimatorPlay(float speed, Vector2 input)
    {
        AnimatorMove(speed, input);
    }

    [ClientRpc]
    private void AnimatorMove(float speed, Vector2 input)
    {
        m_Animator.SetFloat("MovePosX", speed * input.x);
        m_Animator.SetFloat("MovePosZ", speed * input.y);
    }

    [Command]
    private void CommandAtk()
    {
        GameObject attackObjectForSpawn = PoolManager.Instance.GetBullet();
        attackObjectForSpawn.transform.position = m_Transform_AtkSpawnPos.position;
        attackObjectForSpawn.transform.rotation = transform.rotation;
        NetworkServer.Spawn(attackObjectForSpawn);

        RpcOnAttack();
    }

    [ClientRpc]
    private void RpcOnAttack()
    {
        m_Animator.SetTrigger("Fire");

        if (m_Prefab != null)
            m_Prefab.Play();
        
        //Fire �ִϸ��̼�
    }

}
