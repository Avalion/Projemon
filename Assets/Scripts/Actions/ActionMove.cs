using UnityEngine;
using System.Collections.Generic;

/**
 * This action will move a MapObject toward a path, ignore not possible moves
 */
public class ActionMove : MapObjectAction {
    public MapObject target;
    public List<MapObject.PossibleMovement> movements = new List<MapObject.PossibleMovement>();

    public ActionMove() {
        target = Player.Current;
        movements.Add(MapObject.PossibleMovement.NULL);
    }
    public ActionMove(MapObject _target, List<MapObject.PossibleMovement> _movements, bool _waitForEnd = true) {
        target = _target;
        movements = _movements;
        waitForEnd = _waitForEnd;
        movements.Add(MapObject.PossibleMovement.NULL);
    }

    public override void Execute() {
        ActionMoveDisplay display = new GameObject("action_Move").AddComponent<ActionMoveDisplay>();
        display.action = this;
        display.SendMessage("Start");
    }

    public override string InLine() {
        return target.name + " move : " + movements[0].ToString() + (movements.Count > 1 ? "(...)" : "")+".";
    }

    public override string Serialize() {
        // TODO : Add MapObjectID when MapObject are into DB
        // TODO : Serialize list of movements
        return GetType().ToString();
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 1)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        // TODO : Read and find MapObjectID when MapObject are into DB
        // TODO : Deserialize list of movements
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
