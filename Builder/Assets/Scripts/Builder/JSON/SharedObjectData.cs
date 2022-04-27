using System;

[Serializable]
public class SharedObjectData
{
    public int id;
    public string url;
    public string name;
    public string[] tags;
    public SceneObjectData[] objectsData;
}
