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
        InterfaceUtility.ClearAllCache();

        currentBGM = gameObject.AddComponent<AudioSource>();
        currentMap = new Map(startMapID);

        Player.Current.mapCoords = startPlayerCoords;
        Player.Current.sprite = InterfaceUtility.GetTexture(Config.GetResourcePath(MapObject.IMAGE_FOLDER) + "perso_00.png");

        if (currentBGM.clip != null)
            currentBGM.Play();

        foreach (MapObject mo in currentMap.mapObjects)
            mo.OnStart();
    }

    /* MonoBehaviour functions
     */
    public void OnGUI() {
        if (!ShowMap)
            return;

        // Display Map
        currentMap.Display();

        // Display Map Objects
        List<MapObject> mos = new List<MapObject>(currentMap.mapObjects);
        mos.Add(Player.Current);
        mos.Sort(delegate(MapObject a, MapObject b) { return (a.mapCoords.y + (a.currentMovement.y > 0 ? a.currentMovement.y : 0)).CompareTo(b.mapCoords.y + (b.currentMovement.y > 0 ? b.currentMovement.y : 0)); });
        foreach (MapObject mo in mos)
            mo.DisplayOnMap();

        // Display currentFilter
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), InterfaceUtility.ColorToTexture(currentFilter));

        // Display other displayable objects
        foreach (IDisplayable d in haveToDisplay)
            d.Display();
    }
    public void Update() {
        #region Music
        if (currentBGM.clip != null && !currentBGM.isPlaying) {
            currentBGM.Stop();
            currentBGM.Play();
        }
        #endregion

        #region Player
#if UNITY_EDITOR
        if (!Player.Current.isMoving && (!Player.Locked || InputManager.Current.GetKey(KeyCode.LeftControl))) {
#else
        if (!Player.Current.isMoving && !Player.Locked) {
#endif
            if (InputManager.Current.GetKey(KeyCode.LeftArrow))
                Player.Current.Move(MapObject.PossibleMovement.Left);
            else if (InputManager.Current.GetKey(KeyCode.RightArrow))
                Player.Current.Move(MapObject.PossibleMovement.Right);
            else if (InputManager.Current.GetKey(KeyCode.UpArrow))
                Player.Current.Move(MapObject.PossibleMovement.Up);
            else if (InputManager.Current.GetKey(KeyCode.DownArrow))
                Player.Current.Move(MapObject.PossibleMovement.Down);
        }
        #endregion

        #region MapObjects
        foreach (MapObject mo in currentMap.mapObjects)
            mo.OnUpdate();

        Player.Current.OnUpdate();
        #endregion
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
        currentMap = null;

        Resources.UnloadUnusedAssets();
        InterfaceUtility.ClearAllCache();

        currentMap = new Map(mapId);
    }

    
    /* MapObject Actions
     */
    public delegate void ExecOnEnd();

    public void ExecuteActions(MapObjectAction[] moa, ExecOnEnd _action) {
        StartCoroutine(ExecuteActionAsync(moa, _action));
    }
    private IEnumerator ExecuteActionAsync(MapObjectAction[] moa, ExecOnEnd _action) {
        foreach (MapObjectAction action in moa) {
            action.Execute();
            while (!action.Done())
                yield return new WaitForEndOfFrame();
        }
        foreach (MapObjectAction action in moa)
            action.Init();
        
        _action();
    }

    public void ExecuteActions(MapObject mo, bool _lock) {
        mo.isRunning = true;
        if (_lock)
            Player.Lock();

        StartCoroutine(ExecuteActionAsync(mo, _lock));
    }
    private IEnumerator ExecuteActionAsync(MapObject mo, bool _lock) {
        foreach (MapObjectAction action in mo.actions) {
            action.Execute();
            while (!action.Done())
                yield return new WaitForEndOfFrame();
        }
        if (_lock)
            Player.Unlock();
        foreach (MapObjectAction action in mo.actions)
            action.Init();

        mo.isRunning = false;
    }

    public void ExecuteActions(MapObject mo, bool _lock, ExecOnEnd _action) {
        mo.isRunning = true;
        if (_lock)
            Player.Lock();

        StartCoroutine(ExecuteActionAsync(mo, _lock, _action));
    }
    private IEnumerator ExecuteActionAsync(MapObject mo, bool _lock, ExecOnEnd _action) {
        foreach (MapObjectAction action in mo.actions) {
            action.Execute();
            while (!action.Done())
                yield return new WaitForEndOfFrame();
        }
        if (_lock)
            Player.Unlock();
        foreach (MapObjectAction action in mo.actions)
            action.Init();

        mo.isRunning = false;

        _action();
    }

    /* Utils
     */
    public MapObject GetMapObjectById(int _mapObjectId) {
        return currentMap.mapObjects.Find(MO => MO.mapObjectId == _mapObjectId);
    } 
    
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
