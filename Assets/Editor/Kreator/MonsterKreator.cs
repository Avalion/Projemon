using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/**
 * This class provides gestion of MonsterPattern
 */
public class MonsterKreator : EditorWindow {
    public static List<MonsterPattern> elements = new List<MonsterPattern>();
    private int selectedElement = -1;
    private MonsterPattern current { get { return elements[selectedElement]; } }

    public static List<string> battlersTextures = new List<string>();
    public static Texture2D previewBattleSprite;

    public static List<Attack> attackList = new List<Attack>();

    Vector2 _scrollPosList = Vector2.zero;

    // Launch
    [MenuItem("Creation/Monsters")]
    public static void Init() {
        MonsterKreator window = EditorWindow.GetWindow<MonsterKreator>();
        window.minSize = new Vector2(1000, 500);
        window.maxSize = new Vector2(1000, 501);
        window.Show();

        InterfaceUtility.ClearAllCache();

        battlersTextures = SystemDatas.GetBattlersPaths();
        InitStyles();

        elements = SystemDatas.GetMonsterPatterns();

        attackList = SystemDatas.GetAttacks();
    }

    public static void InitStyles() {
    }

    // Display
    public void OnGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(150));
        int value = EditorUtility.DisplayList<MonsterPattern>(selectedElement, elements, ref _scrollPosList);
        if (selectedElement != value) {
            Select(value);
        }
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add")) {
            elements.Add(new MonsterPattern());
            selectedElement = elements.Count - 1;
        }
        if (GUILayout.Button("Delete") && elements.Count > 1) {
            if (selectedElement == elements.Count - 1) {
                elements.RemoveAt(selectedElement);
                Select(elements.Count - 1);
            } else {
                elements[selectedElement] = new MonsterPattern();
            }
        }
        if (GUILayout.Button("Generate")) {
            GameObject monster = new GameObject();
            Monster.GenerateFromPattern(monster, elements[selectedElement], 0, 99);
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        if (elements.Count > 0 && selectedElement >= 0) {
            MonsterPattern mp = elements[selectedElement];
            mp.name = EditorGUILayout.TextField("Name", mp.name);

            mp.type = (Monster.Type)EditorGUILayout.EnumPopup("Type", mp.type);

            GUILayout.BeginHorizontal(GUILayout.Height(100));
            
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            int index = battlersTextures.IndexOf(mp.battleSprite);
            if (index < 0) index = 0;
            value = EditorGUILayout.Popup("Sprite", index, battlersTextures.ToArray(), GUILayout.MaxWidth(Screen.width));
            if (index != value) {
                string localpath = Config.GetResourcePath(Monster.IMAGE_FOLDER) + battlersTextures[value];
                previewBattleSprite = Resources.LoadAssetAtPath(localpath, typeof(Texture2D)) as Texture2D;
                mp.battleSprite = battlersTextures[value];
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label(previewBattleSprite);
            if (GUILayout.Button("Invert Preview"))
                previewBattleSprite = InterfaceUtility.InvertTexture(previewBattleSprite);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Label("- Initials Stats");
            mp.maxLife = EditorGUILayout.IntField("Life", mp.maxLife);
            mp.maxStamina = EditorGUILayout.IntField("Stamina", mp.maxStamina);
            mp.stat_might = EditorGUILayout.IntField("Might", mp.stat_might);
            mp.stat_resistance = EditorGUILayout.IntField("Resistance", mp.stat_resistance);
            mp.stat_speed = EditorGUILayout.IntField("Speed", mp.stat_speed);
            mp.stat_luck = EditorGUILayout.IntField("Luck", mp.stat_luck);
            mp.capture_rate = EditorGUILayout.FloatField("Capture Rate", mp.capture_rate);

            GUILayout.Label("- Attacks");
            List<string> attackListString = EditorUtility.ToStringList<Attack>(attackList);

            foreach(MonsterPattern.AttackLevelUp a in mp.attackLevelUp){
            GUILayout.BeginHorizontal();
            a.lvl = EditorGUILayout.IntField(a.lvl);
            int k = EditorGUILayout.Popup(attackList.IndexOf(a.attack),attackListString.ToArray());
            a.attack = attackList[k];
            GUILayout.EndHorizontal();
                }
            if (GUILayout.Button("Add")) {
               mp.attackLevelUp.Add(new MonsterPattern.AttackLevelUp(){attack = attackList[0]});
            }
            if (GUILayout.Button("Delete")) {
                mp.attackLevelUp.RemoveAt(mp.attackLevelUp.Count-1);
            }
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("OK")) {
            SystemDatas.SetMonsterPatterns(elements);
            Close();
        }
        GUILayout.EndHorizontal();
    }

    public void Select(int _select) {
        selectedElement = _select;
        if (elements[selectedElement].battleSprite == "" && battlersTextures.Count > 0)
            elements[selectedElement].battleSprite = battlersTextures[0];
        string localpath = Config.GetResourcePath(Monster.IMAGE_FOLDER) + elements[selectedElement].battleSprite;
        previewBattleSprite = Resources.LoadAssetAtPath(localpath, typeof(Texture2D)) as Texture2D;
    }
}