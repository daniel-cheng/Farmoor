using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseText : MonoBehaviour
{
	private Texture2D texture;
	private Material mat;
	private Color[] c;
	public int seed;
	public float scale1 = 1;
	public float scale2 = 5;
	public float scale3 = 5;
	public float scale4 = 5;


	void Start()
    {
		texture = new Texture2D(1024, 1024, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;
		mat = new Material(Shader.Find("Unlit/Texture"));
		(GetComponent<MeshRenderer>().material = mat).mainTexture = texture;
    }
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space)) Generate();
	}
	void Generate()
	{
		FastNoise noise = new FastNoise(seed);
		noise.SetFractalType(FastNoise.FractalType.FBM);
		for (int y = 0; y < 1024; ++y)
		{
			for (int x = 0; x < 1024; ++x)
			{
				float c1 = noise.GetPerlin(x * scale1, y * scale1);
				float c2 = noise.GetPerlin(x * scale2, y * scale2);
				float c3 = noise.GetPerlin(x * scale3, y * scale3);
				float c4 = noise.GetCubicFractal(x * scale4, y * scale4);

				//c1 = c1 > .2f ? 1f : 0f;
				//c2 = c2 > .2f ? 1f : 0f;

				float c = Mathf.Min(c1, c2)+(c3*0.01f);
				c = Mathf.Clamp01(c - 0.1f) / 0.9f;
				c = Mathf.Pow(c, 1f/2);
				//if (c == 0) c = 1;
				float result = c;
				//result = result * 0.5f + 0.5f;
				texture.SetPixel(x, y, new Color(result, result, result, 1));
			}
		}
		texture.Apply();
	}
}
