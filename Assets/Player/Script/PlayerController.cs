using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float Speed;


    private CharacterController m_Controller;
    private PlayerInput m_Input;
    private float m_Speed;

    void Start()
    {
        m_Controller = GetComponent<CharacterController>();
        m_Input = GetComponent<PlayerInput>();
    }
    
    void Update()
    {
        
    }

    private void Move()
    {

    }
}
