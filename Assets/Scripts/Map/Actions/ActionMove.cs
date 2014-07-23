using UnityEngine;
using System.Collections.Generic;

/**
 * This action will move a MapObject toward a path, ignore not possible moves
 */
public class ActionMove : MapObjectAction {
    public MapObject target;
    public List<MapObject.PossibleMovement> movements;

    public ActionMove(MapObject _target, List<MapObject.PossibleMovement> _movements, bool _waitForEnd = true) {
        target = _target;
        movements = _movements;
        waitForEnd = _waitForEnd;
    }

    public override void Execute() {
        ActionMoveDisplay display = new GameObject("action_Move").AddComponent<ActionMoveDisplay>();
        display.action = this;
    }
}

public class ActionMoveDisplay : MonoBehaviour {
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
