using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/**
 * This class provides gestion of Attacks
 */
public class AttackKreator : EditorWindow {
    public static List<DBAttack> elements = new List<DBAttack>();
    private int selectedElement = -1;
    private DBAttack current { get { return elements[selectedElement]; } }

    public static List<DBAttack> toDestroy = new List<DBAttack>();
    
    private static int numberElements = 0;

    public static List<BattleAnimation> battleAnimations;


    Vector2 _scrollPosList = Vector2.zero;

    // Launch
    [MenuItem("Creation/Attacks &A")]
    public static void Init() {
        AttackKreator window = EditorWindow.GetWindow<AttackKreator>();
        window.minSize = new Vector2(1000, 500);
        window.maxSize = new Vector2(1000, 501);
        window.Show();

        InterfaceUtility.ClearAllCache();

        if (!DataBase.IsConnected) DataBase.Connect(Application.dataPath + "/database.sql");

        elements = DataBase.Select<DBAttack>();
        numberElements = elements.Count;

        if (numberElements > 0)
            window.Select(0);

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
            elements.Add(new DBAttack() { ID = elements.Count });
            Select(elements.Count - 1);
            numberElements = elements.Count;
        }
        if (GUILayout.Button("Delete") && elements.Count > 1) {
            if (selectedElement == elements.Count - 1) {
                toDestroy.Add(elements[selectedElement]);
                elements.RemoveAt(selectedElement);
                Select(elements.Count - 1);
            } else {
                elements[selectedElement] = new DBAttack() { ID = elements[selectedElement].ID };
            }
            numberElements = elements.Count;
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Nb ");

        // Check return event before Unity Text because it use the KeyDown Event
        if (GUI.GetNameOfFocusedControl() == "EditorListSize" && Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.KeypadEnter || Event.current.keyCode == KeyCode.Return)) {
            while (elements.Count < numberElements)
                elements.Add(new DBAttack() { ID = elements.Count });
            while (elements.Count > numberElements) {
                toDestroy.Add(elements[elements.Count - 1]);
                elements.RemoveAt(elements.Count - 1);
            }
            Select(Mathf.Clamp(selectedElement, 0, numberElements - 1));
        }

        GUI.SetNextControlName("EditorListSize");
        numberElements = EditorGUILayout.IntField(numberElements);
        if (GUILayout.Button("Apply")) {
            while (elements.Count < numberElements)
                elements.Add(new DBAttack() { ID = elements.Count });
            while (elements.Count > numberElements) {
                toDestroy.Add(elements[elements.Count - 1]);
                elements.RemoveAt(elements.Count - 1);
            }
            Select(Mathf.Clamp(selectedElement, 0, numberElements - 1));
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        if (elements.Count > 0 && selectedElement >= 0) {
            current.name = EditorGUILayout.TextField("Name", current.name);
            current.type = (Monster.Type)EditorGUILayout.EnumPopup("Type", current.type);

            GUILayout.Space(10);

            GUILayout.Label("- Attack", InterfaceUtility.TitleStyle);
            current.power = EditorGUILayout.IntField("Power", current.power);
            current.accuracy = EditorGUILayout.IntField("Precision", current.accuracy);

            current.staminaCost = EditorGUILayout.IntField("Cost (Stamina)", current.staminaCost);


            GUILayout.Space(10);
            GUILayout.Label("- States", InterfaceUtility.TitleStyle);
            
            GUILayout.BeginHorizontal();
            current.enemyStateChange = (Monster.State)EditorGUILayout.EnumPopup("Target State change", current.enemyStateChange);
            GUI.enabled = current.enemyStateChange != Monster.State.None;
            GUILayout.Label("Precision (%)", GUILayout.Width(100));
            current.enemyStateChangeAccuracy = EditorGUILayout.IntField(current.enemyStateChangeAccuracy, GUILayout.Width(50));
            GUI.enabled = true;
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            current.launcherStateChange = (Monster.State)EditorGUILayout.EnumPopup("Launcher State change", current.launcherStateChange);
            GUI.enabled = current.launcherStateChange != Monster.State.None;
            GUILayout.Label("Precision (%)", GUILayout.Width(100));
            current.launcherStateChangeAccuracy = EditorGUILayout.IntField(current.launcherStateChangeAccuracy, GUILayout.Width(50));
            GUI.enabled = true;
            GUILayout.EndHorizontal();

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
            if (elements.Find(P => P.name == null || P.name == "") != null) {
                Debug.LogError("You can't save if some elements have no name !");
            } else {
                foreach (DBAttack attack in elements)
                    DataBase.Replace<DBAttack>(attack);
                foreach (DBAttack attack in toDestroy)
                    attack.Delete();
                toDestroy.Clear();
                Close();
            }
            
        }
        GUILayout.EndHorizontal();
    }

    public void Select(int _select) {
        selectedElement = _select;
    }
}