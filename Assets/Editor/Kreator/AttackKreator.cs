﻿using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/**
 * This class provides gestion of Attacks
 */
public class AttackKreator : EditorWindow {
    public static List<Attack> elements = new List<Attack>();
    int selectedElement = -1;
    Attack current {
        get { return elements[selectedElement]; }
    }

    public static List<BattleAnimation> battleAnimations;


    Vector2 _scrollPosList = Vector2.zero;

    // Launch
    [MenuItem("Creation/Attacks")]
    public static void Init() {
        AttackKreator window = EditorWindow.GetWindow<AttackKreator>();
        window.minSize = new Vector2(1000, 500);
        window.maxSize = new Vector2(1000, 501);
        window.Show();

        InterfaceUtility.ClearAllCache();

        elements = SystemDatas.GetAttacks();
        battleAnimations = SystemDatas.GetBattleAnimations();
    }

    // Display
    public void OnGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(150));
        int value = EditorUtility.DisplayList<Attack>(selectedElement, elements, ref _scrollPosList);
        if (selectedElement != value) {
            Select(value);
        }
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add")) {
            elements.Add(new Attack());
            selectedElement = elements.Count - 1;
        }
        if (GUILayout.Button("Delete") && elements.Count > 1) {
            if (selectedElement == elements.Count - 1) {
                elements.RemoveAt(selectedElement);
                Select(elements.Count - 1);
            } else {
                elements[selectedElement] = new Attack();
            }
        } 
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        if (elements.Count > 0 && selectedElement >= 0) {
            current.name = EditorGUILayout.TextField("Name", current.name);
            current.type = (Monster.Type)EditorGUILayout.EnumPopup("Type", current.type);

            GUILayout.BeginHorizontal(GUILayout.Height(100));

            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Label("- Stats");
            current.power = EditorGUILayout.IntField("Power", current.power);
            current.precision = EditorGUILayout.IntField("Precision", current.precision);

            List<string> names = new List<string>();
            foreach (BattleAnimation ba in battleAnimations)
                names.Add(InterfaceUtility.IntString(ba.ID + 1, 3) + ": " + ba.name);
            int index = names.FindIndex(P => P.StartsWith(InterfaceUtility.IntString(current.battleAnimationID + 1, 3)));
            index = EditorGUILayout.Popup("Battle animation", index, names.ToArray());
            current.battleAnimationID = battleAnimations[index].ID;
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();


        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("OK")) {
            SystemDatas.SetAttacks(elements);
            Close();
        }
        GUILayout.EndHorizontal();
    }

    public void Select(int _select) {
        selectedElement = _select;
    }
}