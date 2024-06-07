using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class PlayerCamera : NetworkBehaviour
{
    public Transform target;
    public Vector3 Camera_Offset;

    private Transform StartTransform;

    public override void OnStartClient()
    {
        StartTransform = target;
    }

    void Update()
    {
        transform.position = StartTransform.position + Camera_Offset;   
    }
}
