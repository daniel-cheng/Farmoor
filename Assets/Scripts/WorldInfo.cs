[System.Serializable]
public class WorldInfo
{
	public int id;
	public uint time;
	public string name;
	public int seed;
	public Type type;
	public enum Type
	{
		Default,
		FloatingIslands,
		Flat
	}
	public override string ToString()
	{
		return $"id[{id}] name[{name}] type[{type}] seed[{seed}] time[{time}]";
	}
}
