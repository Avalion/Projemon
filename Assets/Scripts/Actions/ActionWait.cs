using UnityEngine;

/**
 * This action will wait some seconds
 */
public class ActionWait : MapObjectAction {
    public float duration;

    public ActionWait() {}
    public ActionWait(float _duration) {
        duration = _duration;
    }

    public override void Execute() {
        ActionWaitDisplay display = new GameObject("action_Wait").AddComponent<ActionWaitDisplay>();
        display.action = this;
    }

    public override string InLine() {
        return "Wait " + duration + " seconds.";
    }

    public override string Serialize() {
        return GetType().ToString() + "|" + duration;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 2)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        float.TryParse(values[1], out duration);
    }
}

public class ActionWaitDisplay : MonoBehaviour {
    public ActionWait action;

    public float timeCount;

    public void Update() {
        timeCount += Time.deltaTime;
        if (timeCount >= action.duration) {
            action.Terminate();
            Destroy(gameObject);
        }
    }
}