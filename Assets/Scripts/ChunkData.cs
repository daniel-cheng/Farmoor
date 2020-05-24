using UnityEngine;
using System.Threading;
using System.Collections.Generic;
public class ChunkData
{
	public Vector2Int position;
	private byte[,,] blocks;
	public bool terrainReady { get; private set; }
	public bool startedLoadingDetails { get; private set; }
	public bool chunkReady { get; private set; }

	public bool isDirty;

	private const int SURFACE_HEIGHT = 64;

	//devide by chance ( 1 in X )
	const int STRUCTURE_CHANCE_TREE = (int.MaxValue / 100);
	const int STRUCTURE_CHANCE_WELL = (int.MaxValue / 512);
	const int STRUCTURE_CHANCE_CAVE_ENTRANCE = (int.MaxValue / 20);


	private Thread loadTerrainThread;
	private Thread loadDetailsThread;

	private WorldInfo worldInfo;

	public HashSet<Vector2Int> references;

	public List<StructureInfo> structures;

	public Dictionary<Vector3Int, byte> lightSources;

	public byte[,] highestNonAirBlock;

	public ChunkSaveData saveData;

	private ChunkData front, left, back, right; //neighbours (only exist while loading structures)

	private volatile int worldX, worldZ;
	private FastNoise noise;

	public struct StructureInfo
	{
		public StructureInfo(Vector3Int position, Structure.Type type, int seed)
		{
			this.position = position;
			this.type = type;
			this.seed = seed;
		}
		public Vector3Int position;
		public Structure.Type type;
		public int seed;
	}


	public ChunkData(Vector2Int position, WorldInfo worldInfo)
	{
		this.position = position;
		this.worldInfo=worldInfo;
		noise = World.noise;
		terrainReady = false;
		startedLoadingDetails = false;
		chunkReady = false;
		isDirty = false;
		references = new HashSet<Vector2Int>();
		lightSources = new Dictionary<Vector3Int, byte>();
		highestNonAirBlock = new byte[16, 16];
	}

	public byte[,,] GetBlocks()
	{
		//if (!chunkReady) throw new System.Exception($"Chunk {position} has not finished loading");
		return blocks;
	}

	public void StartTerrainLoading()
	{
		//Debug.Log($"Chunk {position} start terrain loading");
		loadTerrainThread = new Thread(LoadTerrain);
		loadTerrainThread.IsBackground = true;
		loadTerrainThread.Start();
	}

	public void StartDetailsLoading(ChunkData front, ChunkData left, ChunkData back, ChunkData right)
	{
		//Debug.Log($"Chunk {position} start structure loading");
		//need to temporarily cache chunkdata of neighbors since generation is on another thread
		this.front = front;
		this.left = left;
		this.right = right;
		this.back = back;

		loadDetailsThread = new Thread(LoadDetails);
		loadDetailsThread.IsBackground = true;
		loadDetailsThread.Start();
		startedLoadingDetails = true;
	}

