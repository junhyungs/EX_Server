using Org.BouncyCastle.Asn1.Sec;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Cinemachine;
using TMPro;



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
    public ParticleSystem m_BulletShells;

    [Header("ItemSO")]
    public ItemSO[] m_itemSo;

    [Header("PlayerCam")]
    public GameObject VirtualCameraPrefab;


    private CharacterController m_Controller;
    private PlayerInput m_Input;
    

    [SyncVar]
    private float targetSpeed;
    [SyncVar]
    private float m_Speed;
    [SyncVar]
    private float m_MaxBulletCount = 20;
    [SyncVar]//(hook = nameof(BulletText)) 해당 변수에 변화가 생길 때마다 메소드 실행
    private float m_BulletCount = 0;
    [SyncVar]
    private bool OnFire = true;


    [Command]
    public void Reload()
    {
        m_BulletCount = 20;
        OnFire = true;
        BulletText(m_BulletCount, m_MaxBulletCount);
    }
    

    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_Input = GetComponent<PlayerInput>();

        m_BulletCount = m_MaxBulletCount;
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        if (isLocalPlayer)
        {
            GameObject Cam = Instantiate(VirtualCameraPrefab);

            Cam.transform.rotation = Quaternion.Euler(50f, 90f, 0);

            CinemachineVirtualCamera virtualcam = Cam.GetComponent<CinemachineVirtualCamera>();

            virtualcam.Follow = transform;

            var doNothing = virtualcam.GetCinemachineComponent<CinemachineComposer>();

            if(doNothing != null)
            {
                Destroy(doNothing);
            }

            var transPoser = virtualcam.GetCinemachineComponent<CinemachineTransposer>();
            if(transPoser != null)
            {
                transPoser.m_BindingMode = CinemachineTransposer.BindingMode.WorldSpace;
                transPoser.m_FollowOffset = new Vector3(-8f, 10f, 0);
            }

            RegisterLocalPlayer(transform);
        }
    }

    [Command]
    private void RegisterLocalPlayer(Transform transform)
    {
        GameManager.Instance.RegisterPlayerTransform(connectionToClient, transform);
    }

    private void Update()
    {
        //창(게임 씬)이 선택되어있지 않다면 아래 메소드를 실행하지 않겠다.
        //이 조건문을 기준으로 항상 실행해야하는 메소드와 창이 선택(플레이)중에만 실행해야하는 메소드를 나눌 수 있는듯.
        if (CheckIsFocusedOnUpdate() == false)
        {
            return;
        }

        if (Input.GetMouseButtonDown(0) && OnFire)
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

        if (Physics.Raycast(ray, out RaycastHit hit, 100))
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

            BulletText(m_BulletCount, m_MaxBulletCount);

            if (m_BulletCount == 0)
            {
                OnFire = false;
            }

            GameObject bullet = Instantiate(m_prefab_AtkObject, m_Transform_AtkSpawnPos.position, m_Transform_AtkSpawnPos.rotation);
            NetworkServer.Spawn(bullet);
            RpcOnAttack();
        }
    }

    [ClientRpc]
    private void BulletText(float currentBullet, float maxBullet)
    {
        if (isLocalPlayer)
        {
            UiManager.Instance.UpdateBulletText(currentBullet, maxBullet);
        }
    }

    [ClientRpc]
    private void RpcOnAttack()
    {
        m_Animator.SetTrigger("Fire");

        if (m_Prefab != null)
        {
            m_Prefab.Play();
            m_BulletShells.Play();
        }
            
        
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
