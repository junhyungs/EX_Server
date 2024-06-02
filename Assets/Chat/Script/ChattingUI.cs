using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Runtime.InteropServices;

public class ChattingUI : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private Text Text_ChatHistroy;
    [SerializeField] private Scrollbar ScrollBar_Chat;
    [SerializeField] private InputField Input_ChatMsg;
    [SerializeField] private Button Btn_Send;

    private static string m_LocalPlayerName;

    //채팅 UI 연결된 플레이어 정보를 관리할 컨테이너 추가
    //- Dictionary로 연결된 플레이어들 이름 관리
    //- OnStartServer() / 서버 시작 시, 해당 Dictionary 초기화
    // 서버온리 - 연결된 플레이어들 이름
    private static readonly Dictionary<NetworkConnectionToClient,string> m_connectedNameDic = new Dictionary<NetworkConnectionToClient, string>();

    public override void OnStartServer()
    {
        m_connectedNameDic.Clear();
    }
    public override void OnStartClient()
    {
        
    }
}
