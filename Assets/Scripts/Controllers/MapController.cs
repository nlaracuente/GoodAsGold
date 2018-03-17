using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

/// <summary>
/// Controls the creation of the map and keeping a reference to all the tiles on the map
/// </summary>
public class MapController : MonoBehaviour
{
    /// <summary>
    /// A ference to self
    /// </summary>
    public static MapController instance;

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
    BaseTile[,] m_tileMap;

    /// <summary>
    /// A list of all the tile tags to run the setup logic for in the order required
    /// </summary>
    [SerializeField, Tooltip("Tag names in the order to process the setup logic")]
    List<string> m_setupOrder = new List<string>();

    /// <summary>
    /// Set references
    /// </summary>
    void Awake()
    {
        if (instance == null) {
            instance = this;
        }
    }

    /// <summary>
    /// Re-establish a references to all of the tiles on the map
    /// </summary>
    void Start()
    {
        SetTextureMap();
        SetupTileMapContainer();
        SaveTileReferences();
        SaveObjectReferences();
    }

    /// <summary>
    /// Updates the <see cref="m_tileMap"/> with all existing tiles on the map
    /// </summary>
    void SaveTileReferences()
    {
        foreach (BaseTile tile in FindObjectsOfType<BaseTile>()) {
            // Where on the array to place this tile
            Vector3 index = GetIndexByPosition(tile.gameObject);

            // Save the index and update the array
            tile.Index = index;
            m_tileMap[(int)index.x, (int)index.z] = tile;
        }
    }

    /// <summary>
    /// Finds all objects and stores a ference to them within the tile 
    /// they share space with
    /// </summary>
    void SaveObjectReferences()
    {
        foreach (BaseObject spawnedObject in FindObjectsOfType<BaseObject>()) {
            // Where on the array to place this tile
            Vector3 index = GetIndexByPosition(spawnedObject.gameObject);

            // Save the index and update the array
            spawnedObject.Index = index;
            BaseTile tile = GetTileAt(index);
            tile.ObjectOnTile = spawnedObject;
        }
    }

    /// <summary>
    /// Returns the array index the given game object would be at based on their current position
    /// </summary>
    /// <param name="go"></param>
    /// <returns></returns>
    public Vector3 GetIndexByPosition(GameObject go)
    {
        Vector3 index = Vector3.zero;

        // Avoid division by 0
        if (go.transform.position.x != 0f) {
            index.x = go.transform.position.x / GameManager.tileXSize;
        }

        if (go.transform.position.z != 0f) {
            index.z = go.transform.position.z / GameManager.tileXSize;
        }

        return index;
    }

    /// <summary>
    /// Clears all existing tiles and re-creates the map
    /// </summary>
    public void GenerateMap(bool clearMap = false)
    {
        // Only run in edit mode
        if (!Application.isEditor) {
            return;
        }

        // Because it is in edit mode the instance property is not yet set
        instance = this;

        // This method is 
        if (clearMap) {
            ClearMap();
            ClearMap();
            ClearMap();
        }

        SetTextureMap();
        CreateDefinitionTable();
        SetupTileMapContainer();
        
        SpawnTiles();
        SetupTiles();
    }

