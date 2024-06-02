using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class LoginPopup : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] InputField Input_NetworkAdress;
    [SerializeField] InputField Input_User_Name;

    [SerializeField] Button Btn_StartAsHostServer;
    [SerializeField] Button Btn_StartAsClient;

    [SerializeField] Text Text_Error;

    [SerializeField] NetworkingManager m_NetManager;

    public static LoginPopup Instance { get; private set; }
    private string m_OriginNetworkAddress;

    private void Awake()
    {
        Instance = this;
        Text_Error.gameObject.SetActive(false);
    }

    private void Start()
    {
        SetDefaultNetworkAddress();
    }
    private void OnEnable()
    {
        Input_User_Name.onValueChanged.AddListener(OnValueChanged_ToggleButton);
    }
    private void OnDisable()
    {
        Input_User_Name.onValueChanged.RemoveListener(OnValueChanged_ToggleButton); 
    }
    private void Update()
    {
        CheckNetworkAddressValidOnUpdate();
    }

    private void SetDefaultNetworkAddress()
    {
        //��Ʈ��ũ �ּҰ� ���� ��� ����Ʈ ����
        if (string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = "localhost";
        }
        //��Ʈ��ũ �ּ� �������� ����� ��츦 ����� ���� ��Ʈ��ũ �ּ� ����
        m_OriginNetworkAddress = NetworkManager.singleton.networkAddress;

    }
    public void SetUIOnAuthValueChanged()
    {//��Ʈ��ũ ������(Ŭ��) UI������ ���� ó��
        //�α��� �˾��� ������ ���� �� ��Ʈ��ũ �����ڿ� ���� ���� ����
        Text_Error.text = string.Empty;
        Text_Error.gameObject.SetActive(false);
    }

    public void SetUIOnAuthError(string msg)
    {
        //��Ʈ��ũ ������(Ŭ��) ���� ���� �� ó��
        Text_Error.text = msg;
        Text_Error.gameObject.SetActive(true); 
    }

    private void CheckNetworkAddressValidOnUpdate()
    {
        if (string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = m_OriginNetworkAddress;
        }

        if(Input_NetworkAdress.text != NetworkManager.singleton.networkAddress)
        {
            Input_NetworkAdress.text = NetworkManager.singleton.networkAddress;
        }
    }

    public void SetUIOnClientDisconnected()
    {
        this.gameObject.SetActive(true);
        Input_User_Name.text = string.Empty;
        Input_User_Name.ActivateInputField();
    }

    public void OnValueChanged_ToggleButton(string userName)
    {
        bool isUserNameValid = !string.IsNullOrWhiteSpace(userName);
        Btn_StartAsHostServer.interactable = isUserNameValid;
        Btn_StartAsClient.interactable = isUserNameValid;
    }

    public void OnClick_StartAsHost()
    {
        if (m_NetManager == null)
            return;

        m_NetManager.StartHost();
        this.gameObject.SetActive(false);
    }

    public void OnClick_StartAsClient()
    {
        if (m_NetManager == null)
            return;

        m_NetManager.StartClient();
        this.gameObject.SetActive(false);
    }
    
}
