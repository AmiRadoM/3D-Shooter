using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticData : MonoBehaviour
{
    
    static public string playerName;
    static public string faceName = "yinon";
    static public int sensitivity = 50;


    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        LoadData();
    }



    public void setPlayerName(string name)
    {
        playerName = name;
    }

    public string getPlayerName()
    {
        return playerName;
    }

    public void setFaceName(string name)
    {
        faceName = name;
    }

    public string getFaceName()
    {
        return faceName;
    }

    public void setSensitivity(int sens)
    {
        sensitivity = sens;
    }

    public int getSensitivity()
    {
        return sensitivity;
    }

    public void SaveData()
    {
        SaveSystem.SaveData(this);
    }

    public void LoadData()
    {
        PlayerData data = SaveSystem.LoadData();

        playerName = data.playerName;
        faceName = data.faceName;
        sensitivity = data.settingsSensitivity;
    }
}
