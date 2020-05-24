using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Linq;

public class Console : MonoBehaviour
{
	public TMP_InputField inputField;
	private void OnEnable()
	{
		inputField.ActivateInputField();
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;

	}

	private void OnDisable()
	{
		Cursor.lockState = CursorLockMode.Locked;
		Cursor.visible = false;
		inputField.text = "";
	}

	public void UpdateConsole()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			gameObject.SetActive(false);
			return;
		}
		if (Input.GetKeyDown(KeyCode.Return))
		{
			if (EventSystem.current.currentSelectedGameObject == inputField.gameObject)
			{
				try
				{
					Execute();
				}
				catch (System.Exception e)
				{
					UI.instance.ShowError(e.Message, 3);
				}
				gameObject.SetActive(false);
			}
			else
			{
				Debug.Log("Not selected, instead: " + EventSystem.current.currentSelectedGameObject);
			}
		}
	}

	private void Execute()
	{
		string[] args = inputField.text.Split(' ');
		if (args.Length == 0 || string.IsNullOrEmpty(args[0]))
		{
			UI.instance.ShowError("No command entered", 3);
			return;
		}
		string commandsLog = "";
		for (int i = 0; i < args.Length; ++i)
		{
			commandsLog += "[" + args[i] + "] ";
		}
		Debug.Log("Commands received: "+commandsLog);

		switch (args[0].ToLower())
		{
			case "tp":
				if (AreParametersInvalid(args, 3)) return;
				if (AnyParameterNotANumber(args)) return;
				GameManager.instance.player.transform.position = new Vector3(
					int.Parse(args[1]),
					int.Parse(args[2]),
					int.Parse(args[3])
					);
				return;
			case "playerstate":
				if (AreParametersInvalid(args, 1)) return;
				Player.State newState;
				switch (args[1].ToLower())
				{
					case "survival":
						newState = Player.State.Survival;
						break;
					case "creative":
						newState = Player.State.Creative_Walking;
						break;
					case "spectator":
						newState = Player.State.Spectator;
						break;
					default:
						UI.instance.ShowError($"Parameter: \"{args[1]}\" is not a valid player state", 3);
						return;
				}
				GameManager.instance.player.state = newState;
				return;
			case "modify":
				if (AreParametersInvalid(args, 4)) return;
				if (AnyParameterNotANumber(args)) return;
				Vector3Int pos = new Vector3Int(int.Parse(args[1]), int.Parse(args[2]), int.Parse(args[3]));
				int blockType = int.Parse(args[4]);
				if (blockType > 255 || blockType < 0)
				{
					UI.instance.ShowError($"Block type \"{blockType.ToString()}\" is out of range (0-255)", 3);
					return;
				}
				World.activeWorld.Modify(pos.x, pos.y, pos.z, (byte)blockType);

				return;
			case "textures":
				if (AreParametersInvalid(args, 1)) return;
				switch (args[1].ToLower())
				{
					case "on":
						Shader.EnableKeyword("TEXTURES_ON");
						Shader.DisableKeyword("TEXTURES_OFF");
						return;
					case "off":
						Shader.EnableKeyword("TEXTURES_OFF");
						Shader.DisableKeyword("TEXTURES_ON");
						return;
					default:
						UI.instance.ShowError($"Invalid parameter \"{args[1]}\" (on/off)", 3);

						return;
				}
			case "shownormals":
				if (AreParametersInvalid(args, 1)) return;
				switch (args[1].ToLower())
				{
					case "true":
						Shader.EnableKeyword("VIEW_NORMALS_ON");
						Shader.DisableKeyword("VIEW_NORMALS_OFF");
						return;
					case "false":
						Shader.EnableKeyword("VIEW_NORMALS_OFF");
						Shader.DisableKeyword("VIEW_NORMALS_ON");
						return;
					default:
						UI.instance.ShowError($"Invalid parameter \"{args[1]}\" (true/false)", 3);
						return;
				}
			case "lighting":
				if (AreParametersInvalid(args, 1)) return;
				switch (args[1].ToLower())
				{
					case "on":
						Shader.EnableKeyword("LIGHTING_ON");
						Shader.DisableKeyword("LIGHTING_OFF");
						return;
					case "off":
						Shader.EnableKeyword("LIGHTING_OFF");
						Shader.DisableKeyword("LIGHTING_ON");
						return;
					default:
						UI.instance.ShowError($"Invalid parameter \"{args[1]}\" (on/off)", 3);
						return;
				}
			case "resetshaders":
				Shader.EnableKeyword("VIEW_NORMALS_OFF");
				Shader.DisableKeyword("VIEW_NORMALS_ON");
				Shader.EnableKeyword("TEXTURES_ON");
				Shader.DisableKeyword("TEXTURES_OFF");
				Shader.EnableKeyword("LIGHTING_ON");
				Shader.DisableKeyword("LIGHTING_OFF");
				return;
				
			default:
				UI.instance.ShowError($"Unknown command: \"{args[0]}\"", 3);
				return;
			
		}

	}

	private bool AreParametersInvalid(string[] args, int amount)
	{
		if (args.Length - 1 != amount)
		{
			UI.instance.ShowError($"Command \"{args[0]}\" expects {amount} parameters", 3);
			return true;
		}
		return false;
	}

	private bool AnyParameterNotANumber(string[] args)
	{
		for (int i = 1; i < args.Length; ++i)
		{
			if (IsParameterNotANumber(args[i]))return true;
		}
		return false;
	}

	private bool IsParameterNotANumber(string parameter)
	{
		bool notANumber = !parameter.All(char.IsDigit);
		if (notANumber)
		{
			UI.instance.ShowError($"Parameter: \"{parameter}\" is not an integer number", 3);
			return true;
		}
		return false;
	}
}
