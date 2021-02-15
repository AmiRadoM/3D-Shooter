using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem
{

	public static void SaveData(StaticData staticData)
	{
		Debug.LogWarning("Saving Data!");
		BinaryFormatter formatter = new BinaryFormatter();
		string path = Application.persistentDataPath + "/data.amir";
		FileStream stream = new FileStream(path, FileMode.Create);

		PlayerData data = new PlayerData(staticData);

		formatter.Serialize(stream, data);
		stream.Close();
		Debug.Log("Data Saved");
	}


	public static PlayerData LoadData()
	{
		string path = Application.persistentDataPath + "/data.amir";
		if (File.Exists(path))
		{
			BinaryFormatter formatter = new BinaryFormatter();
			FileStream stream = new FileStream(path, FileMode.Open);

			PlayerData data = formatter.Deserialize(stream) as PlayerData;
			stream.Close();

			return data;
		}
		else
		{
			Debug.LogError("Save file not found in " + path);
			return null;
		}
	}

}
