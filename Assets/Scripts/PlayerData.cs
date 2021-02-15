using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData 
{
	public string playerName;
	public string faceName;
	public int settingsSensitivity;

	public PlayerData(StaticData staticData)
	{
		playerName = staticData.getPlayerName();
		faceName = staticData.getFaceName();
		settingsSensitivity = staticData.getSensitivity();
	}

}
