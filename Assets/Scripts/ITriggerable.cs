/// <summary>
/// Interface for all events that need to register to a button's events
/// </summary>
public interface ITriggerable
{
    void OnButtonPressed();
    void OnButtonReleased();
}