	public void LoadTerrain() //also loads structures INFO
	{
		blocks = new byte[16, 256, 16];
		Vector2Int worldPos = position * 16;

		System.Random tmpTest = new System.Random();
		for (int z = 0; z < 16; ++z)
		{
			for (int x = 0; x < 16; ++x)
			{
				if (worldInfo.type == WorldInfo.Type.Flat)
				{
					for (int y = 4; y <256; ++y)
					{
						blocks[x, y, z] = BlockTypes.AIR;	
					}
					blocks[x, 3, z] = tmpTest.Next(32)==0?  BlockTypes.GRASS:BlockTypes.DIRT;
					blocks[x, 2, z] = BlockTypes.DIRT;
					blocks[x, 1, z] = BlockTypes.DIRT;
					blocks[x, 0, z] = BlockTypes.BEDROCK;
					continue;
				}

				worldX = position.x * 16 + x;
				worldZ = position.y * 16 + z;
				int bottomHeight = 0;
				float hills = noise.GetPerlin(worldX * 4f + 500, worldZ * 4f) * 0.5f + .5f;

				if (worldInfo.type == WorldInfo.Type.FloatingIslands)
				{
					float distanceToSpawn = Vector2.Distance(new Vector2(worldX, worldZ), Vector2.zero);
					float bigIsland = Mathf.Clamp01((250f - distanceToSpawn) / 250f);
					float i1 = noise.GetPerlin(worldX * .5f, worldZ * .5f);
					float i2 = noise.GetPerlin(worldX * 1f, worldZ * 1f);
					float i3 = noise.GetPerlin(worldX * 5f, worldZ * 5f);
					float height = Mathf.Min(i1, i2) + bigIsland + (i3 * 0.02f);
					height = Mathf.Clamp01(height - 0.1f) / 0.9f;
					height = Mathf.Pow(height, 1f / 2);
					if (height == 0)
					{
						for (int y = 0; y < 256; ++y)
						{
							blocks[x, y, z] = BlockTypes.AIR;
						}
						continue;
					}
					hills *= height; //smooth edge
					bottomHeight = (int)(SURFACE_HEIGHT - (height * 80));
				}

				int hillHeight = (int)(SURFACE_HEIGHT + (hills * 16));
				float bedrock = noise.GetPerlin(worldX * 64f, worldZ * 64f)*0.5f+0.5f;
				int bedrockHeight = (int)(1 + bedrock * 4);



				for (int y = 0; y < 256; ++y)
				{
					if (y > hillHeight || y<bottomHeight)
					{
						blocks[x, y, z] = BlockTypes.AIR;
						continue;
					}
					if (y < bedrockHeight)
					{
						blocks[x, y, z] = BlockTypes.BEDROCK;
						continue;
					}

					if (y > hillHeight - 4)
					{
						if (GenerateCaves(x, y, z, 0.2f)) continue;
						if (y == hillHeight)
						{
							blocks[x, y, z] = BlockTypes.GRASS;
							//blocks[x, y, z] = BlockTypes.AIR; //TEMP
							continue;
						}
						blocks[x, y, z] = BlockTypes.DIRT;
						//blocks[x, y, z] = BlockTypes.AIR; //TEMP
						continue;
					}
					else
					{
						if (GenerateCaves(x, y, z, 0f)) continue;
						if (GenerateOres(x, y, z)) continue;
						blocks[x, y, z] = BlockTypes.STONE;
						//blocks[x, y, z] = BlockTypes.AIR; //TEMP
						continue;
					}
				}
			}
		}

		string hash = World.activeWorld.info.seed.ToString() + position.x.ToString() + position.y.ToString();
		int structuresSeed = hash.GetHashCode();
		System.Random rnd = new System.Random(structuresSeed);
		structures = new List<StructureInfo>();
		bool[,] spotsTaken = new bool[16, 16];

		if (worldInfo.type != WorldInfo.Type.Flat)
		{
			//cave entrances
			if (rnd.Next() < STRUCTURE_CHANCE_CAVE_ENTRANCE)
			{

				int h = 255;
				while (h > 0)
				{
					if (blocks[8, h, 8] != BlockTypes.AIR)
					{
						structures.Add(new StructureInfo(new Vector3Int(0, h + 6, 0), Structure.Type.CAVE_ENTRANCE, rnd.Next()));
						break;
					}
					h--;
				}
			}

			//trees
			for (int y = 2; y < 14; ++y)
			{
				for (int x = 2; x < 14; ++x)
				{
					if (rnd.Next() < STRUCTURE_CHANCE_TREE)
					{
						if (IsSpotFree(spotsTaken, new Vector2Int(x, y), 2))
						{
							spotsTaken[x, y] = true;
							int height = 255;
							while (height > 0)
							{
								if (blocks[x, height, y] == BlockTypes.GRASS)
								{
									structures.Add(new StructureInfo(new Vector3Int(x, height + 1, y), Structure.Type.OAK_TREE, rnd.Next()));
									break;
								}
								height--;
							}
						}
					}
				}
			}

		}

		if (rnd.Next() < STRUCTURE_CHANCE_WELL)
		{
			if (IsSpotFree(spotsTaken, new Vector2Int(7, 7), 3))
			{
				//Debug.Log("Spot is free");

				int minH = 255;
				int maxH = 0;
				bool canPlace = true;
				for (int y = 5; y < 11; ++y)
				{
					for (int x = 5; x < 11; ++x)
					{
						for (int h = 255; h > -1; h--)
						{
							byte b = blocks[x, h, y];
							if (b != BlockTypes.AIR)
							{
								//Debug.Log(b);
								canPlace &= (b == BlockTypes.GRASS);
								minH = Mathf.Min(minH, h);
								maxH = Mathf.Max(maxH, h);
								break;
							}
						}
					}
				}
				canPlace &= Mathf.Abs(minH - maxH) < 2;
				if (canPlace)
				{
					Debug.Log("spawning well structure");
					for (int y = 5; y < 11; ++y)
					{
						for (int x = 5; x < 11; ++x)
						{
							spotsTaken[x, y] = true;
						}
					}
					int h = 255;
					while (h > 0)
					{
						if (blocks[7, h, 7] != BlockTypes.AIR)
						{
							structures.Add(new StructureInfo(new Vector3Int(7, h + 1, 7), Structure.Type.WELL, rnd.Next()));
							break;
						}
						h--;
					}
				}
			}
		}

		//already load changes from disk here (apply later)
		saveData = SaveDataManager.instance.Load(position);

		terrainReady = true;
		//Debug.Log($"Chunk {position} terrain ready");
	}

