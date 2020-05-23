using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
public class MenuNewWorld : MonoBehaviour
{
	public TMP_InputField worldName, seed;
	public MainMenu mainMenu;
	public void Create()
	{
		string newWorldName = worldName.text;
		string newWorldSeed = seed.text;
		if (string.IsNullOrEmpty(newWorldName)) newWorldName = "My World";
		DirectoryInfo worldFolder = new DirectoryInfo(Application.persistentDataPath + "/Worlds");
		int id;
		while (true)
		{
			id = new System.Random().Next();
			DirectoryInfo saveFolder = new DirectoryInfo(worldFolder.FullName + "/" + id.ToString());
			if (saveFolder.Exists) continue;
			break;
		}
		if (string.IsNullOrEmpty(newWorldSeed)) newWorldSeed = id.ToString();

		int generatedSeed;
		bool canConvert = int.TryParse(newWorldSeed, out generatedSeed);
		if (!canConvert) generatedSeed = newWorldSeed.GetHashCode();

		WorldInfo worldInfo = new WorldInfo();
		worldInfo.id = id;
		worldInfo.name = newWorldName;
		worldInfo.seed = generatedSeed;
		worldInfo.time = (uint)(System.DateTime.Now.Subtract(new System.DateTime(1970, 1, 1))).TotalSeconds;
		worldInfo.type = WorldInfo.Type.Default;
		mainMenu.StartGame(worldInfo);
	}
}
