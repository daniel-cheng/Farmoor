﻿[System.Serializable]
public class WorldInfo
{
	public int id;
	public uint time;
	public string name;
	public int seed;
	public override string ToString()
	{
		return $"id[{id}] name[{name}] seed[{seed}] time[{time}]";
	}
}
