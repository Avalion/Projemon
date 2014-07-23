using UnityEngine;
using System.Collections.Generic;

public class ActionMove : MapObjectAction {
    public MapObject target;
    public MapObject.PossibleMovement movement;

    public ActionMove(MapObject _target, MapObject.PossibleMovement _movement, bool _waitForEnd = true) {
        target = _target;
        movement = _movement;
        waitForEnd = _waitForEnd;
    }

    public override void Execute() {
        target.Move(movement);
        if (!target.isMoving) { 
            Terminate();
            return;
        }
        ActionMoveDisplay display = new GameObject("action_Move").AddComponent<ActionMoveDisplay>();
        display.action = this;
    }
}

public class ActionMoveDisplay : MonoBehaviour {
    public ActionMove action;

    public void Update() {
        if (!action.target.isMoving) { 
            action.Terminate();
            Destroy(gameObject);
        }
    }
}
