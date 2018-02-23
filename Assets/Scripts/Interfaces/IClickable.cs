
/// <summary>
/// Interface for all objects that can be clicked on
/// </summary>
public interface IClickable
{
    /// <summary>
    /// Returns TRUE if the object is in a state where it can be clicked
    /// </summary>
    /// <returns></returns>
    bool IsClickable();

    /// <summary>
    /// Executes on click logic
    /// </summary>
    void OnClick();

    /// <summary>
    /// Triggered when the object is not longer being interacted by the player
    /// </summary>
    void OnLoseFocus();
}
