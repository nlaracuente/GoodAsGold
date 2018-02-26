using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// A list of definitions for the colors found in the image resource
    /// </summary>
    [SerializeField]
    List<TileDefinition> m_tileDefinitions;

    /// <summary>
    /// A reference to the image resource that represents this map
    /// </summary>
    [SerializeField]
    Texture2D m_mapImage;

    /// <summary>
    /// Tiles width
    /// </summary>
    [SerializeField]
    int m_tileWidth = 2;

    /// <summary>
    /// Tile's breadth 
    /// </summary>
    [SerializeField]
    int m_tileHeight = 2;

    /// <summary>
    /// Stores the tile definitions in a hash table for quick reference
    /// </summary>
    Dictionary<Color32, GameObject> m_definitionTable = new Dictionary<Color32, GameObject>();

    /// <summary>
    /// A reference to all instantiated tile prefabs
    /// </summary>
    List<GameObject> m_tiles = new List<GameObject>();

    /// <summary>
    /// Trigger map generation
    /// </summary>
    void Start()
    {
        GenerateMap();
    }

    /// <summary>
    /// Clears all existing tiles and re-creates the map
    /// </summary>
    public void GenerateMap()
    {
        ClearMap();
        ClearMap();
        ClearMap();
        CreateDefinitionTable();
        SpawnTiles();
    }

    /// <summary>
    /// Destroys all instantiates tile prefabs
    /// </summary>
    void ClearMap()
    {
        foreach(GameObject tile in m_tiles) {
            if(tile != null) {
                DestroyImmediate(tile.gameObject);

            }
        }

        // Pickup stragglers or anything we lost reference to
        foreach(Transform child in transform) {
            DestroyImmediate(child.gameObject);
        }

        m_tiles.Clear();
    }

    /// <summary>
    /// Creates the definition table
    /// </summary>
    void CreateDefinitionTable()
    {
        foreach (TileDefinition definition in m_tileDefinitions) {
            if (!m_definitionTable.ContainsKey(definition.color)) {
                m_definitionTable.Add(definition.color, definition.prefab);
            }
        }
    }

    /// <summary>
    /// Returns the associated prefab if the given color id is recognize
    /// Returns NULL when a match is not found
    /// </summary>
    /// <param name="colordId"></param>
    /// <returns></returns>
    GameObject GetPrefabByColorId(Color32 colordId)
    {
        GameObject prefab = null;

        if (m_definitionTable.ContainsKey(colordId)) {
            prefab = m_definitionTable[colordId];
        }

        return prefab;
    }

    /// <summary>
    /// Spawns all the recognize tiles from the <see cref="m_mapImage"/>
    /// </summary>
    void SpawnTiles()
    {
        for (int column = 0; column < m_mapImage.width; column++) {
            for (int row = 0; row < m_mapImage.height; row++) {
                Color32 colorId = m_mapImage.GetPixel(column, row);
                GameObject prefab = GetPrefabByColorId(colorId);

                if(prefab == null) {
                    continue;
                }

                string name = string.Format("{0}_{1}_{2}", prefab.name, row, column);

                // Avoid duplicates
                if (transform.Find(name) != null) {
                    continue;
                }

                Vector3 position = new Vector3(row * m_tileWidth, 0f, column * m_tileHeight);
                GameObject instance = Instantiate(prefab, position, Quaternion.identity, transform);
                instance.name = name;

                m_tiles.Add(instance);
            }
        }
    }
}
