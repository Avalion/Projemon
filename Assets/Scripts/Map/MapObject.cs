using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/**
 * This class defines map objects
 */
public class MapObject : MonoBehaviour {
    public enum MovementSpeed { Slow, Normal, Fast, Instant };
    public MovementSpeed speed = MovementSpeed.Normal;

    public enum Orientation { Down, Left, Right, Up };
    public Orientation orientation = Orientation.Down;

    public enum PossibleMovement { NULL, Left, Right, Up, Down, Forward, Backward, StrafeLeft, StrafeRight, TurnLeft, TurnRight, TurnUp, TurnDown, TurnLeftward, TurnRightward, TurnBackward, FollowPlayer, FleePlayer, LookPlayer }

    public enum ExecutionCondition { NULL, Action, ActionFace, Contact, Automatique }
    public ExecutionCondition execCondition = ExecutionCondition.Action;

    public int CHAR_RESOLUTION_X = 32;
    public int CHAR_RESOLUTION_Y = 48;

    public Texture2D sprite;
    public Texture2D Sprite { 
        get {
            int step = 1;
            if (lerp > 0.2f && lerp < 0.8f)
                step = ((orientation == Orientation.Left || orientation == Orientation.Right ? mapCoords.x : mapCoords.y) % 2 == 0) ? 0 : 2;
            

            switch (orientation) {
                case Orientation.Left:
                    return InterfaceUtility.SeparateTexture(sprite, step, 1, CHAR_RESOLUTION_X, CHAR_RESOLUTION_Y);
                case Orientation.Right:
                    return InterfaceUtility.SeparateTexture(sprite, step, 2, CHAR_RESOLUTION_X, CHAR_RESOLUTION_Y);
                case Orientation.Up:
                    return InterfaceUtility.SeparateTexture(sprite, step, 3, CHAR_RESOLUTION_X, CHAR_RESOLUTION_Y);
                default:
                    return InterfaceUtility.SeparateTexture(sprite, step, 0, CHAR_RESOLUTION_X, CHAR_RESOLUTION_Y);
            }
        }
    }

    public Vector2 mapCoords;

    [HideInInspector] public bool isMoving;
    [HideInInspector] public Vector2 currentMovement;
    protected float lerp = 0;

    public List<MapObjectAction> actions = new List<MapObjectAction>();
    public bool isRunning = false;


    public void Start() {
        if (execCondition == ExecutionCondition.Automatique)
            ExecuteActions();

        OnStart();
    }
    public virtual void OnStart() {}

    /* Functions
     */
    public void Update() {
        if (isMoving) {
            switch (speed) {
                case MovementSpeed.Slow: lerp += Time.deltaTime * 2f; break;
                case MovementSpeed.Normal: lerp += Time.deltaTime * 4f; break;
                case MovementSpeed.Fast: lerp += Time.deltaTime * 6f; break;
                case MovementSpeed.Instant: lerp = 1f; break;
            }
        }
        
        if (lerp >= 1) {
            lerp = 0;
            mapCoords += currentMovement;
            currentMovement = Vector2.zero;
            isMoving = false;
        }

        if (HaveToExecute(execCondition))
            ExecuteActions();

        OnUpdate();
    }
    public virtual void OnUpdate() {}

    public void DisplayOnMap() {
        if (sprite != null) {
            float height = Sprite.height * (Map.Resolution.x / Sprite.width);
            GUI.DrawTexture(new Rect(Map.Resolution.x * (mapCoords.x + currentMovement.x * lerp), Map.Resolution.y * (mapCoords.y + currentMovement.y * lerp + 1) - height, Map.Resolution.x, height), Sprite);
        }
    }

