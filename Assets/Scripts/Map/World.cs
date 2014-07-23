using UnityEngine;
using System.Collections.Generic;

/**
 * This class provides all the GUI and sounds. This is the main class of the program !
 */
public class World : MonoBehaviour {
    // Singleton
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
    // Display Map ?
    public static bool ShowMap = true;

    // Define current Map
    [HideInInspector] public Map currentMap;
    // Define background Music
    [HideInInspector] public AudioSource currentBGM;
    // Define camera Filter
    [HideInInspector] public Color currentFilter = new Color(0, 0, 0, 0);

    // List of mapObjects on this map to display
                      public List<MapObject> mapObjects = new List<MapObject>();

    // List of others displayable objects -- List them to order their display
    [HideInInspector] public List<IDisplayable> haveToDisplay = new List<IDisplayable>();


    /* Initialize
     */
    public void Start() {
        currentBGM = gameObject.AddComponent<AudioSource>();
        currentMap = new Map(0);

        if (currentBGM.clip != null)
            currentBGM.Play();

        // TMP
        mapObjects = new List<MapObject>(GameObject.FindObjectsOfType<MapObject>());
    }

    /* MonoBehaviour functions
     */
    public void OnGUI() {
        if (!ShowMap)
            return;

        // Display Map
        currentMap.Display();

        // Display Map Objects
        mapObjects.Sort(delegate(MapObject a, MapObject b) { return a.mapCoords.y.CompareTo(b.mapCoords.y); });
        foreach (MapObject mo in mapObjects)
            mo.DisplayOnMap();

        // Display currentFilter
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), InterfaceUtility.ColorToTexture(currentFilter));

        // Display other displayable objects
        foreach (IDisplayable d in haveToDisplay)
            d.Display();
    }
    public void Update() {
        // Loop background music
        if (currentBGM.clip != null && !currentBGM.isPlaying) {
            currentBGM.Stop();
            currentBGM.Play();
        }
    }

    /* Utils
     */
    public bool CanMoveOn(Vector2 _destination) {
        // If no tile -- TODO : Check and Add permissivity on Tiles
        if (currentMap.GetTile(0, (int)_destination.x, (int)_destination.y) == null)
            return false;

        // If there is already an Event on this -- TODO : Check and Add layers on MapObjects
        foreach (MapObject mo in mapObjects)
            if (mo.mapCoords == _destination)
                return false;

        return true;
    }
}
