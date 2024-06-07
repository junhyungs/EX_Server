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

    [Header("NetType")]
    public TextMesh TextMesh_NetType;

    [Header("Attack")]
    public GameObject m_prefab_AtkObject;
    public Transform m_Transform_AtkSpawnPos;
    
    [Header("PlayerAnimator")]
    public Animator m_Animator;

    [Header("Particle")]
    public ParticleSystem m_Prefab;

    [Header("ItemSO")]
    public ItemSO[] m_itemSo;

    private CharacterController m_Controller;
    private PlayerInput m_Input;
    

    [SyncVar]
    private float targetSpeed;
    [SyncVar]
    private float m_Speed;
    [SyncVar]
    private float m_BulletCount = 20;
    [SyncVar]
    private bool OnFire = true;

    public void Fire()
    {
        m_BulletCount = 20;
        OnFire = true;
    }
    

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

        if (Input.GetMouseButtonDown(0))
        {
            CommandAtk();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CommandReloading();
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
    private void CommandReloading()
    {
        ReloadAnimation();
    }

    [ClientRpc]
    private void ReloadAnimation()
    {
        m_Animator.SetTrigger("Reloading");
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
        if (OnFire)
        {
            m_BulletCount--;

            if(m_BulletCount == 0)
            {
                OnFire = false;
            }
            
            GameObject bullet = Instantiate(m_prefab_AtkObject, m_Transform_AtkSpawnPos.position, m_Transform_AtkSpawnPos.rotation);
            AttackSpawnObject bulletobj = bullet.GetComponent<AttackSpawnObject>();
            bulletobj.SetBullet(1f, false);
            NetworkServer.Spawn(bullet);
            RpcOnAttack();
        }
    }

    [ClientRpc]
    private void RpcOnAttack()
    {
        m_Animator.SetTrigger("Fire");

        if (m_Prefab != null)
            m_Prefab.Play();
        
        //Fire 애니메이션
    }

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Item"))
        {
            Item item = other.gameObject.GetComponent<Item>();

            for(int i = 0; i < m_itemSo.Length; i++)
            {
                if (m_itemSo[i].name == item.m_ItemName)
                {
                    m_itemSo[i].UseItem();
                }
            }
        }
    }

}
