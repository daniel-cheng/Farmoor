using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
	public static UI instance { get; private set; }
	public GameObject playingUI;
	private bool hideUI = false;
	public Hotbar hotbar;
	public LoadingScreen loadingScreen;
	public Console console;
	public TextMeshProUGUI errorText;
	public CanvasGroup errorCanvasGroup;
	private float errorTimer = 0;
	public void Initialize()
	{
		instance = this;
		hotbar.Initialize();
		loadingScreen.Initialize();
		errorCanvasGroup.gameObject.SetActive(false);
	}
	public void UpdateUI()
	{
		hotbar.UpdateHotbar();
		if (Input.GetKeyDown(KeyCode.F1))
		{
			hideUI = !hideUI;
			playingUI.gameObject.SetActive(!hideUI);
		}

		if (Input.GetKeyDown(KeyCode.Slash))
		{
			console.gameObject.SetActive(true);
		}
		if (console.gameObject.activeSelf)
		{
			console.UpdateConsole();
		}
		if (errorTimer > 0)
		{
			errorCanvasGroup.gameObject.SetActive(true);
			errorTimer -= Time.deltaTime;
			errorCanvasGroup.alpha= Mathf.Clamp01(errorTimer);
		}
		else
		{
			errorCanvasGroup.gameObject.SetActive(false);
		}
	}
	public void ShowError(string text, float duration)
	{
		errorText.text = text;
		errorTimer = duration;
	}
}
