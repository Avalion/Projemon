﻿using UnityEngine;

/**
 * This class defines map objects as Player or PNJBattlers
 */
public class MapObject : MonoBehaviour {
    public enum MovementSpeed { None, Slow, Normal, Fast, Instant };
    public MovementSpeed speed = MovementSpeed.None;

    public enum Orientation { Down, Left, Right, Up };
    public Orientation orientation = Orientation.Down;

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
    [HideInInspector] public float lerp = 0;
     
    /* Functions
     */
    public void Update() {
        if (isMoving) {
            switch (speed) {
                case MovementSpeed.None: break;
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

        OnUpdate();
    }

    public virtual void OnUpdate() {}

    public void DisplayOnMap() {
        if (sprite != null) {
            float height = Sprite.height * (Map.Resolution.x / Sprite.width);
            GUI.DrawTexture(new Rect(Map.Resolution.x * (mapCoords.x + currentMovement.x * lerp), Map.Resolution.y * (mapCoords.y + currentMovement.y * lerp + 1) - height, Map.Resolution.x, height), Sprite);
        }
    }

    public void Move(Orientation _o) {
        isMoving = true;

        Vector2 move = new Vector2(_o == Orientation.Left ? -1 : _o == Orientation.Right ? 1 : 0, _o == Orientation.Up ? -1 : _o == Orientation.Down ? 1 : 0);

        orientation = _o;

        if (World.Current.CanMoveOn(mapCoords + move))
            currentMovement = move;
    }
}