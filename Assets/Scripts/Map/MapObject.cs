using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/**
 * This class defines map objects
 * 
 * TODO : Implements States : int value that determines a new list of Actions if state is setted
 *          - So Add State into DBMapObjectAction only
 *          - Only 4 states so just duplicate the actions list for each state names A B C D
 */
public class MapObject {
    public const string IMAGE_FOLDER = "Characters";

    [HideInInspector]
    public int mapObjectId;

    public string name;

    public enum MovementSpeed { Slow, Normal, Fast, Instant };
    public MovementSpeed speed = MovementSpeed.Normal;

    public enum Orientation { Down, Left, Right, Up };
    public Orientation orientation = Orientation.Down;

    public enum Layer { Under, Middle, Top }
    public Layer layer = Layer.Middle;
    public bool allowPassThrough = false;

    public enum PossibleMovement { NULL, Left, Right, Up, Down, Forward, Backward, StrafeLeft, StrafeRight, TurnLeft, TurnRight, TurnUp, TurnDown, TurnLeftward, TurnRightward, TurnBackward, FollowPlayer, FleePlayer, LookPlayer }

    public enum ExecutionCondition { NULL, Action, ActionFace, Contact, Automatique }
    public ExecutionCondition execCondition = ExecutionCondition.Action;

    public string spritePath = "";
    public Texture2D sprite;
    public Texture2D Sprite { 
        get {
            if (sprite == null) {
                if (spritePath == "")
                    return null;
                sprite = InterfaceUtility.GetTexture(Config.GetResourcePath(IMAGE_FOLDER) + spritePath);
            }
                

            int step = 1;
            if (lerp > 0.2f && lerp < 0.8f)
                step = ((orientation == Orientation.Left || orientation == Orientation.Right ? mapCoords.x : mapCoords.y) % 2 == 0) ? 0 : 2;
            
            // A sprite contains 3 columns and 4 lines
            int CHAR_RESOLUTION_X = sprite.width / 3;
            int CHAR_RESOLUTION_Y = sprite.height / 4;

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
    private Vector2 tempRestrictedCase = new Vector2(-1, -1);

    private bool isJumping = false;

    [HideInInspector] public bool isMoving;
    [HideInInspector] public Vector2 currentMovement;
    protected float lerp = 0;

    public List<MapObjectAction> actions = new List<MapObjectAction>();
    [HideInInspector] 
    public bool isRunning = false;


    public void OnStart() {
        if (execCondition == ExecutionCondition.Automatique)
            // TODO : Add Boolean to lockPlayer while actions
            World.Current.ExecuteActions(this, false);
    }

    /* Functions
     */
    public void OnUpdate() {
        if (isMoving) {
            switch (speed) {
                case MovementSpeed.Slow: lerp += Time.deltaTime * 2f / (isJumping ? 2 : 1); break;
                case MovementSpeed.Normal: lerp += Time.deltaTime * 4f / (isJumping ? 2 : 1); break;
                case MovementSpeed.Fast: lerp += Time.deltaTime * 6f / (isJumping ? 2 : 1); break;
                case MovementSpeed.Instant: lerp = 1f; break;
            }
        }
        
        if (lerp >= 1) {
            lerp = 0;
            mapCoords += currentMovement;
            currentMovement = Vector2.zero;
            isJumping = false;
            isMoving = false;
        }

        if (HaveToExecute())
            // TODO : Add Boolean to lockPlayer while actions
            World.Current.ExecuteActions(this, false);
    }

    public void DisplayOnMap() {
        if (sprite != null) {
            float height = Sprite.height * (Map.Resolution.x / Sprite.width);
            GUI.DrawTexture(new Rect(Map.Resolution.x * (mapCoords.x + currentMovement.x * lerp), Map.Resolution.y * (mapCoords.y + currentMovement.y * lerp + 1) - height + (isJumping ? Mathf.RoundToInt(Mathf.Sin(lerp * Mathf.PI) * Map.Resolution.y) : 0), Map.Resolution.x, height), Sprite);
        }
    }

    public Orientation GetOrientation() {
        return orientation;
    }

    public Orientation GetOrientation(Vector2 _vector){
        return _vector.y == 1 ? Orientation.Down : _vector.y == -1 ? Orientation.Up : _vector.x == 1 ? Orientation.Right : Orientation.Left;
    }

    public void Move(PossibleMovement _o) {
        if (isMoving)
            return;
        
        isMoving = true;

        Vector2 move = new Vector2();
        Vector2 vector = Player.Current.mapCoords - this.mapCoords;

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
                if(vector.magnitude == 1) { orientation = GetOrientation(vector); break; }
                if ((tempRestrictedCase - mapCoords).magnitude > 1) { tempRestrictedCase = new Vector2(-1, -1); }
                if(Mathf.Abs(vector.x) >= Mathf.Abs(vector.y)){
                    move = new Vector2(vector.x/Mathf.Abs(vector.x) , 0);
                    if (!World.Current.CanMoveOn(this, mapCoords + move) || (mapCoords + move == tempRestrictedCase)) {
                        if (vector.y != 0) {
                            move = new Vector2(0, vector.y / Mathf.Abs(vector.y));
                            if (!World.Current.CanMoveOn(this, mapCoords + move) || (mapCoords + move == tempRestrictedCase)) {
                                move = new Vector2(-vector.x / Mathf.Abs(vector.x), 0);
                                if (World.Current.CanMoveOn(this, mapCoords + move) && !(mapCoords + move == tempRestrictedCase)) {
                                    tempRestrictedCase = mapCoords;
                                } else {
                                    move = new Vector2(0, -vector.y / Mathf.Abs(vector.y));
                                    if (World.Current.CanMoveOn(this, mapCoords + move) && !(mapCoords + move == tempRestrictedCase)) {
                                        tempRestrictedCase = mapCoords;
                                    } else {
                                        tempRestrictedCase = new Vector2(-1, -1);
                                        move = Vector2.zero;
                                        orientation = GetOrientation(vector);
                                    }
                                }
                            }
                        } else {
                            move = new Vector2(-vector.x / Mathf.Abs(vector.x), 0);
                            if (World.Current.CanMoveOn(this, mapCoords + move) && !(mapCoords + move == tempRestrictedCase)) {
                                tempRestrictedCase = mapCoords;
                            } else {
                                move = new Vector2(0, this.orientation == Orientation.Down ? 1 : this.orientation == Orientation.Up ? -1 : MathUtility.Alea(new float[] { -1, 1 }));
                                if (World.Current.CanMoveOn(this, mapCoords + move) && !(mapCoords + move == tempRestrictedCase)) {
                                    tempRestrictedCase = mapCoords;
                                } else {
                                    move = new Vector2(0, -move.y);
                                    if (World.Current.CanMoveOn(this, mapCoords + move) && !(mapCoords + move == tempRestrictedCase)) {
                                        tempRestrictedCase = mapCoords;
                                    } else {
                                        tempRestrictedCase = new Vector2(-1, -1);
                                        move = Vector2.zero;
                                        orientation = GetOrientation(vector);
                                    }
                                }
                            }
                        }
                    }
                } else {
                    move = new Vector2(0, vector.y / Mathf.Abs(vector.y));
                    if (!World.Current.CanMoveOn(this, mapCoords + move) || (mapCoords + move == tempRestrictedCase)) {
                        if (vector.x != 0) {
                            move = new Vector2(vector.x / Mathf.Abs(vector.x), 0);
                            if (!World.Current.CanMoveOn(this, mapCoords + move) || (mapCoords + move == tempRestrictedCase)) {
                                move = new Vector2(0, -vector.y / Mathf.Abs(vector.y));
                                if (World.Current.CanMoveOn(this, mapCoords + move) && !(mapCoords + move == tempRestrictedCase)) {
                                    tempRestrictedCase = mapCoords;
                                } else {
                                    move = new Vector2(-vector.x / Mathf.Abs(vector.x) ,0);
                                    if (World.Current.CanMoveOn(this, mapCoords + move) && !(mapCoords + move == tempRestrictedCase)) {
                                        tempRestrictedCase = mapCoords;
                                    } else {
                                        tempRestrictedCase = new Vector2(-1, -1);
                                        move = Vector2.zero;
                                        orientation = GetOrientation(vector);
                                    }
                                }
                            }
                        } else {
                            move = new Vector2(0, -vector.y / Mathf.Abs(vector.y));
                            if (World.Current.CanMoveOn(this, mapCoords + move) && !(mapCoords + move == tempRestrictedCase)) {
                                tempRestrictedCase = mapCoords;
                            } else {
                                move = new Vector2(this.orientation == Orientation.Right ? 1 : this.orientation == Orientation.Left ? -1 : MathUtility.Alea(new float[] { -1, 1 }), 0);
                                if (World.Current.CanMoveOn(this, mapCoords + move) && !(mapCoords + move == tempRestrictedCase)) {
                                    tempRestrictedCase = mapCoords;
                                } else {
                                    move = new Vector2(-move.x, 0);
                                    if (World.Current.CanMoveOn(this, mapCoords + move) && !(mapCoords + move == tempRestrictedCase)) {
                                        tempRestrictedCase = mapCoords;
                                    } else {
                                        tempRestrictedCase = new Vector2(-1, -1);
                                        move = Vector2.zero;
                                        orientation = GetOrientation(vector);
                                    }
                                }
                            }
                        }
                    }
                }

                //if (vector.magnitude == 1) { orientation = GetOrientation(vector); break; }
                //if(Mathf.Abs(vector.x) >= Mathf.Abs(vector.y)){
                //    move = new Vector2(vector.x/Mathf.Abs(vector.x) , 0);
                //    if (!World.Current.CanMoveOn(this, mapCoords + move)) {
                //        if (vector.y != 0) { 
                //            move = new Vector2(0, this.orientation == Orientation.Down ? 1 : this.orientation == Orientation.Up ? -1 : vector.y/Mathf.Abs(vector.y));
                //            if (!World.Current.CanMoveOn(this, mapCoords + move)) {
                //                move = -move;
                //            }
                //        } else {
                //            move = new Vector2(0, this.orientation == Orientation.Down ? 1 : this.orientation == Orientation.Up ? -1 : MathUtility.Alea(new float[]{-1,1}));
                //            if (!World.Current.CanMoveOn(this, mapCoords + move)) {
                //                move = -move;
                //            }
                //        }
                //    }
                //} else {
                //    move = new Vector2(0 , vector.y / Mathf.Abs(vector.y));
                //    if (!World.Current.CanMoveOn(this, mapCoords + move)) {
                //        if (vector.x != 0) {
                //            move = new Vector2(this.orientation == Orientation.Right ? 1 : this.orientation == Orientation.Left ? -1 : vector.x / Mathf.Abs(vector.x) , 0);
                //            if (!World.Current.CanMoveOn(this, mapCoords + move)) {
                //                move = -move;
                //            }
                //        } else {
                //            move = new Vector2(this.orientation == Orientation.Right ? 1 : this.orientation == Orientation.Left ? -1 : MathUtility.Alea(new float[] { -1, 1 }) , 0);
                //            if (!World.Current.CanMoveOn(this, mapCoords + move)) {
                //                move = -move;
                //            }
                //        }
                //    }
                //}
                orientation = GetOrientation(move);
                break;
            case PossibleMovement.FleePlayer:
                if (Mathf.Abs(vector.x) >= Mathf.Abs(vector.y)) {
                    move = new Vector2(- vector.x / Mathf.Abs(vector.x), 0);
                    if (!World.Current.CanMoveOn(this, mapCoords + move)) {
                        if (vector.y != 0) {
                            move = new Vector2(0, (this.orientation == Orientation.Down ? 1 : this.orientation == Orientation.Up ? -1 : -vector.y / Mathf.Abs(vector.y)));
                            if (!World.Current.CanMoveOn(this, mapCoords + move)) {
                                move = -move;
                            }
                        } else {
                            move = new Vector2(0, (this.orientation == Orientation.Down ? 1 : this.orientation == Orientation.Up ? -1 : MathUtility.Alea(new float[] { -1, 1 })));
                            if (!World.Current.CanMoveOn(this, mapCoords + move)) {
                                move = -move;
                            }
                        }
                    }
                } else {
                    move = new Vector2(0, -vector.y / Mathf.Abs(vector.y));
                    if (!World.Current.CanMoveOn(this, mapCoords + move)) {
                        if (vector.x != 0) {
                            move = new Vector2((this.orientation == Orientation.Right ? 1 : this.orientation == Orientation.Left ? -1 : -vector.x / Mathf.Abs(vector.x)), 0);
                            if (!World.Current.CanMoveOn(this, mapCoords + move)) {
                                move = -move;
                            }
                        } else {
                            move = new Vector2((this.orientation == Orientation.Right ? 1 : this.orientation == Orientation.Left ? -1 : MathUtility.Alea(new float[] { -1, 1 })), 0);
                            if (!World.Current.CanMoveOn(this, mapCoords + move)) {
                                move = -move;
                            }
                        }
                    }
                }
                orientation = GetOrientation(move);
                break;
            case PossibleMovement.LookPlayer:
                orientation = vector.x == Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y)) ? Orientation.Right : vector.x == - Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y)) ? Orientation.Left : vector.y == Mathf.Max(Mathf.Abs(vector.x), Mathf.Abs(vector.y)) ? Orientation.Down : Orientation.Up;
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
            // TODO : Add Boolean to lockPlayer while actions
            World.Current.ExecuteActions(this, false);
    }

    protected bool HaveToExecute() {
        if (isRunning)
            return false;
        
        switch (execCondition) {
            case ExecutionCondition.Action:
                return (InputManager.Current.GetKeyDown(KeyCode.Return) || InputManager.Current.GetKeyDown(KeyCode.Space)) && (
                    Player.Current.orientation == Orientation.Down  && Player.Current.mapCoords == mapCoords + new Vector2 (0,-1) ||
                    Player.Current.orientation == Orientation.Up    && Player.Current.mapCoords == mapCoords + new Vector2 (0, 1) ||
                    Player.Current.orientation == Orientation.Left  && Player.Current.mapCoords == mapCoords + new Vector2 ( 1,0) ||
                    Player.Current.orientation == Orientation.Right && Player.Current.mapCoords == mapCoords + new Vector2 (-1,0));
            case ExecutionCondition.ActionFace:
                return (InputManager.Current.GetKeyDown(KeyCode.Return) || InputManager.Current.GetKeyDown(KeyCode.Space)) && (
                    Player.Current.orientation == Orientation.Down  && orientation == Orientation.Up    && Player.Current.mapCoords == mapCoords + new Vector2(0,-1) ||
                    Player.Current.orientation == Orientation.Up    && orientation == Orientation.Down  && Player.Current.mapCoords == mapCoords + new Vector2(0, 1) ||
                    Player.Current.orientation == Orientation.Left  && orientation == Orientation.Right && Player.Current.mapCoords == mapCoords + new Vector2( 1,0) ||
                    Player.Current.orientation == Orientation.Right && orientation == Orientation.Left  && Player.Current.mapCoords == mapCoords + new Vector2(-1,0));
        }

        return false;
    }

    public static MapObject Generate(DBMapObject _source) {
        // Generate element
        MapObject m = new MapObject();
        m.mapObjectId = _source.ID;
        m.mapCoords = _source.mapCoords;

        m.name = _source.name;
        m.spritePath = _source.sprite;
        m.sprite = InterfaceUtility.GetTexture(Config.GetResourcePath(IMAGE_FOLDER) + _source.sprite);
        
        m.speed = _source.speed;
        m.orientation = _source.orientation;
        m.layer = _source.layer;
        m.execCondition = _source.execCondition;

        m.allowPassThrough = _source.allowPassThrough;

        // Generate actions
        foreach (DBMapObjectAction action in DataBase.GetMapObjectActions(m.mapObjectId))
            m.actions.Add(MapObjectAction.Generate(action));

        return m;
    }
}
