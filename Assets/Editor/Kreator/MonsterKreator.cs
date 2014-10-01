﻿using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

/**
 * This class provides gestion of MonsterPattern
 */
public class MonsterKreator : EditorWindow {
    public static List<DBMonsterPattern> elements = new List<DBMonsterPattern>();
    private int selectedElement = -1;
    private DBMonsterPattern current { get { return elements[selectedElement]; } }

    private static int numberElements = 0;

    public static List<string> battlersTextures = new List<string>();
    public static Texture2D previewBattleSprite;

    public static List<Attack> attackList = new List<Attack>();

    private Vector2 _scrollPosList = Vector2.zero;

    
    // Launch
    [MenuItem("Creation/Monsters")]
    public static void Init() {
        MonsterKreator window = EditorWindow.GetWindow<MonsterKreator>();
        window.minSize = new Vector2(1000, 500);
        window.maxSize = new Vector2(1000, 501);
        window.Show();

        if (!DataBase.IsConnected) DataBase.Connect(Application.dataPath + "/database.sql");

        InterfaceUtility.ClearAllCache();

        battlersTextures = SystemDatas.GetBattlersPaths();
        
        InitStyles();

        elements = DataBase.Select<DBMonsterPattern>();
        numberElements = elements.Count;

        attackList = SystemDatas.GetAttacks();
    }

    public static void InitStyles() {
    }

