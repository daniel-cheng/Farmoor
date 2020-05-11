using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class MenuSubTitle : MonoBehaviour
{
	public TextMeshProUGUI text1, text2;
	private int refreshes = 0;
	private string[] titles;
	private void Start()
	{
		titles = Resources.Load<TextAsset>("Splash").text.Split("\n"[0]);
		string textToDisplay = titles[new System.Random().Next(titles.Length)];
		text1.text = text2.text = textToDisplay;
	}
	void Update()
    {
		float t = Time.time;
		float s = (Mathf.Max( Mathf.Sin(t*12f)+0.75f, 0))*0.05f+1;
		transform.localScale = Vector3.one * s;
		if (Input.GetKeyDown(KeyCode.F5))
		{
			refreshes++;
			string textToDisplay = titles[new System.Random().Next(titles.Length)];
			if (refreshes == 5) textToDisplay = "Looking for a specific title?";
			if (refreshes == 10) textToDisplay = "There is no hidden title...";
			if (refreshes == 15) textToDisplay = "F5 F5 F5 F5 F5 F5 F5 F5 F5 F5 F5 F5 F5 F5 F5 F5 F5 F5 F5 F5 F5";
			if (refreshes == 20) textToDisplay = "Did Jeffrey told you to do this?";
			text1.text = text2.text = textToDisplay;
		}
    }
}
