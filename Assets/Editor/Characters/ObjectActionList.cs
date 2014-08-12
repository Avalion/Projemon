﻿using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/**
 *  Display a list of MapObjectAction 
 */
public class ObjectActionList : EditorWindow {
    public MapObject mapObject;
    int selectedElement = 0;

    Vector2 scrollPosList;
    Vector2 scrollPosEdit;

    string[] types = new string[] { 
        "Select an Action to Add", 
        "ActionEXP",
        "ActionFade", 
        "ActionHeal",
        "ActionMessage", 
        "ActionMonsterBattle", 
        "ActionMove", 
        "ActionPlaySound", 
        "ActionPNJBattle", 
        "ActionTeleport",
        "ActionTransform",
        "ActionWait"        
    };

    public void OnGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(Screen.width / 3));
        scrollPosList = GUILayout.BeginScrollView(scrollPosList);
        GUILayout.BeginVertical();
        foreach (MapObjectAction a in mapObject.actions) {
            GUILayout.BeginHorizontal();
            if (GUILayout.Button((a.waitForEnd ? "*" : "") + a.InLine(), GUILayout.Width(Screen.width / 3 - 100)))
                selectedElement = mapObject.actions.IndexOf(a);
            if (GUILayout.Button("X")) {
                mapObject.actions.Remove(a);
                GUIUtility.ExitGUI();
                return;
            }
            GUILayout.EndHorizontal();
            
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();

        int value = EditorGUILayout.Popup(0, types);
        if (value != 0) {
            mapObject.actions.Add(System.Activator.CreateInstance(Types.GetType(types[value], "Assembly-CSharp")) as MapObjectAction);
            selectedElement = mapObject.actions.Count - 1;
        }
        GUILayout.EndVertical();
        
        scrollPosEdit = GUILayout.BeginScrollView(scrollPosEdit);
        GUILayout.BeginVertical();
        if (selectedElement < mapObject.actions.Count) {
            EditDisplay(mapObject.actions[selectedElement]);
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        GUILayout.EndHorizontal();
    }    

    public void EditDisplay(MapObjectAction _action) {
        _action.waitForEnd = EditorGUILayout.ToggleLeft("Wait For End", _action.waitForEnd);

        switch (_action.GetType().ToString()) {
            case "ActionEXP":
                DisplayEditor((ActionEXP)_action); break;
            case "ActionFade":
                DisplayEditor((ActionFade)_action); break;
            case "ActionHeal":
                DisplayEditor((ActionHeal)_action); break;
            case "ActionMessage":
                DisplayEditor((ActionMessage)_action); break;
            case "ActionMove":
                DisplayEditor((ActionMove)_action); break;
            case "ActionPlaySound":
                DisplayEditor((ActionPlaySound)_action); break;
            case "ActionTeleport":
                DisplayEditor((ActionTeleport)_action); break;
            case "ActionTransform":
                DisplayEditor((ActionTransform)_action); break;
            case "ActionWait":
                DisplayEditor((ActionWait)_action); break;
            case "ActionMonsterBattle":
                DisplayEditor((ActionMonsterBattle)_action); break;
            case "ActionPNJBattle":
                DisplayEditor((ActionPNJBattle)_action); break;
            default:
                GUILayout.Label("Unknown Action");
                return;
        }
    }

    private void DisplayEditor(ActionFade a) {
        a.color = EditorGUILayout.ColorField("Color", a.color);
        a.duration = EditorGUILayout.FloatField("Duration", a.duration);
    }
    private void DisplayEditor(ActionMessage a) {
        a.face = EditorGUILayout.ObjectField("Face", a.face, typeof(Texture2D), false) as Texture2D;
        a.faceOnRight = EditorGUILayout.Toggle("Face On Right", a.faceOnRight);
        a.message = EditorGUILayout.TextField("Message", a.message);
        a.maxDuration = EditorGUILayout.FloatField("Max Duration", a.maxDuration);
    }
    private void DisplayEditor(ActionMove a) {
        for (int i = 0; i < a.movements.Count; i++) {
            GUILayout.BeginHorizontal();
            a.movements[i] = (MapObject.PossibleMovement)EditorGUILayout.EnumPopup(i + ":", a.movements[i]);
            if (GUILayout.Button("X"))
                a.movements.RemoveAt(i);
            GUILayout.EndHorizontal();
        }
        
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("+"))
            a.movements.Add(MapObject.PossibleMovement.Forward);
        GUILayout.EndHorizontal();
    }
    private void DisplayEditor(ActionPlaySound a) {
        a.sound = EditorGUILayout.ObjectField("Sound", a.sound, typeof(AudioClip), false) as AudioClip;
        a.bgm = EditorGUILayout.Toggle("BGM", a.bgm);
    }
    private void DisplayEditor(ActionTeleport a) {
        GUILayout.Label("TODO : Choisir une map, lister les mapObjects associés et pouvoir choisir une position et une orientation");
    }
    private void DisplayEditor(ActionWait a) {
        a.duration = Mathf.Max(0, EditorGUILayout.FloatField("Time", a.duration));
    }
    private void DisplayEditor(ActionMonsterBattle a) {
        GUILayout.Label("TODO : Display a list of monsterPattern : cf DisplayEditor(ActionMove)");
    }
    private void DisplayEditor(ActionPNJBattle a) {
        GUILayout.Label("TODO : Display a popup with all PNJBattler");
    }
    private void DisplayEditor(ActionTransform a) {
        GUILayout.Label("TODO : Display a popup with all MapObject");
    }
    private void DisplayEditor(ActionHeal a) {
        GUILayout.Label("TODO : Display a popup with all Battler and its Monsters");
    }
    private void DisplayEditor(ActionEXP a) {
        GUILayout.Label("TODO : Display a popup with all Battler and its Monsters");
    }
}
