using UnityEngine;

public abstract class IDisplayable : MonoBehaviour {
    public void Awake() {
        World.Current.haveToDisplay.Add(this);
    }

    public void Dispose() {
        World.Current.haveToDisplay.Remove(this);
        Destroy(gameObject);
    }

    public abstract void Display();
}
