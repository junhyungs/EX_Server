using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Rendering;

public partial class NetworkingAuthenticator : NetworkAuthenticator//인증자 구현을 위해 NetworkAuthenticator 상속
{
    //네트워크 인증자(서버)인증, 연결여부 관리 컨테이너 추가
    readonly HashSet<NetworkConnection> m_connectionsPendingDisconnect = new HashSet<NetworkConnection>();
    internal static readonly HashSet<string> m_playerNames = new HashSet<string>();
    public struct AuthReqMsg : NetworkMessage//네트워크 인증자 인증 전용 네트워크 메세지 추가
    {
        //인증을 위해 사용
        public string authUserName;
    }
    public struct AuthResMsg : NetworkMessage//네트워크 인증자 인증 전용 네트워크 메세지 추가
    {
        public byte code;
        public string message;
    }
    #region ServerSide
    [UnityEngine.RuntimeInitializeOnLoadMethod]//네트워크 인증자 서버사이드 함수들
    static void ResetStatics()
    {

    }
    public override void OnStartServer()//네트워크 인증자 클라 인증 요청 처리용 이벤트 등록
    {
        //클라로부터 인증 요청 처리를 위한 핸들러 연결
        NetworkServer.RegisterHandler<AuthReqMsg>(OnAuthRequestMessage, false);
    }
    public override void OnStopServer()//네트워크 인증자 클라 인증 요청 처리용 이벤트 등록
    {
        NetworkServer.UnregisterHandler<AuthResMsg>();
    }
    public override void OnServerAuthenticate(NetworkConnectionToClient conn)
    {
        
    }
    public void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthReqMsg msg)
    {
        Debug.Log(msg.authUserName);
        //클라 인증 요청 메세지 도착 시 처리
        if(m_connectionsPendingDisconnect.Contains(conn))
        {
            return;
        }

        //웹서버, DB, playerprefabAPI등을 호출하여 인증 확인
        if(!m_playerNames.Contains(msg.authUserName))
        {
            //네트워크 인증자(서버)인증 완료된 정보 응답 패킷 작성
            m_playerNames.Add(msg.authUserName);
            conn.authenticationData = msg.authUserName;

            AuthResMsg authResMsg = new AuthResMsg
            {
                code = 100,
                message = "Auth Success"
            };
            //네트워크 인증자(서버) 응답 패킷 전송 및 인증 완료 이벤트 Invoke
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
            //네트워크 인증자(서버) 미인증 처리부
            conn.Send(authResMsg);
            conn.isAuthenticated = false;
            //네트워크 인증자(서버) 미인증 처리부 - 미인증 처리된 클라는 접속 해제 시킴
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