	private bool GenerateCaves(int x, int y, int z, float threshold)
	{
		float cave1 = noise.GetPerlin(worldX * 10f - 400, y * 10f, worldZ * 10f);
		float cave2 = noise.GetPerlin(worldX * 20f - 600, y * 20f, worldZ * 20f);
		float cave3 = noise.GetPerlin(worldX * 5f - 200, y * 5f, worldZ * 5f);
		float cave4 = noise.GetPerlin(worldX * 2f - 300, y * 2f, worldZ * 2f);
		float cave = Mathf.Min(Mathf.Min( cave1, cave4), Mathf.Min(cave2, cave3));

		if (cave > threshold)
		{
			blocks[x, y, z] = BlockTypes.AIR;
			return true;
		}
		return false;
	}

	private bool GenerateOres(int x, int y, int z)
	{
		float ore1 = noise.GetPerlin(worldX * 15f, y * 15f, worldZ * 15f + 300);
		float ore2 = noise.GetPerlin(worldX * 15f, y * 15f, worldZ * 15f + 400);

		
		if (ore1 > 0.3 && ore2 > 0.4)
		{
			blocks[x, y, z] = BlockTypes.DIORITE;
			return true;
		}
		if (ore1 < -0.3 && ore2 < -0.4)
		{
			blocks[x, y, z] = BlockTypes.GRANITE;
			return true;
		}

		if (ore1 > 0.3 && ore2 < -0.4)
		{
			blocks[x, y, z] = BlockTypes.DIRT;
			return true;
		}


		float ore3 = noise.GetPerlin(worldX * 20f, y * 20f, worldZ * 20f + 500);

		if (ore1 < -0.3 && ore3 > 0.4)
		{
			blocks[x, y, z] = BlockTypes.COAL;
			return true;
		}

		float ore4 = noise.GetPerlin(worldX * 21f, y * 21f, worldZ * 21f - 300);

		if (ore4 > 0.6)
		{
			blocks[x, y, z] = BlockTypes.IRON;
			return true;
		}

		if (y < 32)
		{
			float ore5 = noise.GetPerlin(worldX * 22f, y * 22f, worldZ * 22f - 400);

			if (ore5 > 0.7)
			{
				blocks[x, y, z] = BlockTypes.GOLD;
				return true;
			}
			if (y < 16)
			{
				if (ore5 < -0.7)
				{
					blocks[x, y, z] = BlockTypes.DIAMOND;
					return true;
				}
			}
		}
		return false;
	}

