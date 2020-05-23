using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class GameManager : MonoBehaviour
{
	public static GameManager instance { get; private set; }
	public bool showLoadingScreen = true;
	public Player player;
	public World world;
	public GameSettings gameSettings;
	public UI ui;
	private SaveDataManager saveDataManager;
	public TextureMapper textureMapper;
	public AudioManager audioManager;
	public bool isInStartup;
	public WorldInfo testWorld;
	public Texture2D textures, uvTexture;
	public Camera screenshotCamera;
	public Texture2D latestScreenshot;
	public TextMeshProUGUI debugText;
	private void Start()
	{
		instance = this;
		Initialize();
		BlockTypes.Initialize();
		textureMapper = new TextureMapper();

		if (AudioManager.instance == null)
		{
			audioManager.Initialize();
		}
		audioManager = AudioManager.instance;


		CreateTextures();
		Structure.Initialize();


		WorldInfo worldInfo = MainMenu.worldToLoad != null ? MainMenu.worldToLoad : testWorld;
		InitializeWorld(worldInfo);

		ui.Initialize();

		//_ColorHorizon, _ColorTop, _ColorBottom;
		Shader.SetGlobalColor("_SkyColorTop",new Color( 0.7692239f, 0.7906416f, 0.8113208f,1f));
		Shader.SetGlobalColor("_SkyColorHorizon", new Color(0.3632075f, 0.6424405f, 1f, 1f));
		Shader.SetGlobalColor("_SkyColorBottom", new Color(0.1632253f, 0.2146282f, 0.2641509f, 1f));
		Shader.SetGlobalFloat("_MinLightLevel", gameSettings.minimumLightLevel);
		Shader.SetGlobalInt("_RenderDistance", gameSettings.RenderDistance);
#if !UNITY_EDITOR
		showLoadingScreen = true;
#endif
		if (showLoadingScreen)
		{
			isInStartup = true;
			world.chunkManager.isInStartup = true;
			ui.loadingScreen.gameObject.SetActive(true);
		}
	}

	private void Update()
	{
		debugText.text = "";
		if (!audioManager.IsPlayingMusic())
		{
			if (isInStartup)
			{
				audioManager.PlayNewPlaylist(audioManager.music.menu.clips);
			}
			else
			{
				audioManager.PlayNewPlaylist(audioManager.music.game.clips);

			}
		}
		else
		{
			if (!isInStartup)
			{
				if (audioManager.musicPlaylist != audioManager.music.game.clips)
				{
					audioManager.PlayNewPlaylist(audioManager.music.game.clips);

				}
			}
		}
		if (isInStartup)
		{
			if (world.chunkManager.StartupFinished())
			{
				world.chunkManager.isInStartup = false;
				isInStartup = false;
				ui.loadingScreen.gameObject.SetActive(false);
				audioManager.PlayNewPlaylist(audioManager.music.game.clips);
				System.GC.Collect();
			}
		}
		player.disableInput = ui.console.gameObject.activeSelf;
		player.UpdatePlayer();
		world.UpdateWorld();
		ui.UpdateUI();
		DebugStuff();
	}

	private void Initialize()
	{
		saveDataManager = new SaveDataManager();
	}

	public void InitializeWorld(WorldInfo worldInfo)
	{
		worldInfo = saveDataManager.Initialize(worldInfo);
		world.Initialize(worldInfo);
	}

	private void CreateTextures()
	{
		Texture2D temp = new Texture2D(textures.width, textures.height, TextureFormat.ARGB32, 5, false);
		temp.SetPixels(textures.GetPixels());
		temp.filterMode = FilterMode.Point;
		temp.Apply();
		textures = temp;
		Shader.SetGlobalTexture("_BlockTextures", textures);
		Shader.SetGlobalTexture("_UVTexture", uvTexture);
	}

	public void AddDebugLine(string line)
	{
		debugText.text += line + "\n";
	}

	private void DebugStuff()
	{
		if (Input.GetKeyDown(KeyCode.F3))
		{
			debugText.gameObject.SetActive(!debugText.gameObject.activeSelf);
		}
		//360 screenshot
		if (Input.GetKeyDown(KeyCode.F4))
		{
			RenderTexture cubemap = new RenderTexture(4096, 4096, 0, RenderTextureFormat.ARGB32);
			cubemap.dimension = UnityEngine.Rendering.TextureDimension.Cube;
			cubemap.Create();
			screenshotCamera.transform.position = world.mainCamera.transform.position;
			screenshotCamera.RenderToCubemap(cubemap);

			RenderTexture equirect = new RenderTexture(4096, 2048, 0, RenderTextureFormat.ARGB32);
			Texture2D texture = new Texture2D(4096, 2048, TextureFormat.ARGB32, false);
			cubemap.ConvertToEquirect(equirect, Camera.MonoOrStereoscopicEye.Mono);
			RenderTexture temp = RenderTexture.active;
			RenderTexture.active = equirect;
			texture.ReadPixels(new Rect(0, 0, equirect.width, equirect.height), 0, 0);
			RenderTexture.active = temp;
			texture.Apply();
			latestScreenshot = texture;
			System.IO.FileInfo file = new System.IO.FileInfo(Application.persistentDataPath + "/" + TimeStamp().ToString() + ".png");
			System.IO.File.WriteAllBytes(file.FullName, texture.EncodeToPNG());
		}

		if (Input.GetKeyDown(KeyCode.F8))
		{
			UnloadAll(); //refresh test
		}
	}

	public void UnloadAll()
	{
		CreateScreenshot();
		world.chunkManager.UnloadAll();
	}

	private void OnApplicationQuit()
	{
		if (!this.enabled) return;
		Debug.Log("OnApplicationQuit called in GameManager");
		UnloadAll();
		this.enabled = false;
	}

	private void OnDestroy()
	{
		if (!this.enabled) return;
		Debug.Log("OnDestroy called in GameManager");
		UnloadAll();
		this.enabled = false;
	}

	public void CreateScreenshot()
	{
		RenderTexture temporary = RenderTexture.GetTemporary(256, 144, 0, RenderTextureFormat.ARGB32);
		screenshotCamera.transform.position = world.mainCamera.transform.position;
		screenshotCamera.transform.rotation = world.mainCamera.transform.rotation;
		screenshotCamera.fieldOfView = world.mainCamera.fieldOfView;
		screenshotCamera.targetTexture = temporary;
		screenshotCamera.Render();
		Texture2D texture = new Texture2D(256, 144, TextureFormat.ARGB32, false);

		RenderTexture temp = RenderTexture.active;
		RenderTexture.active = temporary;
		texture.ReadPixels(new Rect(0, 0, temporary.width, temporary.height), 0, 0);
		RenderTexture.active = temp;
		texture.Apply();
		latestScreenshot = texture;
		RenderTexture.ReleaseTemporary(temporary);
		WorldInfo info = world.info;
		System.IO.FileInfo thumb = new System.IO.FileInfo(Application.persistentDataPath + "/Worlds/" + info.id + "/Thumbnail.png");
		System.IO.File.WriteAllBytes(thumb.FullName, texture.EncodeToPNG());
	}

	private long TimeStamp()
	{
		return System.DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
	}
}
