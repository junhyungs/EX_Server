using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Rendering;

public partial class NetworkingAuthenticator : NetworkAuthenticator//������ ������ ���� NetworkAuthenticator ���
{
    //��Ʈ��ũ ������(����)����, ���Ῡ�� ���� �����̳� �߰�
    readonly HashSet<NetworkConnection> m_connectionsPendingDisconnect = new HashSet<NetworkConnection>();
    internal static readonly HashSet<string> m_playerNames = new HashSet<string>();
    public struct AuthReqMsg : NetworkMessage//��Ʈ��ũ ������ ���� ���� ��Ʈ��ũ �޼��� �߰�
    {
        //������ ���� ���
        public string authUserName;
    }
    public struct AuthResMsg : NetworkMessage//��Ʈ��ũ ������ ���� ���� ��Ʈ��ũ �޼��� �߰�
    {
        public byte code;
        public string message;
    }
    #region ServerSide
    [UnityEngine.RuntimeInitializeOnLoadMethod]//��Ʈ��ũ ������ �������̵� �Լ���
    static void ResetStatics()
    {

    }
    public override void OnStartServer()//��Ʈ��ũ ������ Ŭ�� ���� ��û ó���� �̺�Ʈ ���
    {
        //Ŭ��κ��� ���� ��û ó���� ���� �ڵ鷯 ����
        NetworkServer.RegisterHandler<AuthReqMsg>(OnAuthRequestMessage, false);
    }
    public override void OnStopServer()//��Ʈ��ũ ������ Ŭ�� ���� ��û ó���� �̺�Ʈ ���
    {
        NetworkServer.UnregisterHandler<AuthResMsg>();
    }
    public override void OnServerAuthenticate(NetworkConnectionToClient conn)
    {
        
    }
    public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthReqMsg msg)
    {
        Debug.Log(msg.authUserName);
        //Ŭ�� ���� ��û �޼��� ���� �� ó��
        if(m_connectionsPendingDisconnect.Contains(conn))
        {
            return;
        }

        //������, DB, playerprefabAPI���� ȣ���Ͽ� ���� Ȯ��
        if(!m_playerNames.Contains(msg.authUserName))
        {
            //��Ʈ��ũ ������(����)���� �Ϸ�� ���� ���� ��Ŷ �ۼ�
            m_playerNames.Add(msg.authUserName);
            conn.authenticationData = msg.authUserName;

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 100,
                message = "Auth Success"
            };
            //��Ʈ��ũ ������(����) ���� ��Ŷ ���� �� ���� �Ϸ� �̺�Ʈ Invoke
            conn.Send(authResMsg);
            ServerAccept(conn);
        }
        else
        {
            m_connectionsPendingDisconnect.Add(conn);

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 200,
                message = "User Name already in use! Try again!"
            };
            //��Ʈ��ũ ������(����) ������ ó����
            conn.Send(authResMsg);
            conn.isAuthenticated = false;
            //��Ʈ��ũ ������(����) ������ ó���� - ������ ó���� Ŭ��� ���� ���� ��Ŵ
            StartCoroutine(DelayedDisconnect(conn, 1.0f));
        }
    }
    private IEnumerator DelayedDisconnect(NetworkConnectionToClient conn, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        ServerReject(conn);

        yield return null;
        m_connectionsPendingDisconnect.Remove(conn);
    }
    #endregion
}