    // Display
    public void OnGUI() {
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical(GUILayout.Width(150));
        int value = EditorUtility.DisplayList<DBMonsterPattern>(selectedElement, elements, ref _scrollPosList);
        if (selectedElement != value) {
            Select(value);
        }
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add")) {
            elements.Add(new DBMonsterPattern() { ID = elements.Count });
            selectedElement = elements.Count - 1;
            numberElements = elements.Count;
        }
        if (GUILayout.Button("Delete") && elements.Count > 1) {
            if (selectedElement == elements.Count - 1) {
                elements.RemoveAt(selectedElement);
                Select(elements.Count - 1);
                numberElements = elements.Count;
            } else {
                elements[selectedElement] = new DBMonsterPattern();
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        GUILayout.Label("Nb ");
        numberElements = EditorGUILayout.IntField(numberElements);
        if (GUILayout.Button("Apply")) {
            while (elements.Count < numberElements)
                elements.Add(new DBMonsterPattern() { ID = elements.Count });
            while (elements.Count > numberElements)
                elements.RemoveAt(elements.Count - 1);
            Select(Mathf.Min(selectedElement, numberElements) - 1);
        }
        GUILayout.EndHorizontal();

        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        if (elements.Count > 0 && selectedElement >= 0) {
            DBMonsterPattern mp = elements[selectedElement];
            mp.name = EditorGUILayout.TextField("Name", mp.name);

            mp.type = (Monster.Type)EditorGUILayout.EnumPopup("Type", mp.type);

            GUILayout.BeginHorizontal(GUILayout.Height(100));
            
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();
            int index = battlersTextures.IndexOf(mp.battleSprite);
            if (index < 0) index = 0;
            value = EditorGUILayout.Popup("Sprite", index, battlersTextures.ToArray(), GUILayout.MaxWidth(Screen.width));
            if (index != value || mp.battleSprite != battlersTextures[value]) {
                string localpath = Config.GetResourcePath(Monster.IMAGE_FOLDER) + battlersTextures[value];
                previewBattleSprite = Resources.LoadAssetAtPath(localpath, typeof(Texture2D)) as Texture2D;
                mp.battleSprite = battlersTextures[value];
            }
            index = battlersTextures.IndexOf(mp.miniSprite);
            if (index < 0) index = 0;
            value = EditorGUILayout.Popup("MiniSprite", index, battlersTextures.ToArray(), GUILayout.MaxWidth(Screen.width));
            if (index != value || mp.miniSprite != battlersTextures[value]) {
                string localpath = Config.GetResourcePath(Monster.IMAGE_FOLDER) + battlersTextures[value];
                previewBattleSprite = Resources.LoadAssetAtPath(localpath, typeof(Texture2D)) as Texture2D;
                mp.miniSprite = battlersTextures[value];
            }
            
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            GUILayout.Label(previewBattleSprite);
            if (GUILayout.Button("Invert Preview"))
                previewBattleSprite = InterfaceUtility.InvertTexture(previewBattleSprite);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            GUILayout.Label("- Stats");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Life : ", GUILayout.Width(100));
            GUILayout.Label("Start : ");
            mp.start_life = EditorGUILayout.IntField(mp.start_life, GUILayout.Width(40));
            GUILayout.Label("Lvl Up : ");
            mp.lifeUp.x = EditorGUILayout.IntField((int)mp.lifeUp.x, GUILayout.Width(40));
            mp.lifeUp.y = EditorGUILayout.IntField((int)mp.lifeUp.y, GUILayout.Width(40));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Stamina : ", GUILayout.Width(100));
            GUILayout.Label("Start : ");
            mp.start_stamina = EditorGUILayout.IntField(mp.start_stamina, GUILayout.Width(40));
            GUILayout.Label("Lvl Up : ");
            mp.staminaUp.x = EditorGUILayout.IntField((int)mp.staminaUp.x, GUILayout.Width(40));
            mp.staminaUp.y = EditorGUILayout.IntField((int)mp.staminaUp.y, GUILayout.Width(40));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Might : ", GUILayout.Width(100));
            GUILayout.Label("Start : ");
            mp.start_might = EditorGUILayout.IntField(mp.start_might, GUILayout.Width(40));
            GUILayout.Label("Lvl Up : ");
            mp.mightUp.x = EditorGUILayout.IntField((int)mp.mightUp.x, GUILayout.Width(40));
            mp.mightUp.y = EditorGUILayout.IntField((int)mp.mightUp.y, GUILayout.Width(40));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Resistance : ", GUILayout.Width(100));
            GUILayout.Label("Start : ");
            mp.start_resistance = EditorGUILayout.IntField(mp.start_resistance, GUILayout.Width(40));
            GUILayout.Label("Lvl Up : ");
            mp.resistanceUp.x = EditorGUILayout.IntField((int)mp.resistanceUp.x, GUILayout.Width(40));
            mp.resistanceUp.y = EditorGUILayout.IntField((int)mp.resistanceUp.y, GUILayout.Width(40));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Luck : ", GUILayout.Width(100));
            GUILayout.Label("Start : ");
            mp.start_luck = EditorGUILayout.IntField(mp.start_luck, GUILayout.Width(40));
            GUILayout.Label("Lvl Up : ");
            mp.luckUp.x = EditorGUILayout.IntField((int)mp.luckUp.x, GUILayout.Width(40));
            mp.luckUp.y = EditorGUILayout.IntField((int)mp.luckUp.y, GUILayout.Width(40));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Speed : ", GUILayout.Width(100));
            GUILayout.Label("Start : ");
            mp.start_speed = EditorGUILayout.IntField(mp.start_speed, GUILayout.Width(40));
            GUILayout.Label("Lvl Up : ");
            mp.speedUp.x = EditorGUILayout.IntField((int)mp.speedUp.x, GUILayout.Width(40));
            mp.speedUp.y = EditorGUILayout.IntField((int)mp.speedUp.y, GUILayout.Width(40));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            
            mp.capture_rate = EditorGUILayout.FloatField("Capture Rate", mp.capture_rate);

            //GUILayout.Label("- Attacks");
            //List<string> attackListString = EditorUtility.ToStringList<Attack>(attackList);

            //foreach(MonsterPattern.AttackLevelUp a in mp.attackLevelUp) {
            //    GUILayout.BeginHorizontal();
            //    a.lvl = EditorGUILayout.IntField(a.lvl);
            //    int k = EditorGUILayout.Popup(attackList.IndexOf(a.attack),attackListString.ToArray());
            //    a.attack = attackList[k];
            //    GUILayout.EndHorizontal();
            //}
            //if (GUILayout.Button("Add")) {
            //   mp.attackLevelUp.Add(new MonsterPattern.AttackLevelUp(){attack = attackList[0]});
            //}
            //if (GUILayout.Button("Delete")) {
            //    mp.attackLevelUp.RemoveAt(mp.attackLevelUp.Count-1);
            //}
        }
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("OK")) {
            if (elements.Find(P => P.name == null) != null) {
                Debug.LogError("You can't save if some élements have no name !");
            } else {
                foreach (DBMonsterPattern pattern in elements)
                    DataBase.Replace<DBMonsterPattern>(pattern);
                Close();
            }
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