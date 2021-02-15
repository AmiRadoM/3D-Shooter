using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using UnityEngine;



public class MainMenuController : MonoBehaviourPunCallbacks
{
    
    public GameObject quickStartButton;
    public GameObject quickConnectingButton;
    public GameObject CreateRoomButton;
    public GameObject CreatingRoomButton;
    public GameObject RoomListButton;
    public GameObject SettingsButton;
    public GameObject CustomButton;
    public GameObject MainCan;
    public GameObject RoomListCan;
    public GameObject SettingsCan;
    public GameObject CustomCan;
    public GameObject RoomButton;
    public GameObject ServerSelectObject;
    public GameObject customizeContent;
    public Slider sensSlider;
    public TMP_Text sensText;
    

    public int RoomSize = 8;

    public TMP_Dropdown serverRegion;
    public TMP_Text ConnectingtoServer;
    public TMP_Text PingText;
    public TMP_InputField playerName;


    bool selectedServer;

    StaticData staticData;

    List<RoomInfo> roomList;


    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        staticData = GameObject.Find("Static Data").GetComponent<StaticData>();
        playerName.text = staticData.getPlayerName();
        sensSlider.value = staticData.getSensitivity();
        staticData.SaveData();


        PhotonNetwork.Disconnect();
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("connected to master [" + PhotonNetwork.CloudRegion + "] server with ping ["+PhotonNetwork.GetPing()+"]");
        ServerSelectObject.SetActive(true);
        ConnectingtoServer.enabled = false;
        PhotonNetwork.JoinLobby();
        
        

        base.OnConnectedToMaster();
    }


    public override void OnJoinedLobby()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        quickStartButton.SetActive(true);
        CreateRoomButton.SetActive(true);
        RoomListButton.SetActive(true);
        SettingsButton.SetActive(true);
        CustomButton.SetActive(true);
        
        
        base.OnJoinedLobby();
    }

    public void Disconnect()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        quickStartButton.SetActive(false);
        CreateRoomButton.SetActive(false);
        RoomListButton.SetActive(false);
        SettingsButton.SetActive(false);
        CustomButton.SetActive(false);
    }

    public void QuickStart()
    {
        staticData.SaveData();
        quickStartButton.SetActive(false);
        quickConnectingButton.SetActive(true);
        PhotonNetwork.JoinRandomRoom();
        Debug.Log("Joined Room [" + PhotonNetwork.CurrentRoom + "]");
    }

    public void CreateRoomButn()
    {
        Debug.Log("Creating Room Right Now");
        staticData.SaveData();
        CreateRoomButton.SetActive(false);
        CreatingRoomButton.SetActive(true);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)RoomSize };
        PhotonNetwork.CreateRoom(playerName.text+"'s Room", roomOps);
        Debug.Log("Created and Joined a Room Called ["+playerName.text+"'s Room]");
    }

    public void DeSelectAllFaces()
	{
        List<GameObject> faces = new List<GameObject>();
        for(int i = 0; i < customizeContent.transform.childCount; i++)
		{
			if (customizeContent.transform.GetChild(i).gameObject.GetComponent<Button>()!=null)
			{
                customizeContent.transform.GetChild(i).gameObject.GetComponent<Button>();

            }
		}
	}

    void HideAllCan()
    {
        MainCan.SetActive(false);
        RoomListCan.SetActive(false);
        SettingsCan.SetActive(false);
        CustomCan.SetActive(false);
    }

    public void ShowRoomList()
    {
        HideAllCan();
        RoomListCan.SetActive(true);
    }

    public void ShowMain()
    {
        HideAllCan();
        MainCan.SetActive(true);
    }

    public void ShowSettings()
    {
        HideAllCan();
        SettingsCan.SetActive(true);
    }

    public void ShowCustom()
    {
        HideAllCan();
        CustomCan.SetActive(true);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to Join a Random Room");
        CreateRoom();
    }

    void CreateRoom()
    {
        Debug.Log("Creating Room Right Now");
        int randomRoomNumber = Random.Range(0, 10000);
        RoomOptions roomOps = new RoomOptions() { IsVisible = true, IsOpen = true, MaxPlayers = (byte)RoomSize };
        PhotonNetwork.CreateRoom("Room" + randomRoomNumber, roomOps);
        Debug.Log("Created and Joined a Room Called [Room"+randomRoomNumber+"]");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed to Create a Room... Trying Again");
        CreateRoom();
    }

    public void ClearRoomList()
    {
        Transform content = RoomListCan.transform.Find("Scroll View/Viewport/Content");
        foreach (Transform a in content) Destroy(a.gameObject);
    }

    public override void OnRoomListUpdate(List<RoomInfo> p_List)
    {
        roomList = p_List;
        ClearRoomList();

        Transform content = RoomListCan.transform.Find("Scroll View/Viewport/Content");

        foreach(RoomInfo a in roomList)
        {
            GameObject newRoomButton = Instantiate(RoomButton, content) as GameObject;

            newRoomButton.transform.Find("Name").GetComponent<TMP_Text>().text = a.Name;
            newRoomButton.transform.Find("Players").GetComponent<TMP_Text>().text = a.PlayerCount + "/" + a.MaxPlayers;

            newRoomButton.GetComponent<Button>().onClick.AddListener(delegate { JoinRoom(newRoomButton.transform); });
        }

        base.OnRoomListUpdate(roomList);
    }

    public void JoinRoom(Transform pButton)
    {
        string RoomName = pButton.transform.Find("Name").GetComponent<TMP_Text>().text;
        staticData.SaveData();
        PhotonNetwork.JoinRoom(RoomName);
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Failed Joining Room");

    }

    public void ExitGame()
    {
        staticData.SaveData();
        Application.Quit();
    }

    public void ServerSelect()
    {
        if (PhotonNetwork.CloudRegion != null)
        {
            Disconnect();
            PhotonNetwork.Disconnect();
            
        }

        if (serverRegion.options[serverRegion.value].text == "USA")
        {
            PhotonNetwork.ConnectToRegion("us");
        }
        else if (serverRegion.options[serverRegion.value].text == "Asia")
            {
            PhotonNetwork.ConnectToRegion("asia");
        }
        else if (serverRegion.options[serverRegion.value].text == "Europe")
        {
            PhotonNetwork.ConnectToRegion("eu");
        }

        if (!selectedServer)
        {
            MainCan.SetActive(true);
            selectedServer = true;
        }
    }


    private void Update()
    {
        PingText.text = "Ping: "+PhotonNetwork.GetPing().ToString();
        staticData.setSensitivity((int)sensSlider.value);
        sensText.text = staticData.getSensitivity().ToString();
        staticData.setPlayerName(playerName.text);
		if (CustomCan.activeSelf)
		{
            customizeContent.transform.Find(staticData.getFaceName()).GetComponent<Button>().Select();
            customizeContent.transform.Find(staticData.getFaceName()).GetComponent<Button>().OnSelect(null);
        }

    }

}
