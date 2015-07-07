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

    public ImageRessources InterfaceResources = new ImageRessources();



    // Init Var
    [HideInInspector] public int startMapID;
    [HideInInspector] public Vector2 startPlayerCoords;

    // Define current Map
    [HideInInspector] public Map currentMap;
    // Define background Music
    public AudioClip BGM {
        set {
            currentBGM.Stop();
            currentBGM.clip = value;
            if (value != null)
                currentBGM.Play();
        }
        get {
            if (currentBGM == null)
                return null;
            return currentBGM.clip;
        }
    }
    // Define background Music
    public AudioClip BGS {
        set {
            currentBGS.Stop();
            currentBGS.clip = value;
            if (value != null)
                currentBGS.Play();
        }
        get {
            if (currentBGS == null)
                return null;
            return currentBGS.clip;
        }
    }
    
    private AudioSource currentBGM = null;
    private AudioSource currentBGS = null;

    // Define camera Filter
    [HideInInspector] public Color currentFilter = new Color(0, 0, 0, 0);

    // List of others displayable objects -- List them to order their display
    [HideInInspector] public List<IDisplayable> haveToDisplay = new List<IDisplayable>();


    // DisplayOptions
    [HideInInspector] public Vector2 m_coordsOffset = Vector2.zero;
    [HideInInspector] public Vector2 m_scrolling = Vector2.zero;
    


    /* Initialize
     */
    public void Awake() {
        GameData.Load("");
    }

    public void Start() {
        InterfaceUtility.ClearAllCache();

        currentBGM = gameObject.AddComponent<AudioSource>();
        currentBGS = gameObject.AddComponent<AudioSource>();

        LoadMap(startMapID, startPlayerCoords);

        Player.Current.mapCoords = startPlayerCoords;

        // TEMPORARY
        Player.Current.sprite = InterfaceUtility.GetTexture(Config.GetResourcePath(MapObject.IMAGE_FOLDER) + "perso_00.png");

        if (currentBGM.clip != null)
            currentBGM.Play();
        if (currentBGS.clip != null)
            currentBGS.Play();

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
        haveToDisplay.Sort(delegate(IDisplayable a, IDisplayable b) { return a.m_layer.CompareTo(b.m_layer); });
        foreach (IDisplayable d in haveToDisplay)
            d.Display();
    }
    public void Update() {
        #region Music
        if (currentBGM.clip != null && !currentBGM.isPlaying) {
            currentBGM.Stop();
            currentBGM.Play();
        }
        if (currentBGS.clip != null && !currentBGS.isPlaying) {
            currentBGS.Stop();
            currentBGS.Play();
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
    public void LateUpdate() {
        #region Map
        Vector2 destination = Player.Current.mapCoords + Player.Current.currentMovement;
        if (m_coordsOffset.x + m_scrolling.x > 0 && destination.x <= m_coordsOffset.x + m_scrolling.x + 2) m_scrolling.x += -1;
        if (m_coordsOffset.y + m_scrolling.y > 0 && destination.y <= m_coordsOffset.y + m_scrolling.y + 2) m_scrolling.y += -1;
        if (m_coordsOffset.x + m_scrolling.x < currentMap.Size.x - Map.MAP_SCREEN_X && destination.x >= m_coordsOffset.x + m_scrolling.x + Map.MAP_SCREEN_X - 3) m_scrolling.x += 1;
        if (m_coordsOffset.y + m_scrolling.y < currentMap.Size.y - Map.MAP_SCREEN_Y && destination.y >= m_coordsOffset.y + m_scrolling.y + Map.MAP_SCREEN_Y - 3) m_scrolling.y += 1;

        if (m_scrolling != Vector2.zero && !Player.Current.isMoving) {
            m_coordsOffset += m_scrolling;
            m_scrolling = Vector2.zero;
            currentMap.UpdateVisibleList(m_coordsOffset);
        }
        #endregion
    }

    public void OnDestroy() {
        GameData.Dispose();
    }
    public void OnApplicationQuit() {
        GameData.Dispose();
    }

    /* Load
     */
    public void LoadMap(int mapId) {
        LoadMap(mapId, Vector2.zero);
    }
    public void LoadMap(int mapId, Vector2 _arrival) {
        if (currentMap != null) {
            currentMap.Dispose();
            currentMap = null;
        }

        Resources.UnloadUnusedAssets();
        InterfaceUtility.ClearAllCache();

        currentMap = new Map(mapId);
        currentMap.Load();

        m_scrolling = Vector2.zero;
        while (m_coordsOffset.x > 0 && _arrival.x <= m_coordsOffset.x + 2) m_coordsOffset.x += -1;
        while (m_coordsOffset.y > 0 && _arrival.y <= m_coordsOffset.y + 2) m_coordsOffset.y += -1;
        while (m_coordsOffset.x < currentMap.Size.x - Map.MAP_SCREEN_X && _arrival.x >= m_coordsOffset.x + Map.MAP_SCREEN_X - 3) m_coordsOffset.x += 1;
        while (m_coordsOffset.y < currentMap.Size.y - Map.MAP_SCREEN_Y && _arrival.y >= m_coordsOffset.y + Map.MAP_SCREEN_Y - 3) m_coordsOffset.y += 1;
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
        if (_mapObjectId == -1)
            return Player.Current;
        return currentMap.mapObjects.Find(MO => MO.mapObjectId == _mapObjectId);
    } 
    
    public bool CanMoveOn(MapObject o, Vector2 _destination) {
        if (!currentMap.collisions[(int)_destination.x, (int)_destination.y])
            return false;

        if (currentMap.GetTile(0, (int)_destination.x, (int)_destination.y) == null)
            return false;
        
        List<MapObject> mapObjects = new List<MapObject>(currentMap.mapObjects);
        mapObjects.Add(Player.Current);
        if (mapObjects.Contains(o)) mapObjects.Remove(o);

        // If there is already an Event on this
        foreach (MapObject mo in mapObjects) {
            if (mo.mapCoords == _destination ||
                mo.mapCoords + mo.currentMovement == _destination
                ) {

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
