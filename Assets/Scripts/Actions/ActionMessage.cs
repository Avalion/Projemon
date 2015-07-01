using UnityEngine;

/**
 * This action will display a Message
 */
public class ActionMessage : MapObjectAction {
    public const int MESSAGE_HEIGHT = 200;
    public const string IMAGE_FOLDER = "Faces";

    public Texture2D face;
    public string message = "";
    public bool faceOnRight = false;
    public float maxDuration = -1;

    public enum Placement { Top, Middle, Bottom }
    public Placement placement = Placement.Bottom;

    public ActionMessage() {}
    public ActionMessage(string _message, bool _waitForEnd = true) {
        waitForEnd = _waitForEnd;
        message = _message;
    }
    public ActionMessage(Texture2D _face, string _message, bool _right, bool _waitForEnd = true) {
        waitForEnd = _waitForEnd;
        face = _face;
        message = _message;
        faceOnRight = _right;
    }
    public ActionMessage(Texture2D _face, string _message, bool _right, float _duration, bool _waitForEnd = true) {
        waitForEnd = _waitForEnd;
        face = _face;
        message = _message;
        faceOnRight = _right;
        maxDuration = _duration;
    }
    
    public override void Execute() {
        ActionMessageDisplay display = new GameObject("action_Message").AddComponent<ActionMessageDisplay>();
        display.action = this;
        display.SendMessage("Start");
    }

    public override string InLine() {
        return "Message : " + (message.Length > 25 ? message.Substring(0,25) + "(...)" : message);
    }
    public override string Serialize() {
        return GetType().ToString() + "|" + (face == null ? "" : face.name) + "|" + message + "|" + faceOnRight + "|" + maxDuration + "|" + (int)placement;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 6)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        if (values[1] != "") face = InterfaceUtility.GetTexture(Config.GetResourcePath(IMAGE_FOLDER) + values[1] + ".png");
        message = values[2];
        faceOnRight = bool.Parse(values[3]);
        maxDuration = float.Parse(values[4]);
        placement = (Placement)int.Parse(values[5]);
    }
}

public class ActionMessageDisplay : IDisplayable {
    [HideInInspector]
    public ActionMessage action;

    private GUIStyle messageStyle = new GUIStyle();
    private GUIStyle headStyle = new GUIStyle();

    private float lerp = 0;
    public float duration = -1;

    public float timeCount = 0;

    public void Start() {
        if (duration < 0)
            duration = 0.05f * action.message.Length;

        messageStyle = new GUIStyle();
        messageStyle.fontSize = 30;
        messageStyle.fontStyle = FontStyle.Bold;
        messageStyle.wordWrap = true;
        messageStyle.normal.textColor = Color.white;
        headStyle = new GUIStyle();
        headStyle.normal.background = action.face;
    }

    public void Update() {
        if (action.maxDuration > 0) {
            timeCount += Time.deltaTime;
            if (timeCount >= action.maxDuration) {
                action.Terminate();
                Dispose();
            }
        }

        if (lerp < 1) {
            if (duration == 0)
                lerp = 1;
            else
                lerp += Time.deltaTime / duration;
        }
        if (lerp > 1)
            lerp = 1;
        
        if (InputManager.Current.GetKeyDown(KeyCode.Return)) {
            if (lerp < 1)
                lerp = 1;
            else {
                action.Terminate();
                Dispose();
            }
        }
    }

    public override void Display() {
        // TODO : replace KeyWords or KeyCharacters by their values (variable, ou variable analysé (nom de MapObject ou de MonsterPattern ou de Monster)

        Rect area = new Rect(0, 0, Screen.width, ActionMessage.MESSAGE_HEIGHT);
        if (action.placement == ActionMessage.Placement.Top)
            area.y = 0;
        else if (action.placement == ActionMessage.Placement.Middle)
            area.y = (Screen.height - ActionMessage.MESSAGE_HEIGHT) / 2;
        else if (action.placement == ActionMessage.Placement.Bottom)
            area.y = Screen.height - ActionMessage.MESSAGE_HEIGHT;

        GUILayout.BeginArea(area);
        InterfaceUtility.BeginBox();
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width));
        if (action.face != null && !action.faceOnRight) {
            GUILayout.Label("", headStyle, GUILayout.Width(ActionMessage.MESSAGE_HEIGHT - 30), GUILayout.Height(ActionMessage.MESSAGE_HEIGHT - 30));
            GUILayout.Space(20);
        }
        string m = action.message;
        m = m.Substring(0, Mathf.RoundToInt(lerp * m.Length));
        GUILayout.Label(m, messageStyle);
        if (action.face != null && action.faceOnRight) {
            GUILayout.Space(20);
            GUILayout.Label("", headStyle, GUILayout.Width(170), GUILayout.Height(170));
        }
        GUILayout.EndHorizontal();
        InterfaceUtility.EndBox();
        GUILayout.EndArea();
    }
}
