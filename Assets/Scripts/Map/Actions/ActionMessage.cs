using UnityEngine;

public class ActionMessage : MapObjectAction {
    public Texture2D face;
    public string message;
    public bool faceOnRight = false;

    public ActionMessage(string message) {
        this.message = message;
    }
    public ActionMessage(Texture2D face, string message, bool right) {
        this.face = face;
        this.message = message;
        faceOnRight = right;
    }

    public override void Execute() {
        ActionMessageDisplay display = new GameObject("action_Message").AddComponent<ActionMessageDisplay>();
        display.action = this;
    }
}

public class ActionMessageDisplay : IDisplayable {
    public ActionMessage action;

    public GUIStyle messageStyle = new GUIStyle() { fontSize = 30, fontStyle = FontStyle.Bold, wordWrap = true, normal = new GUIStyleState() { textColor = Color.white } };
    public GUIStyle headStyle = new GUIStyle() { fontSize = 30, fontStyle = FontStyle.Bold, wordWrap = true, normal = new GUIStyleState() { textColor = Color.white } };

    private float lerp;
    public float duration = 2;

    public void Start() {
        messageStyle = new GUIStyle();
        messageStyle.fontSize = 30;
        messageStyle.fontStyle = FontStyle.Bold;
        messageStyle.wordWrap = true;
        messageStyle.normal.textColor = Color.white;
        headStyle = new GUIStyle();
        headStyle.normal.background = action.face;
    }

    public void Update() {
        lerp += Time.deltaTime / duration;
        if (lerp >= 1) lerp = 1;
        
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
        GUILayout.BeginArea(new Rect(0, Screen.height - 200, Screen.width, 200));
        InterfaceUtility.BeginBox();
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width));
        if (action.face != null && !action.faceOnRight) {
            GUILayout.Label("", headStyle, GUILayout.Width(170), GUILayout.Height(170));
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