    /// <summary>
    /// Destroys all instantiates tile prefabs
    /// </summary>
    void ClearMap()
    {
        m_tileMap = null;

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
    void SetupTileMapContainer()
    {
        m_tileMap = new BaseTile[m_tileTextureMap.width, m_tileTextureMap.height];
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
    /// Note: the tile texture and object texture must have the same dimensions
    /// </summary>
    /// <param name="textureMap"></param>
    void SpawnTiles()
    {
        //float x_offset = (textureMap.width * GameManager.tileXSize) / 2;
        //float z_offset = (textureMap.height * GameManager.tileZSize) / 2;

        for (int x = 0; x < m_tileTextureMap.width; x++) {
            for (int z = 0; z < m_tileTextureMap.height; z++) {
                // Keeps track of where on the array this tile is placed
                Vector3 index = new Vector3(x, 0f, z);

                // We want to spawn both the tile and the object if one exists
                Color32 tileColorId = m_tileTextureMap.GetPixel(x, z);
                Color32 objectColorId = m_objectsTextureMap.GetPixel(x, z);

                GameObject tilePrefab = GetPrefabByColorId(tileColorId);
                GameObject objectPrefab = GetPrefabByColorId(objectColorId);

                GameObject tileInstance = InstantiatePrefab(x, z, tilePrefab);
                GameObject objectInstance = InstantiatePrefab(x, z, objectPrefab);

                // MAJOR ERROR!
                if (tileInstance == null) {
                    Debug.LogErrorFormat("MapController SpanwTiles Error!: Failed to retrieved spanwed prefab for '{0}'", tilePrefab.name);
                    continue;
                }
                
                BaseTile tile = tileInstance.GetComponent<BaseTile>();

                // Save a reference to the tile
                if (tile != null) {
                    m_tileMap[x, z] = tile;
                    tile.Index = index;

                    if(objectInstance != null) {
                        // Set a reference to the object on the tile should there be one
                        BaseObject objectOnTile = objectInstance.GetComponent<BaseObject>();

                        if (objectOnTile != null) {
                            objectOnTile.Index = index;
                            tile.ObjectOnTile = objectOnTile;
                        }
                    }

                } else {
                    Debug.LogWarningFormat("MapController SpawnTiles: Faild to get BaseTile component for {0}", tilePrefab.name);
                }
            }
        }
    }

    /// <summary>
    /// Spawns the given prefab at the given position as long as the prefab is not null
    /// Prefabs have their name updated to math the position where they are at
    /// Prefabs are parented based on their name to keep the hierachy cleaner
    /// As this is expected to be invoked in the Editor, all prefabs are instatiated with
    /// links to the original
    /// </summary>
    /// <param name="x"></param>
    /// <param name="z"></param>
    /// <param name="prefab"></param>
    /// <returns></returns>
    GameObject InstantiatePrefab(int x, int z, GameObject prefab)
    {
        // Can't instantiate what doesn't exist
        if(prefab == null) {
            return null;
        }

        string prefabName = string.Format("{0}_{1}_{2}", prefab.name, x, z);
        string parentName = string.Format("_{0}s", prefab.name);

        GameObject go = null;
        Vector3 position = new Vector3(x * GameManager.tileXSize, 0f, z * GameManager.tileZSize);

        if (Application.isEditor) {
            go = PrefabUtility.InstantiatePrefab(prefab) as GameObject;
            go.transform.position = position;
        } else {
            go = Instantiate(prefab, position, Quaternion.identity);
        }

        // If this is a duplicate we need to remove the newly created instance
        // and update the existing instance as the tiles around it may have changed
        // in addition to re-establishing a reference to the existing
        GameObject duplicate = GameObject.Find(prefabName);
        if (duplicate != null) {
            DestroyImmediate(go);
            go = duplicate;
        } else {
            SetInstanceParent(go, parentName);
        }

        // Update the instance's name
        go.name = prefabName;

       return go;
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
                    ISpawnable tile = go.GetComponent<ISpawnable>();
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
    public BaseTile GetTileAt(Vector3 position)
    {
        BaseTile tile = null;
        int x = (int)position.x;
        int z = (int)position.z;

        // Within bounds
        if (x >= 0 && x < m_tileMap.GetLength(0) &&
            z >= 0 && z < m_tileMap.GetLength(1)) {
            tile = m_tileMap[x, z];
        }

        return tile;
    }

    /// <summary>
    /// Returns the Object located at the given position should one exist
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public BaseObject GetObjectAt(Vector3 position)
    {
        BaseObject objectAt = null;
        BaseTile tile = GetTileAt(position);

        if(tile != null) {
            objectAt = tile.ObjectOnTile;
        }

        return objectAt;
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
    bool SetObjectAt(Vector3 position, BaseObject newObject)
    {
        bool wasUpdated = false;
        BaseTile tile = GetTileAt(position);

        // Make sure the tile doesn't already have an object 
        // so that we don't lose a reference
        if(tile != null && tile.ObjectOnTile == null) {
            tile.ObjectOnTile = newObject;
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
        BaseTile tile = GetTileAt(currentPosition);

        // If the new reference can be set then we can proceed
        if(tile != null && tile.ObjectOnTile != null) {
            SetObjectAt(newPosition, tile.ObjectOnTile);

            // Remove the reference at the current tile
            tile.ObjectOnTile = null;
        }
    }
}
