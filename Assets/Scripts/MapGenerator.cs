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
    /// A reference to the image resource that contains all the static tiles:
    /// i.e. walls, floors
    /// for the current level
    /// </summary>
    [SerializeField]
    Texture2D m_tileTextureMap;

    /// <summary>
    /// A reference to the image resource that contains all the objects that sit on tiles:
    /// i.e ramps, doors, buttons, coins
    /// for the current level
    /// </summary>
    [SerializeField]
    Texture2D m_objectsTextureMap;

    /// <summary>
    /// Stores the tile definitions in a hash table for quick reference
    /// </summary>
    Dictionary<Color32, GameObject> m_definitionTable = new Dictionary<Color32, GameObject>();

    /// <summary>
    /// A collection of all the spawned tiles based on their x,z position
    /// </summary>
    GameObject[,] m_tiles;

    /// <summary>
    /// A collection of all the spawned objects based on their x,z position
    /// </summary>
    GameObject[,] m_objects;

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
        SetupContainers();

        // Spawn first all the tiles followed by all the objects
        SpawnTiles(m_tileTextureMap);
        SpawnTiles(m_objectsTextureMap, true);

        SetupTiles();
    }

    /// <summary>
    /// Destroys all instantiates tile prefabs
    /// </summary>
    void ClearMap()
    {
        m_tiles = null;
        m_objects = null;

        // Pickup stragglers or anything we lost reference to
        foreach (Transform child in transform) {
            DestroyImmediate(child.gameObject);
        }
    }

    /// <summary>
    /// Stores the texture map data from the resources folder when one is not already set
    /// </summary>
    void SetTextureMap()
    {
        if (m_tileTextureMap == null) {
            string tilesPath = string.Format("{0}/{1}", m_resourcePath, SceneManager.GetActiveScene().name + "_tiles");
            m_tileTextureMap = Resources.Load<Texture2D>(tilesPath);
        }

        if (m_objectsTextureMap == null) {
            string objectsPath = string.Format("{0}/{1}", m_resourcePath, SceneManager.GetActiveScene().name + "_objects");
            m_objectsTextureMap = Resources.Load<Texture2D>(objectsPath);
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
    /// Creates the array containers for the tiles and the objects that are on the tiles
    /// </summary>
    void SetupContainers()
    {
        m_tiles = new GameObject[m_tileTextureMap.width, m_tileTextureMap.height];
        m_objects = new GameObject[m_objectsTextureMap.width, m_objectsTextureMap.height];
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
    /// Spawns all the recognized tiles/objects from the given texture map
    /// </summary>
    /// <param name="textureMap"></param>
    void SpawnTiles(Texture2D textureMap, bool isForObjects = false)
    {
        float x_offset = (textureMap.width * GameManager.tileXSize) / 2;
        float z_offset = (textureMap.height * GameManager.tileZSize) / 2;

        for (int x = 0; x < textureMap.width; x++) {
            for (int z = 0; z < textureMap.height; z++) {
                Color32 colorId = textureMap.GetPixel(x, z);
                GameObject prefab = GetPrefabByColorId(colorId);

                if(prefab == null) {
                    continue;
                }

                string instanceName = string.Format("{0}_{1}_{2}", prefab.name, z, x);
                string parentName = string.Format("_{0}s", prefab.name);

                Vector3 position = new Vector3(x * GameManager.tileXSize, 0f, z * GameManager.tileZSize);
                position.x -= x_offset;
                position.z -= z_offset;
                GameObject instance = Instantiate(prefab, position, Quaternion.identity);

                // MAJOR ERROR!
                if(instance == null) {
                    Debug.LogErrorFormat("MapGenerator SpanwTiles Error!: Failed to retrieved spanwed prefab for '{0}'", prefab.name);
                    continue;
                }

                // If this is a duplicate we need to remove the newly created instance
                // and update the existing instance as the tiles around it may have changed
                // in addition to re-establishing a reference to the existing
                GameObject duplicate = GameObject.Find(instanceName);
                if (duplicate != null) {
                    DestroyImmediate(instance);
                    instance = duplicate;
                } else {
                    SetInstanceParent(instance, parentName);
                }

                // Update the instance's name
                instance.name = instanceName;

                // Save the spawned prefab on the approiate array
                if (isForObjects) {
                    m_objects[x, z] = instance;
                } else {
                    m_tiles[x, z] = instance;
                }

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
            try {
                foreach (GameObject go in GameObject.FindGameObjectsWithTag(tag)) {
                    BaseTile tile = go.GetComponent<BaseTile>();
                    if (tile != null) {
                        tile.Setup();
                    }
                }
            } catch (UnityException exception) {
                Debug.LogWarningFormat("MapGenerator::SetupTiles() Error! Exception Message = '{0}'", exception.Message);
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

    /// <summary>
    /// Returns the Tile located at the given position should one exist
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public GameObject GetTileAt(Vector3 position)
    {
        return GetGameObjectAt(position, m_tiles);
    }

    /// <summary>
    /// Returns the Object located at the given position should one exist
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public GameObject GetObjectAt(Vector3 position)
    {
        return GetGameObjectAt(position, m_objects);
    }

    /// <summary>
    /// Returns the gameobject located at the give position within the given container
    /// making sure the given position is within bounds
    /// </summary>
    /// <param name="position"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    GameObject GetGameObjectAt(Vector3 position, GameObject[,] container)
    {
        GameObject go = null;
        int x = (int)position.x;
        int z = (int)position.z;

        // Within bounds
        if (x >= 0 && x < container.GetLength(0) &&
            z >= 0 && z < container.GetLength(1)) {
            go = container[x, z];
        }

        return go;
    }

    /// <summary>
    /// Updates the given position object reference with the new object given
    /// </summary>
    /// <param name="position"></param>
    /// <param name="newObject"></param>
    bool SetObjectAt(Vector3 position, GameObject newObject)
    {
        bool wasUpdated = false;

        int x = (int)position.x;
        int z = (int)position.z;

        if (x >= 0 && x < m_objects.GetLength(0) &&
            z >= 0 && z < m_objects.GetLength(1)) {
            m_objects[x, z] = newObject;
            wasUpdated = true;
        }

        return wasUpdated;
    }

    /// <summary>
    /// Attempts to updates the position of the object in the current position to the new position
    /// If there is no object at the current position or the new position is invalid 
    /// then the update fails and no changes are made
    /// </summary>
    /// <param name="currentPosition"></param>
    /// <param name="newPosition"></param>
    public void UpdateObjectPosition(Vector3 currentPosition, Vector3 newPosition)
    {
        GameObject objectGO = GetTileAt(currentPosition);

        // If the new reference can be set then we can proceed
        if(objectGO != null && SetObjectAt(newPosition, objectGO)) {
            SetObjectAt(currentPosition, null);
        }
    }
}
