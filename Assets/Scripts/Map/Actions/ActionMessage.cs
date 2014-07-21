﻿using UnityEngine;

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
        display.message = this;
    }
}

public class ActionMessageDisplay : IDisplayable {
    public ActionMessage message;

    public GUIStyle messageStyle = new GUIStyle() { fontSize = 30, fontStyle = FontStyle.Bold, wordWrap = true };

    private float lerp;
    public float duration = 10;

    public void Update() {
        lerp += Time.deltaTime / duration;
        if (lerp >= 1) lerp = 1;

        if (InputManager.Current.GetKeyDown(KeyCode.Return)) {
            if (lerp < 1)
                lerp = 1;
            else {
                message.Terminate();
                Dispose();
            }
        }
    }

    public override void  Display() {
        GUILayout.BeginArea(new Rect(0, Screen.height - 200, Screen.width, 200));
        InterfaceUtility.BeginBox();
        GUILayout.BeginHorizontal(GUILayout.MaxWidth(Screen.width));
        if (message.face != null && !message.faceOnRight) {
            GUILayout.Label(message.face, InterfaceUtility.EmptyStyle, GUILayout.Width(160), GUILayout.Height(160));
            GUILayout.Space(20);
        }
        string m = message.message;
        m = m.Substring(0, Mathf.RoundToInt(lerp * m.Length));
        GUILayout.Label(m, messageStyle);
        if (message.face != null && message.faceOnRight) {
            GUILayout.Space(20);
            GUILayout.Label(message.face, InterfaceUtility.EmptyStyle, GUILayout.Width(160), GUILayout.Height(160));
        }
        GUILayout.EndHorizontal();
        InterfaceUtility.EndBox();
        GUILayout.EndArea();
    }
}
