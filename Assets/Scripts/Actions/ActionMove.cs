using UnityEngine;
using System.Collections.Generic;

/**
 * This action will move a MapObject toward a path, ignore not possible moves
 */
public class ActionMove : MapObjectAction {
    public int targetId;
    public MapObject target = null;
    public List<MapObject.PossibleMovement> movements = new List<MapObject.PossibleMovement>();

    public ActionMove() {
        targetId = -1;
        movements.Add(MapObject.PossibleMovement.NULL);
    }
    public ActionMove(MapObject _target, List<MapObject.PossibleMovement> _movements, bool _waitForEnd = true) {
        target = _target;
        targetId = _target.mapObjectId;
        movements = _movements;
        waitForEnd = _waitForEnd;
        movements.Add(MapObject.PossibleMovement.NULL);
    }

    public override void Execute() {
        target = World.Current.GetMapObjectById(targetId);

        ActionMoveDisplay display = new GameObject("action_Move").AddComponent<ActionMoveDisplay>();
        display.action = this;
        display.SendMessage("Start");
    }

    public override string InLine() {
        if (target == null)
            target = World.Current.GetMapObjectById(targetId);
        return target.name + " move : " + movements[0].ToString() + (movements.Count > 1 ? "(...)" : "")+".";
    }

    public override string Serialize() {
        string result = GetType().ToString() + "|" + targetId;
        foreach (MapObject.PossibleMovement move in movements)
            result += "|" + ((int)move);
        
        return result;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length < 2)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        targetId = int.Parse(values[1]);
        
        movements.Clear();
        for (int i = 2; i < values.Length; ++i)
            movements.Add((MapObject.PossibleMovement)int.Parse(values[i]));
    }
}

public class ActionMoveDisplay : MonoBehaviour {
    [HideInInspector]
    public ActionMove action;
    private int count = 0;

    public void Start() {
        do {
            action.target.Move(action.movements[count]);
            if (!action.target.isMoving) {
                count++;
                if (count >= action.movements.Count) {
                    action.Terminate();
                    Destroy(gameObject);
                    return;
                }
            }
        } while (!action.target.isMoving);
    }

    public void Update() {
        if (action.target.isMoving)
            return;

        count++;
        if (count >= action.movements.Count) {
            action.Terminate();
            Destroy(gameObject);
            return;
        }
        
        do {
            action.target.Move(action.movements[count]);
            if (!action.target.isMoving) {
                count++;
                if (count >= action.movements.Count) {
                    action.Terminate();
                    Destroy(gameObject);
                    return;
                }
            }
        } while (!action.target.isMoving);
    }
}
