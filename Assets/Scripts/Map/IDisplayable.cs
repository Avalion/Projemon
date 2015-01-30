using UnityEngine;

/**
 * This abstract class patterns a displayable élement. It will be displayed by the class World.
 */
public abstract class IDisplayable : MonoBehaviour, System.IDisposable {
    public void Awake() {
        World.Current.haveToDisplay.Add(this);
    }

    public void Dispose() {
        World.Current.haveToDisplay.Remove(this);
        Destroy(gameObject);
    }

    public abstract void Display();
}
