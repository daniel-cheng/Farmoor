using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureMapper
{
	public Dictionary<byte, TextureMap> map;
	public TextureMapper()
	{
		map = new Dictionary<byte, TextureMap>();

		map.Add(BlockTypes.GRASS, new TextureMap(
			new TextureMap.Face(new Vector2Int(0, 1)),
			new TextureMap.Face(new Vector2Int(0, 1)),
			new TextureMap.Face(new Vector2Int(0, 1)),
			new TextureMap.Face(new Vector2Int(0, 1)),
			new TextureMap.Face(new Vector2Int(0, 0)),
			new TextureMap.Face(new Vector2Int(0, 2))
			)
		);

		map.Add(BlockTypes.DIRT, new TextureMap(
			new TextureMap.Face(new Vector2Int(0, 2)),
			new TextureMap.Face(new Vector2Int(0, 2)),
			new TextureMap.Face(new Vector2Int(0, 2)),
			new TextureMap.Face(new Vector2Int(0, 2)),
			new TextureMap.Face(new Vector2Int(0, 2)),
			new TextureMap.Face(new Vector2Int(0, 2))
			)
		);

		map.Add(BlockTypes.STONE, new TextureMap(
			new TextureMap.Face(new Vector2Int(1, 0)),
			new TextureMap.Face(new Vector2Int(1, 0)),
			new TextureMap.Face(new Vector2Int(1, 0)),
			new TextureMap.Face(new Vector2Int(1, 0)),
			new TextureMap.Face(new Vector2Int(1, 0)),
			new TextureMap.Face(new Vector2Int(1, 0))
			)
		);

		map.Add(BlockTypes.BEDROCK, new TextureMap(
			new TextureMap.Face(new Vector2Int(2, 0)),
			new TextureMap.Face(new Vector2Int(2, 0)),
			new TextureMap.Face(new Vector2Int(2, 0)),
			new TextureMap.Face(new Vector2Int(2, 0)),
			new TextureMap.Face(new Vector2Int(2, 0)),
			new TextureMap.Face(new Vector2Int(2, 0))
			)
		);

		map.Add(BlockTypes.COAL, new TextureMap(
			new TextureMap.Face(new Vector2Int(1, 1)),
			new TextureMap.Face(new Vector2Int(1, 1)),
			new TextureMap.Face(new Vector2Int(1, 1)),
			new TextureMap.Face(new Vector2Int(1, 1)),
			new TextureMap.Face(new Vector2Int(1, 1)),
			new TextureMap.Face(new Vector2Int(1, 1))
			)
		);

		map.Add(BlockTypes.IRON, new TextureMap(
			new TextureMap.Face(new Vector2Int(1, 2)),
			new TextureMap.Face(new Vector2Int(1, 2)),
			new TextureMap.Face(new Vector2Int(1, 2)),
			new TextureMap.Face(new Vector2Int(1, 2)),
			new TextureMap.Face(new Vector2Int(1, 2)),
			new TextureMap.Face(new Vector2Int(1, 2))
			)
		);

		map.Add(BlockTypes.GOLD, new TextureMap(
			new TextureMap.Face(new Vector2Int(1, 3)),
			new TextureMap.Face(new Vector2Int(1, 3)),
			new TextureMap.Face(new Vector2Int(1, 3)),
			new TextureMap.Face(new Vector2Int(1, 3)),
			new TextureMap.Face(new Vector2Int(1, 3)),
			new TextureMap.Face(new Vector2Int(1, 3))

			)
		);

		map.Add(BlockTypes.DIAMOND, new TextureMap(
			new TextureMap.Face(new Vector2Int(1, 4)),
			new TextureMap.Face(new Vector2Int(1, 4)),
			new TextureMap.Face(new Vector2Int(1, 4)),
			new TextureMap.Face(new Vector2Int(1, 4)),
			new TextureMap.Face(new Vector2Int(1, 4)),
			new TextureMap.Face(new Vector2Int(1, 4))
			)
		);

		map.Add(BlockTypes.LOG_OAK, new TextureMap(
			new TextureMap.Face(new Vector2Int(3, 3)),
			new TextureMap.Face(new Vector2Int(3, 3)),
			new TextureMap.Face(new Vector2Int(3, 3)),
			new TextureMap.Face(new Vector2Int(3, 3)),
			new TextureMap.Face(new Vector2Int(3, 2)),
			new TextureMap.Face(new Vector2Int(3, 2))
			)
		);

		map.Add(BlockTypes.PLANKS_OAK, new TextureMap(
			new TextureMap.Face(new Vector2Int(3, 1)),
			new TextureMap.Face(new Vector2Int(3, 1)),
			new TextureMap.Face(new Vector2Int(3, 1)),
			new TextureMap.Face(new Vector2Int(3, 1)),
			new TextureMap.Face(new Vector2Int(3, 1)),
			new TextureMap.Face(new Vector2Int(3, 1))
			)
		);
		map.Add(BlockTypes.LEAVES_OAK, new TextureMap(
			new TextureMap.Face(new Vector2Int(3, 4)),
			new TextureMap.Face(new Vector2Int(3, 4)),
			new TextureMap.Face(new Vector2Int(3, 4)),
			new TextureMap.Face(new Vector2Int(3, 4)),
			new TextureMap.Face(new Vector2Int(3, 4)),
			new TextureMap.Face(new Vector2Int(3, 4))
			)
		);

		map.Add(BlockTypes.GLOWSTONE, new TextureMap(
			new TextureMap.Face(new Vector2Int(3, 0)),
			new TextureMap.Face(new Vector2Int(3, 0)),
			new TextureMap.Face(new Vector2Int(3, 0)),
			new TextureMap.Face(new Vector2Int(3, 0)),
			new TextureMap.Face(new Vector2Int(3, 0)),
			new TextureMap.Face(new Vector2Int(3, 0))
			)
		);

		map.Add(BlockTypes.ANDESITE, new TextureMap(
			new TextureMap.Face(new Vector2Int(2, 3)),
			new TextureMap.Face(new Vector2Int(2, 3)),
			new TextureMap.Face(new Vector2Int(2, 3)),
			new TextureMap.Face(new Vector2Int(2, 3)),
			new TextureMap.Face(new Vector2Int(2, 3)),
			new TextureMap.Face(new Vector2Int(2, 3))
			)
		);

		map.Add(BlockTypes.DIORITE, new TextureMap(
			new TextureMap.Face(new Vector2Int(2, 1)),
			new TextureMap.Face(new Vector2Int(2, 1)),
			new TextureMap.Face(new Vector2Int(2, 1)),
			new TextureMap.Face(new Vector2Int(2, 1)),
			new TextureMap.Face(new Vector2Int(2, 1)),
			new TextureMap.Face(new Vector2Int(2, 1))
			)
		);

		map.Add(BlockTypes.GRANITE, new TextureMap(
			new TextureMap.Face(new Vector2Int(2, 2)),
			new TextureMap.Face(new Vector2Int(2, 2)),
			new TextureMap.Face(new Vector2Int(2, 2)),
			new TextureMap.Face(new Vector2Int(2, 2)),
			new TextureMap.Face(new Vector2Int(2, 2)),
			new TextureMap.Face(new Vector2Int(2, 2))
			)
		);

		map.Add(BlockTypes.COBBLESTONE, new TextureMap(
			new TextureMap.Face(new Vector2Int(2, 4)),
			new TextureMap.Face(new Vector2Int(2, 4)),
			new TextureMap.Face(new Vector2Int(2, 4)),
			new TextureMap.Face(new Vector2Int(2, 4)),
			new TextureMap.Face(new Vector2Int(2, 4)),
			new TextureMap.Face(new Vector2Int(2, 4))
			)
		);

		map.Add(BlockTypes.GRASS_PATCH_1, new TextureMap(
			new TextureMap.Face(new Vector2Int(5, 2)),
			new TextureMap.Face(new Vector2Int(5, 2)),
			new TextureMap.Face(new Vector2Int(5, 2)),
			new TextureMap.Face(new Vector2Int(5, 2)),
			new TextureMap.Face(new Vector2Int(5, 2)),
			new TextureMap.Face(new Vector2Int(5, 2))
			)
		);
	}

	public class TextureMap
	{
		public TextureMap(Face front, Face back, Face left, Face right, Face top, Face bottom)
		{
			this.front = front;
			this.back = back;
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}
		public Face front, back, left, right, top, bottom;
		public class Face
		{
			public Face(Vector2Int tl)
			{
				this.tl = tl;
				tr = tl + new Vector2Int(1, 0);
				bl = tl + new Vector2Int(0, 1);
				br = tl + new Vector2Int(1, 1);
			}
			public Vector2Int tl, tr, bl, br;
		}
	}
}
