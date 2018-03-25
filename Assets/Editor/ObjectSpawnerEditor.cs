using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Allows us to create prefabs in the editor view
/// </summary>
public class GameObjectSpawner : MonoBehaviour
{
    /// <summary>
    /// Instantiates a prefab with a link to the original and returns it
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="position"></param>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static GameObject Instantiate(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        GameObject instance = PrefabUtility.InstantiatePrefab(prefab) as GameObject;

        instance.transform.position = position;
        instance.transform.rotation = rotation;

        return instance;
    }
}
