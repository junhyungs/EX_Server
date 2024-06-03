using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Rendering;

public class ChatUser : NetworkBehaviour
{
    [SyncVar] //���� ������ ��� Ŭ�� �ڵ� ����ȭ�ϴµ� ���ȴ�. Ŭ�� ���� �����ϸ� �ȵǰ�, �������� �����ؾ� ��
    public string m_playerName;

    //ȣ��Ʈ �Ǵ� ���������� ȣ��Ǵ� �Լ�
    public override void OnStartServer()
    {
        //�������̵� ���� �� PlayerName �ش� ���� ����
        m_playerName = (string)connectionToClient.authenticationData;
    }
    public override void OnStartLocalPlayer()
    {
        var objChatUI = GameObject.Find("ChanttingUI");

        if(objChatUI != null)
        {
            var chattingUI = objChatUI.GetComponent<ChattingUI>();

            if(chattingUI != null)
            {
                chattingUI.SetLocalPlayerName(m_playerName);
            }
        }
    }
}
