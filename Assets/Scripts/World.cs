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
    [HideInInspector] public Vector2 coordsOffset = Vector2.zero;
    [HideInInspector] public Vector2 scrolling = Vector2.zero;
    

    // Loading Screen
    [HideInInspector] public bool loading = false;


    /* Initialize
     */
    public void Awake() {
        GameData.Load("Saves/Save0.sql");
    }

    public void Start() {
        InterfaceUtility.ClearAllCache();

        currentBGM = gameObject.AddComponent<AudioSource>();
        currentBGS = gameObject.AddComponent<AudioSource>();

        LoadMap(GameData.dbSystem.playerMapID, GameData.dbSystem.playerCoords);
        Player.Current.orientation = GameData.dbSystem.playerOrientation;
        Player.Current.name = GameData.dbSystem.playerName;
        Player.Current.goldCount = GameData.dbSystem.playerGold;

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

        if (loading) {

            return;
        }


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
        if (coordsOffset.x + scrolling.x > 0 && destination.x <= coordsOffset.x + scrolling.x + 2) scrolling.x += -1;
        if (coordsOffset.y + scrolling.y > 0 && destination.y <= coordsOffset.y + scrolling.y + 2) scrolling.y += -1;
        if (coordsOffset.x + scrolling.x < currentMap.Size.x - Map.MAP_SCREEN_X && destination.x >= coordsOffset.x + scrolling.x + Map.MAP_SCREEN_X - 3) scrolling.x += 1;
        if (coordsOffset.y + scrolling.y < currentMap.Size.y - Map.MAP_SCREEN_Y && destination.y >= coordsOffset.y + scrolling.y + Map.MAP_SCREEN_Y - 3) scrolling.y += 1;

        if (scrolling != Vector2.zero && !Player.Current.isMoving) {
            coordsOffset += scrolling;
            scrolling = Vector2.zero;
            currentMap.UpdateVisibleList(coordsOffset);
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

        Player.Current.mapCoords = _arrival;

        scrolling = Vector2.zero;
        while (coordsOffset.x > 0 && _arrival.x <= coordsOffset.x + 2) coordsOffset.x += -1;
        while (coordsOffset.y > 0 && _arrival.y <= coordsOffset.y + 2) coordsOffset.y += -1;
        while (coordsOffset.x < currentMap.Size.x - Map.MAP_SCREEN_X && _arrival.x >= coordsOffset.x + Map.MAP_SCREEN_X - 3) coordsOffset.x += 1;
        while (coordsOffset.y < currentMap.Size.y - Map.MAP_SCREEN_Y && _arrival.y >= coordsOffset.y + Map.MAP_SCREEN_Y - 3) coordsOffset.y += 1;
    }

    
    /* MapObject Actions
     */
    public delegate void ExecOnEnd();

    public void ExecuteActions(List<MapObjectAction> moa, ExecOnEnd _action) {
        StartCoroutine(ExecuteActionAsync(moa, _action));
    }
    private IEnumerator ExecuteActionAsync(List<MapObjectAction> moa, ExecOnEnd _action) {
        for (int index = 0; index < moa.Count; index ++) {
            #region ActionCondition
            if (moa[index].GetType() == typeof(ActionIf)) {
                ActionIf current = (ActionIf)moa[index];
                current.Execute();
                if (!current.isActive) {
                    index = moa.FindIndex(A => A.GetType() == typeof(ConditionElse) && ((ConditionElse)A).parentId == moa[index].actionId) - 1;
                    continue;
                }
            }
            if (moa[index].GetType() == typeof(ConditionElse)) {
                ConditionElse current = (ConditionElse)moa[index];
                if (((ActionIf)moa.Find(A => A.actionId == current.parentId)).isActive) {
                    index = moa.FindIndex(A => A.GetType() == typeof(ConditionEnd) && ((ConditionEnd)A).parentId == current.parentId);
                    continue;
                }
            }
            if (moa[index].GetType() == typeof(ConditionEnd))
                continue;
            #endregion

            moa[index].Execute();
            while (!moa[index].Done())
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
        for (int index = 0; index < mo.actions.Count; index++) {
            #region ActionCondition
            if (mo.actions[index].GetType() == typeof(ActionIf)) {
                ActionIf current = (ActionIf)mo.actions[index];
                current.Execute();
                if (!current.isActive) {
                    index = mo.actions.FindIndex(A => A.GetType() == typeof(ConditionElse) && ((ConditionElse)A).parentId == mo.actions[index].actionId) - 1;
                    continue;
                }
            }
            if (mo.actions[index].GetType() == typeof(ConditionElse)) {
                ConditionElse current = (ConditionElse)mo.actions[index];
                if (((ActionIf)mo.actions.Find(A => A.actionId == current.parentId)).isActive) {
                    index = mo.actions.FindIndex(A => A.GetType() == typeof(ConditionEnd) && ((ConditionEnd)A).parentId == current.parentId);
                    continue;
                }
            }
            if (mo.actions[index].GetType() == typeof(ConditionEnd))
                continue;
            #endregion

            mo.actions[index].Execute();
            while (!mo.actions[index].Done())
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
        for (int index = 0; index < mo.actions.Count; index++) {
            #region ActionCondition
            if (mo.actions[index].GetType() == typeof(ActionIf)) {
                ActionIf current = (ActionIf)mo.actions[index];
                current.Execute();
                if (!current.isActive) {
                    index = mo.actions.FindIndex(A => A.GetType() == typeof(ConditionElse) && ((ConditionElse)A).parentId == mo.actions[index].actionId) - 1;
                    continue;
                }
            }
            if (mo.actions[index].GetType() == typeof(ConditionElse)) {
                ConditionElse current = (ConditionElse)mo.actions[index];
                if (((ActionIf)mo.actions.Find(A => A.actionId == current.parentId)).isActive) {
                    index = mo.actions.FindIndex(A => A.GetType() == typeof(ConditionEnd) && ((ConditionEnd)A).parentId == current.parentId);
                    continue;
                }
            }
            if (mo.actions[index].GetType() == typeof(ConditionEnd))
                continue;
            #endregion

            mo.actions[index].Execute();
            while (!mo.actions[index].Done())
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
