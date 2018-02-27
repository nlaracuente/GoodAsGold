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
    /// Clears all existing tiles and re-creates the map
    /// </summary>
    public void GenerateMap(bool clearMap = false)
    {
        if (clearMap) {
            ClearMap();
            ClearMap();
            ClearMap();
        }
        
        CreateDefinitionTable();
        SpawnTiles();
    }

    /// <summary>
    /// Destroys all instantiates tile prefabs
    /// </summary>
    void ClearMap()
    {
        // Pickup stragglers or anything we lost reference to
        foreach(Transform child in transform) {
            DestroyImmediate(child.gameObject);
        }
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
        for (int x = 0; x < m_mapImage.width; x++) {
            for (int z = 0; z < m_mapImage.height; z++) {
                Color32 colorId = m_mapImage.GetPixel(x, z);
                GameObject prefab = GetPrefabByColorId(colorId);

                if(prefab == null) {
                    continue;
                }

                string instanceName = string.Format("{0}_{1}_{2}", prefab.name, z, x);
                
                Vector3 position = new Vector3(x * m_tileWidth, 0f, z * m_tileHeight);
                GameObject instance = Instantiate(prefab, position, Quaternion.identity);

                instance.name = instanceName;
                string parentName = string.Format("_{0}", instance.tag);

                // Remove duplicates
                if (transform.Find(parentName) && transform.Find(parentName).Find(instanceName) != null) {
                    DestroyImmediate(instance);
                } else {
                    SetInstanceParent(instance, parentName);
                }
            }
        }
    }

    /// <summary>
    /// Parents the instance based on its tag
    /// </summary>
    /// <param name="instance"></param>
    void SetInstanceParent(GameObject instance, string parentName)
    {
        GameObject parent = null;

        // use existing or create it
        if (transform.Find(parentName)) {
            parent = transform.Find(parentName).gameObject;
        } else {
            parent = new GameObject(parentName);
            parent.transform.SetParent(transform);
        }

        instance.transform.SetParent(parent.transform);
    }
}
