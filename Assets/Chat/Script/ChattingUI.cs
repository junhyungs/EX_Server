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

    //ä�� UI ����� �÷��̾� ������ ������ �����̳� �߰�
    //- Dictionary�� ����� �÷��̾�� �̸� ����
    //- OnStartServer() / ���� ���� ��, �ش� Dictionary �ʱ�ȭ
    // �����¸� - ����� �÷��̾�� �̸�
    private static readonly Dictionary<NetworkConnectionToClient,string> m_connectedNameDic = new Dictionary<NetworkConnectionToClient, string>();

    public override void OnStartServer()
    {
        m_connectedNameDic.Clear();
    }
    public override void OnStartClient()
    {
        
    }
}
