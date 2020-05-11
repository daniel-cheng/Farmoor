using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class MenuSelectWorld : MonoBehaviour
{
	public MenuWorldElement worldElementPrefab;
	private List<MenuWorldElement> elements;
    void OnEnable()
    {
		if (elements != null)
		{
			for (int i = elements.Count-1; i > -1; --i)
			{
				Destroy(elements[i].gameObject);
			}
			elements.Clear();
		}
		else
		{
			elements = new List<MenuWorldElement>();
		}
		
		DirectoryInfo worldFolder = new DirectoryInfo(Application.persistentDataPath + "/Worlds");
		foreach (DirectoryInfo d in worldFolder.GetDirectories())
		{
			FileInfo worldInfoFile = new FileInfo(d.FullName + "/Info.json");
			if (worldInfoFile.Exists)
			{
				WorldInfo worldInfo = JsonUtility.FromJson<WorldInfo>(File.ReadAllText(worldInfoFile.FullName));
				MenuWorldElement element = Instantiate(worldElementPrefab);
				element.worldInfo = worldInfo;
				element.transform.SetParent(worldElementPrefab.transform.parent);
				element.transform.localScale = worldElementPrefab.transform.localScale;
				element.worldName.text = worldInfo.name;
				FileInfo thumbnail = new FileInfo(d.FullName + "/Thumbnail.png");
				if (thumbnail.Exists)
				{
					byte[] thumbnailBytes = File.ReadAllBytes(thumbnail.FullName);
					Texture2D texture = new Texture2D(2, 2, TextureFormat.RGB24, false);
					texture.LoadImage(thumbnailBytes);
					texture.Apply();
					element.thumbnail.texture = texture;
				}
				elements.Add(element);
				element.gameObject.SetActive(true);
			}
		}
		worldElementPrefab.gameObject.SetActive(false);

	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
