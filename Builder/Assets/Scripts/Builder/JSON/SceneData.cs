using System;

[Serializable]
public class SceneData
{
    public string name;
    public int chunksX;
    public int chunksY;
    public SharedObjectData ground;
    public SharedObjectData[] sharedObjects;
}
