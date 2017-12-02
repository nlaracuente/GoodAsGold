using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A previous explorer consumed by the cursed and bound to be a 
/// statue for the rest of eternety
/// 
/// Can be pushed/pulled by the player to trigger buttons, etc.
/// </summary>
[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class Statue : MonoBehaviour, IMoveable
{
    /// <summary>
    /// How far to cast the ray
    /// </summary>
    [SerializeField]
    float rayDistance = 1f;

    /// <summary>
    /// The layer where the player is on
    /// </summary>
    [SerializeField]
    LayerMask playerLayer;

    /// <summary>
    /// A reference to the rigidbody component
    /// </summary>
    new Rigidbody rigidbody;

    /// <summary>
    /// A reference to the collider component
    /// </summary>
    new Collider collider;

    /// <summary>
    /// Contains all the directions we want to cast a ray
    /// </summary>
    Dictionary<string, Vector3> directions = new Dictionary<string, Vector3>() {
        {"forward", Vector3.forward },
        {"left", Vector3.left },
        {"back", Vector3.back},
        {"right", Vector3.right},
    };

    /// <summary>
    /// True when the player is actively moving this statue
    /// </summary>
    bool isBeingPulled = false;
    public bool IsBeingPulled
    {
        set { this.isBeingPulled = true; }
    }

    /// <summary>
    /// Assigns references
    /// </summary>
    void Start()
    {
        this.rigidbody = GetComponent<Rigidbody>();
        this.collider = GetComponent<Collider>();
    }

    /// <summary>
    /// Returns the name of the direction from which the player interacted with the statue
    /// </summary>
    /// <param name="playerRigidBody"></param>
    public string GetInteractionDirectionName(Rigidbody playerRigidBody)
    {
        string fromDirection = "";
        Vector3 origin = this.rigidbody.position;

        foreach (KeyValuePair<string, Vector3> dirInfo in this.directions) {
            string dirName = dirInfo.Key;
            Vector3 direction = dirInfo.Value;

            // Show a line in the editor to see when we are calculating for attraction
            Debug.DrawLine(origin, origin + direction * this.rayDistance, Color.red);

            Ray ray = new Ray(origin, direction);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, this.rayDistance, this.playerLayer)) {
                fromDirection = dirName;
                break;
            }
        }

        return fromDirection;
    }

}
