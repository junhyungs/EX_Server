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
    //�� ������ ��Ʈ��ũ�� ���� ����ȭ �Ǿ�� ���� ��Ÿ����. �� ���� ����
    //����Ǹ� ��Ʈ��ũ�� ���� Ŭ�󰣿� ����ȭ�ȴ�.
    [Header("PlayerAnimator")]
    public Animator m_Animator;


    private CharacterController m_Controller;
    private PlayerInput m_Input;
    
    private float targetSpeed;
    private float m_Speed;

    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_Input = GetComponent<PlayerInput>();
    }
    
    void Update()
    {
        string netTypeStr = isClient ? "Client(O)" : "Client(X)";
        TextMesh_NetType.text = this.isLocalPlayer ? $"[����/{netTypeStr}]" 
            : $"[���þƴ�/{netTypeStr}]{this.netId}";


        if (Input.GetMouseButtonDown(0))
        {
            CommandAtk();
        }

        Move();
    }

    private void Move()
    {
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

        Vector3 inputDirection = transform.TransformDirection(new Vector3(m_Input.inputValue.x, 0, m_Input.inputValue.y).normalized);
        RotatePlayer();
        AnimatorPlay(m_Speed, m_Input.inputValue);
        
        m_Controller.Move(inputDirection * m_Speed * Time.deltaTime);
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

    private void AnimatorPlay(float speed, Vector2 input)
    {
        if (input == Vector2.zero)
            speed = 0;
        if (m_Animator == null)
            return;

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
        Debug.Log($"{this.netId}�� RPCȣ����");
        //Fire �ִϸ��̼�
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