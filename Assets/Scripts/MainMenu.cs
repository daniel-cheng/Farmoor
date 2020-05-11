using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public static WorldInfo worldToLoad = null;
	public AudioManager audioManager;
	public int gameSceneBuildIndex = 1;
	public CanvasGroup menuMain, menuWorlds, menuNewWorld;
	public CanvasGroup currentMenu, targetMenu;

	private void Start()
	{
		menuMain.gameObject.SetActive(false);
		menuWorlds.gameObject.SetActive(false);
		menuNewWorld.gameObject.SetActive(false);
		currentMenu = null;
		targetMenu = null;

		if (AudioManager.instance == null)
		{
			audioManager.Initialize();
		}
		audioManager = AudioManager.instance;

		ShowMenu(menuMain);
	}
	private void Update()
	{
		SwitchMenus();
		if (!audioManager.IsPlayingMusic())
		{
			audioManager.PlayNewPlaylist(audioManager.music.menu.clips);
		}
	}

	private void SwitchMenus()
	{
		if (targetMenu != null)
		{
			if (targetMenu != currentMenu)
			{
				if (currentMenu != null)
				{
					currentMenu.alpha = Mathf.Clamp01(currentMenu.alpha - Time.deltaTime*16f);
					((RectTransform)currentMenu.transform).anchoredPosition = new Vector2(-(1 - currentMenu.alpha)*64, 0);
					if (currentMenu.alpha > 0) return;
					currentMenu.gameObject.SetActive(false);
					currentMenu = null;
					targetMenu.alpha = 0;
					targetMenu.gameObject.SetActive(true);
					return; //wait 1 frame
				}
				targetMenu.gameObject.SetActive(true);
				targetMenu.alpha = Mathf.Clamp01(targetMenu.alpha + Time.deltaTime*16f);
				((RectTransform)targetMenu.transform).anchoredPosition = new Vector2((1 - targetMenu.alpha)*64, 0);
				if (targetMenu.alpha < 1) return;
				currentMenu = targetMenu;
				targetMenu = null;
			}
		}
	}

	public void ShowMenu(CanvasGroup canvasGroup)
	{
		if (targetMenu == null) targetMenu = canvasGroup;
	}
	public void StartGame(WorldInfo worldInfo)
	{
		worldToLoad = worldInfo;
		Debug.Log("Starting " + worldToLoad);
		UnityEngine.SceneManagement.SceneManager.LoadScene(gameSceneBuildIndex);
	}
}
