using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Preassure Sensitive Button
/// Requires that an object be on top of this object to be marked as active
/// </summary>
public class TriggerButton : ButtonTile
{
    /// <summary>
    /// Material for when the button is on
    /// </summary>
    [SerializeField]
    Material buttonOnMaterial;

    /// <summary>
    /// Mateerial for when the button is no longer active
    /// </summary>
    [SerializeField]
    Material butonOffMaterial;

    protected override void OnButtonPressed()
    {
        base.OnButtonPressed();
        if(this.buttonOnMaterial != null) {
            this.ChangeButtonMaterial(this.buttonOnMaterial);
        }
    }

    protected override void OnButtonReleased()
    {
        base.OnButtonReleased();
        if (this.buttonOnMaterial != null) {
            this.ChangeButtonMaterial(this.butonOffMaterial);
        }
    }

    /// <summary>
    /// Changes the material that represents the button to the given one
    /// </summary>
    /// <param name="buttonMaterial"></param>
    void ChangeButtonMaterial(Material buttonMaterial)
    {
        Material[] materials = this.renderer.materials;
        materials[1] = buttonMaterial;
        this.renderer.materials = materials;
    }
}
