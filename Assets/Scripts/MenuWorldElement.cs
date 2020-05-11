using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class MenuWorldElement : MonoBehaviour
{
	public MainMenu mainMenu;
	public WorldInfo worldInfo;
	public TextMeshProUGUI worldName;
	public RawImage thumbnail;
	public void LoadWorld()
	{
		mainMenu.StartGame(worldInfo);
	}
}