	private bool IsSpotFree(bool[,] spotsTaken, Vector2Int position, int size) //x area is for example size + 1 + size
	{
		bool spotTaken = false;
		for (int y = Mathf.Max(0, position.y-size); y < Mathf.Min(15, position.y + size + 1); ++y)
		{
			for (int x = Mathf.Max(0, position.x-size); x < Mathf.Min(15, position.x + size + 1); ++x)
			{
				spotTaken |= spotsTaken[x, y];
			}
		}
		return !spotTaken;
	}

	private void LoadDetails()
	{
		//load structures

		for (int i = 0; i < structures.Count; ++i)
		{
			StructureInfo structure = structures[i];
			bool overwritesEverything = Structure.OverwritesEverything(structure.type);
			Vector3Int p = structure.position;
			int x = p.x;
			int y = p.y;
			int z = p.z;
			List<Structure.Change> changeList = Structure.Generate(structure.type, structure.seed);
			//Debug.Log($"placing {structure.type} wich has {changeList.Count} blocks");
			for (int j = 0; j < changeList.Count; ++j)
			{
				Structure.Change c = changeList[j];
				int placeX = x + c.x;
				int placeY = y + c.y;
				int placeZ = z + c.z;
				if (placeX < 0 || placeX > 15) continue;
				if (placeZ < 0 || placeZ > 15) continue;
				if (placeY < 0 || placeY > 255) continue;
				if (blocks[placeX, placeY, placeZ] == BlockTypes.BEDROCK) continue;

				if (!overwritesEverything)
				{
					//only place new blocks if density is higher or the same (leaves can't replace dirt for example)
					if (blocks[placeX, placeY, placeZ] < BlockTypes.density[c.b]) continue;
				}

				blocks[placeX, placeY, placeZ] = c.b;
			}
		}

		//remove all references to neighbors to avoid them staying in memory when unloading chunks
		front = null;
		left = null;
		right = null;
		back = null;

		//load changes
		List<ChunkSaveData.C> changes = saveData.changes;
		for (int i = 0; i < changes.Count; ++i)
		{
			ChunkSaveData.C c = changes[i];
			blocks[c.x, c.y, c.z] = c.b;
			byte lightLevel = BlockTypes.lightLevel[c.b];
			if (lightLevel > 0)
			{
				lightSources[new Vector3Int(c.x, c.y, c.z)]=lightLevel;
			}
		}

		//get highest non-air blocks to speed up light simulation
		for (int z = 0; z < 16; ++z)
		{
			for (int x = 0; x < 16; ++x)
			{
				highestNonAirBlock[x, z] = 0;
				for (int y = 255; y > -1; --y)
				{
					if (blocks[x, y, z] != BlockTypes.AIR)
					{
						highestNonAirBlock[x, z] = (byte)y;
						break;
					}
				}
			}
		}
		chunkReady = true;
	}

	public void Modify(int x, int y, int z, byte blockType)
	{
		if (!chunkReady) throw new System.Exception("Chunk has not finished loading");
		Debug.Log($"Current highest block at {x}x{z} is {highestNonAirBlock[x, z]}");

		saveData.changes.Add(new ChunkSaveData.C((byte)x, (byte)y, (byte)z, blockType));
		blocks[x, y, z] = blockType;
		if (blockType == BlockTypes.AIR)
		{
			if (highestNonAirBlock[x, z] == y)
			{
				highestNonAirBlock[x, z] = 0;
				for (int yy = y; yy > -1; yy--)
				{
					if (blocks[x, yy, z] != BlockTypes.AIR)
					{
						highestNonAirBlock[x, z] = (byte)yy;
						break;
					}
				}
			}
		}
		else
		{
			highestNonAirBlock[x, z] = (byte)Mathf.Max(highestNonAirBlock[x, z], y);
		}
		Debug.Log($"New highest block at {x}x{z} is {highestNonAirBlock[x, z]}");
	}

	public void Unload()
	{
		if (isDirty)
		{
			SaveDataManager.instance.Save(saveData);
		}
	}
}