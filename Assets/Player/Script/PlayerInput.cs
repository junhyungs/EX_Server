using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;


public class PlayerInput : NetworkBehaviour
{
    [Header("InputVector")]
    [SerializeField]
    [SyncVar]
    private Vector2 m_input;

    public Vector2 inputValue
    {
        get { return m_input; }
    }

    private void OnMove(InputValue inputValue)
    {
        SetMove(inputValue.Get<Vector2>());
    }
    private void SetMove(Vector2 input)
    {
        m_input = input;
    }
}
