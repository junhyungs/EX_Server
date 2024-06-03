using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public partial class NetworkingAuthenticator
{
    //네트워크 인증자 클라 연관함수 추가
    [SerializeField] private LoginPopup m_LoginPopup;

    [Header("Client UserName")]
    public string m_PlayerName;

    public void OnInputValueChanged_SetPlayerName(string userName)
    {
        //네트워크 인증자(클라) UI유저명 변경 처리
        //로그인 팝업에 유저명 변경 시 네트워크 인증자에 관련 정보 수정
        m_PlayerName = userName;
        m_LoginPopup.SetUIOnAuthValueChanged();
    }
    public override void OnStartClient()
    {
        //네트워크 인증자(클라) 인증 응답 처리부 이벤트 등록과 해제
        NetworkClient.RegisterHandler<AuthResMsg>(OnAuthResponseMessage, false);
    }
    public override void OnStopClient()
    {
        //네트워크 인증자(클라) 인증 응답 처리부 이벤트 등록과 해제
        NetworkClient.UnregisterHandler<AuthResMsg>();
    }
    public override void OnClientAuthenticate()
    {
        //클라 인증 요청 시 불려지는 함수
        NetworkClient.Send(new AuthReqMsg { authUserName = m_PlayerName });
        //네트워크(클라) 인증 요청 시 패킷을 만들어서 전송
    }
    public void OnAuthResponseMessage(AuthResMsg msg)
    {
        if(msg.code == 100)//인증 성공
        {
            ClientAccept(); //성공하면ClientAccept 이벤트 발생
        }
        else//실패
        {
            NetworkManager.singleton.StopHost();
            m_LoginPopup.SetUIOnAuthError(msg.message);
        }
    }

}
