using UnityEngine;
using System.Collections.Generic;
using System.Collections;

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

    // Init Var
    public int startMapID;

    public Vector2 startPlayerCoords;

    // Define current Map
    [HideInInspector] public Map currentMap;
    // Define background Music
    [HideInInspector] public AudioSource currentBGM;
    // Define camera Filter
    [HideInInspector] public Color currentFilter = new Color(0, 0, 0, 0);

    // List of others displayable objects -- List them to order their display
    [HideInInspector] public List<IDisplayable> haveToDisplay = new List<IDisplayable>();


    /* Initialize
     */
    public void Awake() {
        if (!DataBase.IsConnected) DataBase.Connect(Application.dataPath + "/database.sql");
    }

    public void Start() {
        currentBGM = gameObject.AddComponent<AudioSource>();
        currentMap = new Map(startMapID);

        Player.Current.mapCoords = startPlayerCoords;

        if (currentBGM.clip != null)
            currentBGM.Play();
    }

    /* MonoBehaviour functions
     */
    public void OnGUI() {
        if (!ShowMap)
            return;

        // Display Map
        currentMap.Display();

        // Display Map Objects
        currentMap.mapObjects.Sort(delegate(MapObject a, MapObject b) { return a.mapCoords.y.CompareTo(b.mapCoords.y); });
        foreach (MapObject mo in currentMap.mapObjects)
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

        // Player movements
        if (!Player.Current.isMoving && !Player.Locked) {
            if (InputManager.Current.GetKey(KeyCode.LeftArrow))
                Player.Current.Move(MapObject.PossibleMovement.Left);
            else if (InputManager.Current.GetKey(KeyCode.RightArrow))
                Player.Current.Move(MapObject.PossibleMovement.Right);
            else if (InputManager.Current.GetKey(KeyCode.UpArrow))
                Player.Current.Move(MapObject.PossibleMovement.Up);
            else if (InputManager.Current.GetKey(KeyCode.DownArrow))
                Player.Current.Move(MapObject.PossibleMovement.Down);
        }
    }

    public void OnDestroy() {
        DataBase.Close();
    }
    public void OnApplicationQuit() {
        DataBase.Close();
    }

    /* Load
     */
    public void LoadMap(int mapId) {
        currentMap = new Map(mapId);
    }

    
    /* MapObject Actions
     */
    public virtual void ExecuteActions(MapObject mo) {
        mo.isRunning = true;
        //Player.Lock(); TEMP TODO: add bool

        StartCoroutine(ExecuteActionAsync(mo));
    }

    private IEnumerator ExecuteActionAsync(MapObject mo) {
        foreach (MapObjectAction action in mo.actions) {
            action.Execute();
            while (!action.Done())
                yield return new WaitForEndOfFrame();
        }
        //Player.Unlock(); TEMP TODO: add bool
        foreach (MapObjectAction action in mo.actions)
            action.Init();

        mo.isRunning = false;
    }

    /* Utils
     */
    public bool CanMoveOn(MapObject o, Vector2 _destination) {
        if (!currentMap.collisions[(int)_destination.x, (int)_destination.y])
            return false;

        if (currentMap.GetTile(0, (int)_destination.x, (int)_destination.y) == null)
            return false;

        // If there is already an Event on this
        foreach (MapObject mo in currentMap.mapObjects) {
            if (mo.mapCoords == _destination) {
                o.OnCollision();
                mo.OnCollision();

                if (mo.layer != o.layer || mo.allowPassThrough)
                    continue;
                return false;
            }
            if (mo.mapCoords + mo.currentMovement == _destination) {
                o.OnCollision();
                mo.OnCollision();

                if (mo.layer != o.layer || mo.allowPassThrough)
                    continue;
                return false;
            }
        }

        return true;
    }

}
