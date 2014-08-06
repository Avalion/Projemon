﻿using UnityEngine;

/**
 * This action will apply a filter on the camera
 */
[System.Serializable]
public class ActionFade : MapObjectAction {
    public float duration;

    public Color color;
    
    public ActionFade() {
        color = new Color(0,0,0,0);
        duration = 0;
    }
    public ActionFade(Color _color, float _duration, bool _waitForEnd = true) {
        color = _color;
        duration = _duration;
        waitForEnd = _waitForEnd;
    }

    public override void Execute() {
        ActionFadeDisplay display = new GameObject("action_Fade").AddComponent<ActionFadeDisplay>();
        display.action = this;
    }


    
}

public class ActionFadeDisplay : IDisplayable {
    public ActionFade action;

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