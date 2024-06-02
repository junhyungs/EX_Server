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
        NetworkClient.RegisterHandler<AuthResiveMsg>(OnAuthResponseMessage, false);
    }
    public override void OnStopClient()
    {
        //��Ʈ��ũ ������(Ŭ��) ���� ���� ó���� �̺�Ʈ ��ϰ� ����
        NetworkClient.UnregisterHandler<AuthResiveMsg>();
    }
    public override void OnClientAuthenticate()
    {
        //Ŭ�� ���� ��û �� �ҷ����� �Լ�
        NetworkClient.Send(new AuthRequestMsg { authUserName = m_PlayerName });
        //��Ʈ��ũ(Ŭ��) ���� ��û �� ��Ŷ�� ���� ����
    }
    public void OnAuthResponseMessage(AuthResiveMsg msg)
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
