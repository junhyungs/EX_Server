using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public partial class NetworkingAuthenticator
{
    //��Ʈ��ũ ������ Ŭ�� �����Լ� �߰�
    [SerializeField] private LoginPopup m_LoginPopup;

    [Header("Client UserName")]
    public string m_PlayerName;

    public void OnInputValueChanged_SetPlayerName(string userName)
    {
        //��Ʈ��ũ ������(Ŭ��) UI������ ���� ó��
        //�α��� �˾��� ������ ���� �� ��Ʈ��ũ �����ڿ� ���� ���� ����
        m_PlayerName = userName;
        m_LoginPopup.SetUIOnAuthValueChanged();
    }
    public override void OnStartClient()
    {
        //��Ʈ��ũ ������(Ŭ��) ���� ���� ó���� �̺�Ʈ ��ϰ� ����
        NetworkClient.RegisterHandler<AuthResMsg>(OnAuthResponseMessage, false);
    }
    public override void OnStopClient()
    {
        //��Ʈ��ũ ������(Ŭ��) ���� ���� ó���� �̺�Ʈ ��ϰ� ����
        NetworkClient.UnregisterHandler<AuthResMsg>();
    }
    public override void OnClientAuthenticate()
    {
        //Ŭ�� ���� ��û �� �ҷ����� �Լ�
        NetworkClient.Send(new AuthReqMsg { authUserName = m_PlayerName });
        //��Ʈ��ũ(Ŭ��) ���� ��û �� ��Ŷ�� ���� ����
    }
    public void OnAuthResponseMessage(AuthResMsg msg)
    {
        if(msg.code == 100)//���� ����
        {
            ClientAccept(); //�����ϸ�ClientAccept �̺�Ʈ �߻�
        }
        else//����
        {
            NetworkManager.singleton.StopHost();
            m_LoginPopup.SetUIOnAuthError(msg.message);
        }
    }

}
