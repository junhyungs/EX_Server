using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInput : MonoBehaviour
{
    [Header("InputVector")]
    [SerializeField] private Vector2 m_input;
    [Header("IsSprint")]
    [SerializeField] private bool m_sprint;

    public Vector2 inputSystem { get; private set; }
    public bool sprint { get; set; }
    
    private void OnMove(InputValue inputValue)
    {
        SetMove(inputValue.Get<Vector2>());
    }
    private void OnSprint(InputValue inputValue)
    {
        IsSprint(inputValue.isPressed);
    }

    private void SetMove(Vector2 input)
    {
        m_input = input;
    }

    private void IsSprint(bool input)
    {
        m_sprint = input;
    }
    
}
