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
        //창(게임 씬)이 선택되어있지 않다면 아래 메소드를 실행하지 않겠다.
        //이 조건문을 기준으로 항상 실행해야하는 메소드와 창이 선택(플레이)중에만 실행해야하는 메소드를 나눌 수 있는듯.
        if (CheckIsFocusedOnUpdate() == false)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            CommandAtk();
        }

        Move();
    }
    private bool CheckIsFocusedOnUpdate()
    {
        return Application.isFocused;
    }
    
    private void Move()
    {
        if (this.isLocalPlayer == false)
            return;

        targetSpeed = m_Input.sprint ? SprintSpeed : WalkSpeed;
        
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
        //Vector3 inputDir = transform.TransformDirection(new Vector3(m_Input.inputValue.x, 0, m_Input.inputValue.y)).normalized;
        //PlayerMove(m_Speed, inputDir);
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

    //[Command]
    //private void PlayerMove(float speed, Vector3 input)
    //{
    //    Movement(speed, input);
    //}

    //[ClientRpc]
    //private void Movement(float speed, Vector3 input)
    //{
    //    m_Controller.Move(input * speed * Time.deltaTime);
    //}

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
        GameObject attackObjectForSpawn = Instantiate(m_prefab_AtkObject, m_Transform_AtkSpawnPos.position, Quaternion.identity);
        attackObjectForSpawn.transform.rotation = transform.rotation;
        NetworkServer.Spawn(attackObjectForSpawn);

        RpcOnAttack();
    }

    [ClientRpc]
    private void RpcOnAttack()
    {
        m_Animator.SetTrigger("Fire");
        ParticleManager.Instance.OnEable_GunShot();
        //Fire 애니메이션
    }

}
