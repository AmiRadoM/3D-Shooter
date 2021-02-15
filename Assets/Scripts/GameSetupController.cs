using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSetupController : MonoBehaviourPunCallbacks, IPunObservable
{
    public Transform[] spawnPoints = null;

    public GameObject GameStartUI;
    public GameObject PlayerListContent;
    public GameObject PlayerListPrefab;
    
    StaticData staticData;

    public float DeathY = 0;

    List<playerGameData> publicData = new List<playerGameData>();

    playerGameData data = new playerGameData();




    void Start()
    {
        staticData = GameObject.Find("Static Data").GetComponent<StaticData>();
        data.name = staticData.getPlayerName();

    }

	private void Update()
	{
        UpdatePlayerList();
	}

	void CreatePlayer()
    {
        

        Debug.Log("Creating Player");

        if(spawnPoints == null)
        {
            
            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), Vector3.zero, Quaternion.identity);
            
        }
        else
        {

            PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity);
            
        }
        
    }

    public void Respawn(string msg)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        GameStartUI.SetActive(true);
        //PhotonNetwork.Instantiate(Path.Combine("Prefabs", "Player"), spawn, Quaternion.identity).GetComponent<PlayerController>().spawner = spawn;
    }

    public void ClearPlayerList()
	{
        foreach(GameObject item in PlayerListContent.transform)
		{
            Destroy(item);
		}
	}

    public void UpdatePlayerList()
	{
        ClearPlayerList();

		foreach (playerGameData data in publicData)
		{
            GameObject item = Instantiate(PlayerListPrefab,PlayerListContent.transform) as GameObject;

            item.transform.Find("Name").GetComponent<TMP_Text>().text = data.name;
		}
	}

    [PunRPC]
    public void StartGame()
	{
        GameStartUI.SetActive(false);
        CreatePlayer();
    }


	public override void OnDisconnected(DisconnectCause cause)
    {
        staticData.SaveData();
        SceneManager.LoadScene(0);
        base.OnDisconnected(cause);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
			
            stream.SendNext((playerGameData)data);
            Debug.Log("Hey");

        }
        else
        {
            
            this.publicData.Add((playerGameData)stream.ReceiveNext());

            Debug.Log("Bye");
        }
    }

}
