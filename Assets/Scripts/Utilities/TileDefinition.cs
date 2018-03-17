using UnityEngine;

/// <summary>
/// Associates colors with a prefab used when interpreting the level image resource
/// </summary>
[System.Serializable]
public struct TileDefinition
{
    public Color32 color;
    public GameObject prefab;

    public void MapData(Color32 color, GameObject prefab)
    {
        this.color = color;
        this.prefab = prefab;
    }
}
