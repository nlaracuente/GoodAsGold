using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapGenerator : MonoBehaviour
{
    /// <summary>
    /// A list of definitions for the colors found in the image resource
    /// </summary>
    [SerializeField]
    List<TileDefinition> m_tileDefinitions;

    /// <summary>
    /// The location within the Resource folder where the level textures are located at
    /// </summary>
    [SerializeField]
    string m_resourcePath = "Levels";

    /// <summary>
    /// A reference to the image resource that represents this map
    /// </summary>
    [SerializeField]
    Texture2D m_textureMap;

    /// <summary>
    /// Stores the tile definitions in a hash table for quick reference
    /// </summary>
    Dictionary<Color32, GameObject> m_definitionTable = new Dictionary<Color32, GameObject>();

    /// <summary>
    /// A collection of all the spawn tiles based on their x,z position
    /// </summary>
    GameObject[,] m_tiles;

    /// <summary>
    /// A list of all the tile tags to run the setup logic for in the order required
    /// </summary>
    [SerializeField, Tooltip("Tag names in the order to process the setup logic")]
    List<string> m_setupOrder = new List<string>();

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

        SetTextureMap();
        CreateDefinitionTable();
        SpawnTiles();
        SetupTiles();
    }

    /// <summary>
    /// Stores the texture map data from the resources folder when one is not already set
    /// </summary>
    void SetTextureMap()
    {
        if (m_textureMap == null) {
            string path = string.Format("{0}/{1}", m_resourcePath, SceneManager.GetActiveScene().name);
            m_textureMap = Resources.Load<Texture2D>(path);
        }
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
    /// Spawns all the recognize tiles from the <see cref="m_textureMap"/>
    /// </summary>
    void SpawnTiles()
    {
        m_tiles = new GameObject[m_textureMap.width, m_textureMap.height];

        float x_offset = (m_textureMap.width * GameManager.tileXSize) / 2;
        float z_offset = (m_textureMap.height * GameManager.tileZSize) / 2;

        for (int x = 0; x < m_textureMap.width; x++) {
            for (int z = 0; z < m_textureMap.height; z++) {
                Color32 colorId = m_textureMap.GetPixel(x, z);
                GameObject prefab = GetPrefabByColorId(colorId);

                if(prefab == null) {
                    continue;
                }

                string instanceName = string.Format("{0}_{1}_{2}", prefab.name, z, x);
                
                Vector3 position = new Vector3(x * GameManager.tileXSize, 0f, z * GameManager.tileZSize);
                position.x -= x_offset;
                position.z -= z_offset;
                GameObject instance = Instantiate(prefab, position, Quaternion.identity);

                instance.name = instanceName;
                string parentName = string.Format("_{0}s", prefab.name);

                // Remove duplicates
                if (transform.Find(parentName) && transform.Find(parentName).Find(instanceName) != null) {
                    DestroyImmediate(instance);
                } else {
                    SetInstanceParent(instance, parentName);
                }

                // Store the new tile
                m_tiles[x, z] = instance;

                // Initialize the tile
                BaseTile tile = instance.GetComponent<BaseTile>();
                if(tile != null) {
                    tile.Init(this, x, z);
                }
            }
        }
    }

    /// <summary>
    /// Invokes the setup logic for all the tiles with the tags specified in <see cref="m_setupOrder"/>
    /// in the order as they appear on the list. This is done to ensure that tiles are setup in the order
    /// required in order for their setup logic to work correctly.
    /// This may be slow but it expected to only be execute manually from within the editor 
    /// </summary>
    void SetupTiles()
    {
        // First we need all the ramps to be setup as certain rely on their orientation
        // to know how to setup 
        foreach (string tag in m_setupOrder) {
            foreach(GameObject go in GameObject.FindGameObjectsWithTag(tag)) {
                BaseTile tile = go.GetComponent<BaseTile>();
                if(tile != null) {
                    tile.Setup();
                }
            }
        }

        //for (int x = 0; x < m_tiles.GetLength(0); x++) {
        //    for (int z = 0; z < m_tiles.GetLength(1); z++) {
        //        GameObject go = m_tiles[x, z];
        //        if(go == null) {
        //            continue;
        //        }

        //        BaseTile tile = go.GetComponent<BaseTile>();
        //        if(tile != null) {
        //            tile.Setup();
        //        }
        //    }
        //}
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

    /// <summary>
    /// Returns the tile gameobject located at the given position should one exist
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public GameObject GetTileAt(Vector3 position)
    {
        GameObject tile = null;
        int x = (int)position.x;
        int z = (int)position.z;

        // Within bounds
        if (x >= 0 && x < m_tiles.GetLength(0) &&
            z >= 0 && z < m_tiles.GetLength(1)) {
            tile = m_tiles[x, z];
        }

        return tile;
    }
}
