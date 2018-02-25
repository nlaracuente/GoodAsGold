using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Coin))]
public class CoinEditor : Editor
{
    Coin m_coin;

    #if (UNITY_EDITOR)
    void OnEnable()
    {
        m_coin = (Coin)target;
    }

    /// <summary>
    /// By re-assigning state to the current state
    /// we trigger the coin's visual update to represent the current state
    /// </summary>
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        m_coin.CoinState = m_coin.CoinState;
    }
    #endif
}
