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
        //네트워크 주소가 없는 경우 디폴트 세팅
        if (string.IsNullOrWhiteSpace(NetworkManager.singleton.networkAddress))
        {
            NetworkManager.singleton.networkAddress = "localhost";
        }
        //네트워크 주소 공란으로 변경될 경우를 대비해 기존 네트워크 주소 보관
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

    public void OnValueChanged_ToggleButton(string userName)
    {
        bool isUserNameValid = !string.IsNullOrWhiteSpace(userName);
        Btn_StartAsHostServer.interactable = isUserNameValid;
        Btn_StartAsClient.interactable = isUserNameValid;
    }
    
}
