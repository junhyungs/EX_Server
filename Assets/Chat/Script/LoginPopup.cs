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

    //[SerializeField] NetworkingManager m_NetManager;

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
        
    }
    private void OnDisable()
    {
        
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
    
}
