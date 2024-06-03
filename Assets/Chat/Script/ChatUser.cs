using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.Rendering;

public class ChatUser : NetworkBehaviour
{
    [SyncVar] //서버 변수를 모든 클라에 자동 동기화하는데 사용된다. 클라가 직접 변경하면 안되고, 서버에서 변경해야 함
    public string m_playerName;

    //호스트 또는 서버에서만 호출되는 함수
    public override void OnStartServer()
    {
        //서버사이드 시작 시 PlayerName 해당 정보 대입
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
