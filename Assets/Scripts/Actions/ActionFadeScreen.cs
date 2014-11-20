using UnityEngine;

/**
 * This action will apply a filter on the camera
 */
[System.Serializable]
public class ActionFadeScreen : MapObjectAction {
    public float duration;

    public Color color;
    
    public ActionFadeScreen() {
        color = new Color(0,0,0,0);
        duration = 0;
    }
    public ActionFadeScreen(Color _color, float _duration, bool _waitForEnd = true) {
        color = _color;
        duration = _duration;
        waitForEnd = _waitForEnd;
    }

    public override void Execute() {
        ActionFadeDisplay display = new GameObject("action_FadeScreen").AddComponent<ActionFadeDisplay>();
        display.action = this;
    }


    public override string InLine() {
        return "Fade to C(" + (int)(color.r * 256) + "," + (int)(color.g * 256) + "," + (int)(color.b * 256) + "," + (int)(color.a * 256) + ") in " + duration + " seconds.";
    }

    public override string Serialize() {
        return GetType().ToString() + "|" + duration + "|" + color.r + "|" + color.g + "|" + color.b + "|" + color.a;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 3)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        // TODO : Read and find MapObjectID when MapObject are into DB
        duration = float.Parse(values[1]);
        color = new Color(float.Parse(values[2]), float.Parse(values[3]), float.Parse(values[4]), float.Parse(values[5]));
    }
}

public class ActionFadeDisplay : IDisplayable {
    public ActionFadeScreen action;

    public float lerp = 0;

    public Color init;

    public void Start() {
        init = World.Current.currentFilter;
    }

    public void Update() {
        if (lerp < 1) {
            if (action.duration == 0)
                lerp = 1;
            else
                lerp += Time.deltaTime / action.duration;
        }
        if (lerp >= 1) {
            World.Current.currentFilter = action.color;
            action.Terminate();
            Destroy(gameObject);
        }
    }

    public override void Display() {
        World.Current.currentFilter = new Color(init.r + (action.color.r - init.r) * lerp, init.g + (action.color.g - init.g) * lerp, init.b + (action.color.b - init.b) * lerp, init.a + (action.color.a - init.a) * lerp);
    }
}