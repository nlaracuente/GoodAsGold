using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// All the directions an object could be facing
/// </summary>
public enum FacingDirection
{
    Up,
    Left,
    Down,
    Right,
}

public class Utility
{
    /// <summary>
    /// Vector3 representations of the FacingDirections
    /// </summary>
    public static Dictionary<FacingDirection, Vector3> m_directionVector = new Dictionary<FacingDirection, Vector3>()
    {
        { FacingDirection.Up, Vector3.forward },
        { FacingDirection.Left, Vector3.left },
        { FacingDirection.Down, Vector3.back },
        { FacingDirection.Right, Vector3.right },
    };

    /// <summary>
    /// Returns the direction the transform is facing based on its forward vector
    /// </summary>
    /// <param name="transform"></param>
    /// <returns></returns>
    public static FacingDirection GetFacingDirection(Transform transform)
    {
        Vector3 directionVector = new Vector3(
            Mathf.Round(transform.forward.x),
            0f,
            Mathf.Round(transform.forward.z)
        );

        return m_directionVector.FirstOrDefault(d => d.Value == directionVector).Key;
    }

    /// <summary>
    /// Returns the Vector3 value associated with the given direction name
    /// </summary>
    /// <param name="dirName"></param>
    /// <returns></returns>
    public static Vector3 GetDirectionVectorByName(FacingDirection dirName)
    {
        return m_directionVector.FirstOrDefault(d => d.Key == dirName).Value;
    }
}
