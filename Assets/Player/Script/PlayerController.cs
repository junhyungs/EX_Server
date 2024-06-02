using Org.BouncyCastle.Asn1.Sec;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float WalkSpeed;
    [Header("SprintSpeed")]
    [SerializeField] private float SprintSpeed;
    [Header("ChangeSpeed")]
    [SerializeField] private float ChangeSpeed;

    private CharacterController m_Controller;
    private PlayerInput m_Input;
    private Animator m_Animator;
    private float m_Speed;

    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_Input = GetComponent<PlayerInput>();
        m_Animator = GetComponent<Animator>();  
    }
    
    void Update()
    {
        
        Move();
    }

    private void Move()
    {
        float targetSpeed = m_Input.sprint ? SprintSpeed : WalkSpeed;
        
        if (m_Input.inputValue == Vector2.zero)
        {
            targetSpeed = 0;
        }

        float currentHorizontalSpeed = new Vector3(m_Controller.velocity.x, 0, m_Controller.velocity.z).magnitude;
        
        float speedOffset = 0.1f;

        if(currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            m_Speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, Time.deltaTime * ChangeSpeed);
            m_Speed = Mathf.Round(m_Speed * 1000f)/1000f;
        }
        else
        {
            m_Speed = targetSpeed;
        }
        
        RotatePlayer();

        Vector3 inputDirection = transform.TransformDirection(new Vector3(m_Input.inputValue.x, 0, m_Input.inputValue.y).normalized);
        m_Controller.Move(inputDirection * m_Speed * Time.deltaTime);

    }

    private void RotatePlayer()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if(Physics.Raycast(ray, out RaycastHit hit, 100))
        {
            Vector3 lookRotate = new Vector3(hit.point.x, transform.position.y, hit.point.z);

            transform.LookAt(lookRotate);
        }
    }
}
