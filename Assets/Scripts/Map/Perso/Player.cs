using UnityEngine;
using System.Collections.Generic;

/**
 * This class design the Hero
 */
public class Player : Battler {
    private static Player current = null;
    public static Player Current {
        get {
            if (current == null) {
                current = GameObject.FindObjectOfType<Player>();
                if (current == null) {
                    current = new GameObject("Player").AddComponent<Player>();
                }
            }
            return current;
        }
    }

    private static int locked;
    public static bool Locked {
        get { return locked != 0; }
    }
    public static void Lock() {
        locked++;
    }
    public static void Unlock() {
        locked = Mathf.Max(0, locked - 1);
    }
    public static void ForceUnlock() {
        locked = 0;
    }

    public void Start() {
        monsters.Add(Monster.GenerateFromPattern(SystemDatas.GetMonsterPatterns()[0], 5, 5));
    }

    public override void OnUpdate() {
        if (!isMoving && !Locked) {
            if (InputManager.Current.GetKey(KeyCode.LeftArrow))
                Move(PossibleMovement.Left);
            else if (InputManager.Current.GetKey(KeyCode.RightArrow))
                Move(PossibleMovement.Right);
            else if (InputManager.Current.GetKey(KeyCode.UpArrow))
                Move(PossibleMovement.Up);
            else if (InputManager.Current.GetKey(KeyCode.DownArrow))
                Move(PossibleMovement.Down);
        }

        if (isMoving && Locked) {
            lerp = 0;
            currentMovement = Vector2.zero;
            isMoving = false;
        }
    }
}