using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

public class NetworkingManager : NetworkManager
{
    [SerializeField] private LoginPopup m_LoginPopup;
    [SerializeField] private ChattingUI m_ChattingUI;

    public void OnInputValueChanged_SetHostName(string hostName)
    {
        this.networkAddress = hostName;
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        if(m_ChattingUI != null)
        {
            m_ChattingUI.RemoveNameOnServerDisconnect(conn);
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnClientDisconnect()
    {
        base.OnClientDisconnect();

        if(m_LoginPopup != null)
        {
            m_LoginPopup.SetUIOnClientDisconnected();
        }
    }
}
