using Org.BouncyCastle.Asn1.Sec;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;


public class PlayerController : NetworkBehaviour
{
    [Header("Speed")]
    [SerializeField] private float WalkSpeed;
    [Header("SprintSpeed")]
    [SerializeField] private float SprintSpeed;
    [Header("ChangeSpeed")]
    [SerializeField] private float ChangeSpeed;

    [Header("HealthBar")]
    public TextMesh TextMesh_HealthBar;
    [Header("NetType")]
    public TextMesh TextMesh_NetType;

    [Header("Attack")]
    public GameObject m_prefab_AtkObject;
    public Transform m_Transform_AtkSpawnPos;

    [Header("Stats Server")]
    [SyncVar] public int m_Health = 4;
    //이 변수가 네트워크를 통해 동기화 되어야 함을 나타낸다. 이 변수 값이
    //변경되면 네트워크를 통해 클라간에 동기화된다.
    [Header("PlayerAnimator")]
    public Animator m_Animator;

    private CharacterController m_Controller;
    private PlayerInput m_Input;
    //private NetworkAnimator m_Animator;

    [SyncVar]
    private float targetSpeed;
    [SyncVar]
    private float m_Speed;

    [SyncVar]
    Vector3 lastPosition;

    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_Input = GetComponent<PlayerInput>();
    }
    
    void Update()
    {
        string netTypeStr = isClient ? "Client(O)" : "Client(X)";
        TextMesh_NetType.text = this.isLocalPlayer ? $"[로컬/{netTypeStr}]" 
            : $"[로컬아님/{netTypeStr}]{this.netId}";

        SetHPBarOnUpdate(m_Health);

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
    private void SetHPBarOnUpdate(int health)
    {
        TextMesh_HealthBar.text = new string('-', health);
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

        Vector3 inputDir = transform.TransformDirection(new Vector3(m_Input.inputValue.x, 0, m_Input.inputValue.y)).normalized;
        PlayerMove(m_Speed, inputDir);
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
    private void PlayerMove(float speed, Vector3 input)
    {
        Movement(speed, input);
    }

    [ClientRpc]
    private void Movement(float speed, Vector3 input)
    {
        m_Controller.Move(input * speed * Time.deltaTime);
    }

    //[Command]
    //private void AnimatorPlay(float speed, Vector2 input)
    //{
    //    AnimatorMove(speed, input);
    //}

    //[ClientRpc]
    //private void AnimatorMove(float speed, Vector2 input)
    //{
    //    if (input == Vector2.zero)
    //        speed = 0;
    //    if (m_Animator == null)
    //        return;

    //    m_Animator.SetFloat("MovePosX", speed * input.x);
    //    m_Animator.SetFloat("MovePosZ", speed * input.y);
    //}

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
        Debug.Log($"{this.netId}가 RPC호출함");
        //Fire 애니메이션
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        var AtkGenObject = other.GetComponent<AttackSpawnObject>();

        if (AtkGenObject == null)
            return;

        m_Health--;

        if (m_Health <= 0)
            NetworkServer.Destroy(this.gameObject);
    }

}