    public void Move(PossibleMovement _o) {
        if (isMoving)
            return;
        
        isMoving = true;

        Vector2 move = new Vector2();

        switch (_o) {
            case PossibleMovement.Left:
                move = new Vector2(-1, 0);
                orientation = Orientation.Left;
                break;
            case PossibleMovement.TurnLeft:
                orientation = Orientation.Left;
                break;

            case PossibleMovement.Right:
                move = new Vector2(1, 0);
                orientation = Orientation.Right;
                break;
            case PossibleMovement.TurnRight:
                orientation = Orientation.Right;
                break;

            case PossibleMovement.Up:
                move = new Vector2(0, -1);
                orientation = Orientation.Up;
                break;
            case PossibleMovement.TurnUp:
                orientation = Orientation.Up;
                break;

            case PossibleMovement.Down:
                move = new Vector2(0, 1);
                orientation = Orientation.Down;
                break;
            case PossibleMovement.TurnDown:
                orientation = Orientation.Down;
                break;

            case PossibleMovement.Forward:
                move = new Vector2(orientation == Orientation.Left ? -1 : orientation == Orientation.Right ? 1 : 0, orientation == Orientation.Up ? -1 : orientation == Orientation.Down ? 1 : 0);
                break;
            case PossibleMovement.Backward:
                move = new Vector2(orientation == Orientation.Left ? 1 : orientation == Orientation.Right ? -1 : 0, orientation == Orientation.Up ? 1 : orientation == Orientation.Down ? -1 : 0);
                break;

            case PossibleMovement.StrafeLeft:
                move = new Vector2(orientation == Orientation.Up ? -1 : orientation == Orientation.Down ? 1 : 0, orientation == Orientation.Right ? -1 : orientation == Orientation.Left ? 1 : 0);
                break;
            case PossibleMovement.StrafeRight:
                move = new Vector2(orientation == Orientation.Up ? 1 : orientation == Orientation.Down ? -1 : 0, orientation == Orientation.Right ? 1 : orientation == Orientation.Left ? -1 : 0);
                break;

            case PossibleMovement.TurnLeftward:
                orientation = orientation == Orientation.Up ? Orientation.Left : orientation == Orientation.Left ? Orientation.Down : orientation == Orientation.Down ? Orientation.Right : Orientation.Up;
                break;
            case PossibleMovement.TurnRightward:
                orientation = orientation == Orientation.Up ? Orientation.Right : orientation == Orientation.Right ? Orientation.Down : orientation == Orientation.Down ? Orientation.Left : Orientation.Up;
                break;
            case PossibleMovement.TurnBackward:
                orientation = orientation == Orientation.Up ? Orientation.Down : orientation == Orientation.Down ? Orientation.Up : orientation == Orientation.Left ? Orientation.Right : Orientation.Left;
                break;
            
            case PossibleMovement.FollowPlayer:
                Debug.LogWarning("FollowPlayer not implemented yet.");
                break;
            case PossibleMovement.FleePlayer:
                Debug.LogWarning("FleePlayer not implemented yet.");
                break;
            case PossibleMovement.LookPlayer:
                Debug.LogWarning("LookPlayer not implemented yet.");
                break;
            default :
                break;
        }

        if (World.Current.CanMoveOn(this, mapCoords + move))
            currentMovement = move;
        else
            isMoving = false;
    }

    public void OnCollision() {
        if (execCondition == ExecutionCondition.Contact)
            ExecuteActions();
    }

    protected bool HaveToExecute(ExecutionCondition cond) {
        if (isRunning)
            return false;
        
        switch (cond) {
            case ExecutionCondition.Action:
                return InputManager.Current.GetKeyDown(KeyCode.Return) && (
                    Player.Current.orientation == Orientation.Down  && Player.Current.mapCoords == mapCoords + new Vector2 (0,-1) ||
                    Player.Current.orientation == Orientation.Up    && Player.Current.mapCoords == mapCoords + new Vector2 (0, 1) ||
                    Player.Current.orientation == Orientation.Left  && Player.Current.mapCoords == mapCoords + new Vector2 ( 1,0) ||
                    Player.Current.orientation == Orientation.Right && Player.Current.mapCoords == mapCoords + new Vector2 (-1,0));
            case ExecutionCondition.ActionFace:
                return InputManager.Current.GetKeyDown(KeyCode.Return) && (
                    Player.Current.orientation == Orientation.Down  && orientation == Orientation.Up    && Player.Current.mapCoords == mapCoords + new Vector2(0,-1) ||
                    Player.Current.orientation == Orientation.Up    && orientation == Orientation.Down  && Player.Current.mapCoords == mapCoords + new Vector2(0, 1) ||
                    Player.Current.orientation == Orientation.Left  && orientation == Orientation.Right && Player.Current.mapCoords == mapCoords + new Vector2( 1,0) ||
                    Player.Current.orientation == Orientation.Right && orientation == Orientation.Left  && Player.Current.mapCoords == mapCoords + new Vector2(-1,0));
        }

        return false;
    }

    public virtual void ExecuteActions() {
        isRunning = true;
        Player.Lock();
        
        StartCoroutine(ExecuteActionAsync());
    }

    private IEnumerator ExecuteActionAsync() {
        foreach(MapObjectAction action in actions) {
            action.Execute();
            while (!action.Done())
                yield return new WaitForEndOfFrame();
        }
        Player.Unlock();
        foreach (MapObjectAction action in actions)
            action.Init();
        isRunning = false;
    }

}
