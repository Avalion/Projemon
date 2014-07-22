using UnityEngine;
using System.Collections.Generic;

public class ActionWait : MapObjectAction {
    public float duration;

    public ActionWait(float _duration) {
        duration = _duration;
    }

    public override void Execute() {
        ActionWaitDisplay display = new GameObject("action_Wait").AddComponent<ActionWaitDisplay>();
        display.action = this;
    }

    
}

public class ActionWaitDisplay : MonoBehaviour {
    public ActionWait action;

    public float count;

    public void Update() {
        count += Time.deltaTime;
        if (count >= action.duration) {
            action.Terminate();
            Destroy(gameObject);
        }
    }
}