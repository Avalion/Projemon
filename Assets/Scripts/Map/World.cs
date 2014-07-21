using UnityEngine;
using System.Collections.Generic;

public class World : MonoBehaviour {
    public Map currentMap;

    private static World current = null;
    public static World Current {
        get {
            if (current == null) {
                current = GameObject.FindObjectOfType<World>();
                if (current == null) {
                    current = new GameObject("World").AddComponent<World>();
                }
            }
            return current;
        }
    }

    public List<MapObject> mapObjects = new List<MapObject>();

    public static bool Show = true;

    [HideInInspector]
    public List<IDisplayable> haveToDisplay = new List<IDisplayable>();



    public void Start() {
        currentMap = new Map(0);

        mapObjects = new List<MapObject>(GameObject.FindObjectsOfType<MapObject>());
    }

    public void OnGUI() {
        if (!Show)
            return;

        currentMap.Display();

        mapObjects.Sort(delegate(MapObject a, MapObject b) { return a.mapCoords.y.CompareTo(b.mapCoords.y); });

        foreach (MapObject mo in mapObjects)
            mo.DisplayOnMap();

        foreach (IDisplayable d in haveToDisplay) {
            d.Display();
        }
    }

    public bool CanMoveOn(Vector2 _destination) {
        if (currentMap.GetTile(0, (int)_destination.x, (int)_destination.y) == null)
            return false;

        foreach (MapObject mo in mapObjects)
            if (mo.mapCoords == _destination)
                return false;

        return true;
    }
}
