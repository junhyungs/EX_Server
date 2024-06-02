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

    //ä�� UI ����� �÷��̾� ������ ������ �����̳� �߰�
    //- Dictionary�� ����� �÷��̾�� �̸� ����
    //- OnStartServer() / ���� ���� ��, �ش� Dictionary �ʱ�ȭ
    // �����¸� - ����� �÷��̾�� �̸�
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

    [Command(requiresAuthority = false)]//Ŀ�ǵ��� ��Ʈ����Ʈ�� �̿��� Ŭ�󿡼� ������ Ư�� ��ɼ����� ��û�Ѵ�.
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

    [ClientRpc]//�������̵忡�� ��� Ŭ���̾�Ʈ���� Ư�� �Լ��� �����ų �� �ֵ��� Ŭ���̾�ƮRPC�� ���δ�.
    private void OnRecvMessge(string senderName, string msg)
    {// �����ڿ� ���� �÷��̾��� �̸� �� �� �޼��� ������(��)
        string formatedMsg = (senderName == m_LocalPlayerName) ?
            $"<color = red>{senderName}:</color>{msg}" :
            $"<color = blue>{senderName}:</color>{msg}";

        AppendMessage(formatedMsg);
    }

    //UI----------------------------------------------------------
    private void AppendMessage(string msg)
    {//�ؽ�Ʈ�� ä�� ������ �ְ� �ٹٲ�
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
