using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/**
 * This class provides gestion of Attacks
 */
public class AttackKreator : EditorWindow {
    public static List<DBAttack> elements = new List<DBAttack>();
    int selectedElement = -1;
    DBAttack current {
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

        if (!DataBase.IsConnected) DataBase.Connect(Application.dataPath + "/database.sql");

        elements = DataBase.Select<DBAttack>();

        battleAnimations = SystemDatas.GetBattleAnimations();
    }

    // Display
    public void OnGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(150));
        int value = EditorUtility.DisplayList<DBAttack>(selectedElement, elements, ref _scrollPosList);
        if (selectedElement != value) {
            Select(value);
        }
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add")) {
            elements.Add(new DBAttack());
            selectedElement = elements.Count - 1;
        }
        if (GUILayout.Button("Delete") && elements.Count > 1) {
            if (selectedElement == elements.Count - 1) {
                elements.RemoveAt(selectedElement);
                Select(elements.Count - 1);
            } else {
                elements[selectedElement] = new DBAttack();
            }
        } 
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        if (elements.Count > 0 && selectedElement >= 0) {
            current.name = EditorGUILayout.TextField("Name", current.name);
            current.type = (Monster.Type)EditorGUILayout.EnumPopup("Type", current.type);

            GUILayout.Label("- Stats");
            current.power = EditorGUILayout.IntField("Power", current.power);
            current.accuracy = EditorGUILayout.IntField("Precision", current.accuracy);

            List<string> names = new List<string>();
            foreach (BattleAnimation ba in battleAnimations)
                names.Add(InterfaceUtility.IntString(ba.ID + 1, 3) + ": " + ba.name);

            int index = names.FindIndex(P => P.StartsWith(InterfaceUtility.IntString(current.battleAnimationID + 1, 3)));
            index = EditorGUILayout.Popup("Battle animation", index, names.ToArray());
            
            if (index >= 0 && index < battleAnimations.Count)
                current.battleAnimationID = battleAnimations[index].ID;
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("OK")) {
            if (elements.Find(P => P.name == null) != null) {
                Debug.LogError("You can't save if some elements have no name !");
            } else {
                foreach (DBAttack attack in elements)
                    DataBase.Replace<DBAttack>(attack);
                Close();
            }
            
        }
        GUILayout.EndHorizontal();
    }

    public void Select(int _select) {
        selectedElement = _select;
    }
}