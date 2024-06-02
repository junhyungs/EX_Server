using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using Mirror.Examples.Chat;

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
    public void SetLocalPlayerName(string userName)
    {
        m_LocalPlayerName = userName;
    }
    public override void OnStartServer()
    {
        this.gameObject.SetActive(true);
        m_connectedNameDic.Clear();
    }

    public override void OnStartClient()
    {
        this.gameObject.SetActive(true);
        Text_ChatHistroy.text = string.Empty;
    }

    [Command(requiresAuthority = false)]//커맨드라는 어트리뷰트를 이용해 클라에서 서버로 특정 기능수행을 요청한다.
    private void CommandSendMsg(string msg, NetworkConnectionToClient sender = null)
    {
        if (!m_connectedNameDic.ContainsKey(sender))
        {
            ChatUser player = sender.identity.GetComponent<ChatUser>();
            string playerName = player.m_playerName;
            m_connectedNameDic.Add(sender, playerName);
        }

        if (!string.IsNullOrWhiteSpace(msg))
        {
            var senderName = m_connectedNameDic[sender];
            OnRecvMessge(senderName, msg.Trim());
        }
    }
    public void RemoveNameOnServerDisconnect(NetworkConnectionToClient conn)
    {
        m_connectedNameDic.Remove(conn);
    }

    [ClientRpc]//서버사이드에서 모든 클라이언트에게 특정 함수를 실행시킬 수 있도록 클라이언트RPC를 붙인다.
    private void OnRecvMessge(string senderName, string msg)
    {// 전송자와 현재 플레이어의 이름 비교 후 메세지 포매팅(색)
        string formatedMsg = (senderName == m_LocalPlayerName) ?
            $"<color = red>{senderName}:</color>{msg}" :
            $"<color = blue>{senderName}:</color>{msg}";

        AppendMessage(formatedMsg);
    }

    //UI----------------------------------------------------------
    private void AppendMessage(string msg)
    {//텍스트에 채팅 내용을 넣고 줄바꿈
        StartCoroutine(AppendAndScroll(msg));
    }

    private IEnumerator AppendAndScroll(string msg)
    {
        Text_ChatHistroy.text += msg + "\n";

        yield return null;
        yield return null;

        ScrollBar_Chat.value = 0;
    }
    //UI--------------------------------------------------------------
    public void OnClick_SendMsg()
    {
        var currentChatMsg = Input_ChatMsg.text;
        if (!string.IsNullOrWhiteSpace(currentChatMsg))
        {
            CommandSendMsg(currentChatMsg.Trim());
        }
    }
    public void OnClick_Exit()
    {
        NetworkManager.singleton.StopHost();
    }
    public void OnValueChanged_ToggleButton(string input)
    {
        Btn_Send.interactable = !string.IsNullOrWhiteSpace(input);
    }

    public void OnEndEdit_SendMsg(string input)
    {
        if(Input.GetKeyDown(KeyCode.Return)
            || Input.GetKeyDown(KeyCode.KeypadEnter)
            || Input.GetButtonDown("Submit"))
        {
            OnClick_SendMsg();
        }
    }

}
