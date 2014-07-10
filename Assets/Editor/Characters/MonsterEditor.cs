using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

/**
 * This class design Monster Inspector
 */
[CustomEditor (typeof(Monster))]
public class MonsterEditor : Editor {
    Monster monster = null;

    string[] menus = new string[] { "Stats", "Design" };
    int selmenu;
    int sellvl;

    public override void OnInspectorGUI() {
        if (monster == null) {
            monster = target as Monster;
            if (Application.isEditor) {
                monster.CalcExpRequired();
                monster.life = monster.maxLife;
            }
            sellvl = monster.lvl;
        }

        selmenu = GUILayout.Toolbar(selmenu, menus, GUILayout.MaxWidth(Screen.width), GUILayout.MinWidth(Screen.width - 16));

        if (selmenu == 0)
            DisplayStats();
        else
            DrawDefaultInspector();
    }

    public void DisplayStats() {
        List<Attack> attackList = new List<Attack>() {new Attack()};
        attackList.AddRange(SystemDatas.GetAttacks());
       List<string> attackListString = EditorUtility.ToStringList<Attack>(attackList);

        EditorGUILayout.LabelField("Name", InterfaceUtility.TitleStyle);
        monster.monsterName = EditorGUILayout.TextField("Name", monster.monsterName);

        EditorGUILayout.LabelField("State", InterfaceUtility.TitleStyle);
        monster.state = (Monster.State)EditorGUILayout.EnumPopup("State", monster.state);

        EditorGUILayout.LabelField("Attacks", InterfaceUtility.TitleStyle);

        for (int i = 0; i < monster.attacks.Length; i++) {
                int k = EditorGUILayout.Popup(attackList.FindIndex(A => A.name == monster.attacks[i].name), attackListString.ToArray());
                monster.attacks[i] = attackList[k];           
        }        

        EditorGUILayout.LabelField("Experience", InterfaceUtility.TitleStyle);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("F1", GUILayout.Width(20));
        int F1 = Mathf.Clamp(EditorGUILayout.IntField(monster.expMultiplier1), 5, 60);
        if (F1 != monster.expMultiplier1) { monster.expMultiplier1 = F1; monster.CalcExpRequired(); }
        EditorGUILayout.LabelField("F2", GUILayout.Width(20));
        int F2 = Mathf.Clamp(EditorGUILayout.IntField(monster.expMultiplier2), 0, 60);
        if (F2 != monster.expMultiplier2) { monster.expMultiplier2 = F2; monster.CalcExpRequired(); }
        EditorGUILayout.LabelField("F3", GUILayout.Width(20));
        int F3 = Mathf.Clamp(EditorGUILayout.IntField(monster.expMultiplier3), 0, 60);
        if (F3 != monster.expMultiplier3) { monster.expMultiplier3 = F3; monster.CalcExpRequired(); }
        GUILayout.EndHorizontal();
        
        sellvl = EditorGUILayout.IntSlider(sellvl, 1, 98);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(sellvl + " -> " + (sellvl + 1));
        GUILayout.FlexibleSpace();
        EditorGUILayout.LabelField("need " + (monster.expRequired[sellvl - 1] - monster.exp));
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        EditorGUILayout.LabelField("Level Up", InterfaceUtility.TitleStyle);

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Life", GUILayout.Width(50));
        EditorGUILayout.LabelField("Min", GUILayout.Width(30));
        monster.lifeUp.x = Mathf.Max(EditorGUILayout.IntField((int)monster.lifeUp.x), 0);
        EditorGUILayout.LabelField("Max", GUILayout.Width(30));
        monster.lifeUp.y = Mathf.Max(EditorGUILayout.IntField((int)monster.lifeUp.y), monster.lifeUp.x);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Stamina", GUILayout.Width(50));
        EditorGUILayout.LabelField("Min", GUILayout.Width(30));
        monster.staminaUp.x = Mathf.Max(EditorGUILayout.IntField((int)monster.staminaUp.x), 0);
        EditorGUILayout.LabelField("Max", GUILayout.Width(30));
        monster.staminaUp.y = Mathf.Max(EditorGUILayout.IntField((int)monster.staminaUp.y), monster.staminaUp.x);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Refill Life", GUILayout.Width(85));
        monster.refillLife = EditorGUILayout.Toggle(monster.refillLife);
        GUILayout.EndHorizontal();
        GUILayout.Space(5);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Might", GUILayout.Width(70));
        EditorGUILayout.LabelField("Min", GUILayout.Width(30));
        monster.stat_mightUp.x = Mathf.Max(EditorGUILayout.IntField((int)monster.stat_mightUp.x), 0);
        EditorGUILayout.LabelField("Max", GUILayout.Width(30));
        monster.stat_mightUp.y = Mathf.Max(EditorGUILayout.IntField((int)monster.stat_mightUp.y), monster.stat_mightUp.x);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Resistance", GUILayout.Width(70));
        EditorGUILayout.LabelField("Min", GUILayout.Width(30));
        monster.stat_resistanceUp.x = Mathf.Max(EditorGUILayout.IntField((int)monster.stat_resistanceUp.x), 0);
        EditorGUILayout.LabelField("Max", GUILayout.Width(30));
        monster.stat_resistanceUp.y = Mathf.Max(EditorGUILayout.IntField((int)monster.stat_resistanceUp.y), monster.stat_resistanceUp.x);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Luck", GUILayout.Width(70));
        EditorGUILayout.LabelField("Min", GUILayout.Width(30));
        monster.stat_luckUp.x = Mathf.Max(EditorGUILayout.IntField((int)monster.stat_luckUp.x), 0);
        EditorGUILayout.LabelField("Max", GUILayout.Width(30));
        monster.stat_luckUp.y = Mathf.Max(EditorGUILayout.IntField((int)monster.stat_luckUp.y), monster.stat_luckUp.x);
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Speed", GUILayout.Width(70));
        EditorGUILayout.LabelField("Min", GUILayout.Width(30));
        monster.stat_speedUp.x = Mathf.Max(EditorGUILayout.IntField((int)monster.stat_speedUp.x), 0);
        EditorGUILayout.LabelField("Max", GUILayout.Width(30));
        monster.stat_speedUp.y = Mathf.Max(EditorGUILayout.IntField((int)monster.stat_speedUp.y), monster.stat_speedUp.x);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Reload")) {
            Monster.GenerateFromPattern(monster.gameObject, monster.monsterPattern, monster.lvl, monster.lvl);
        }
        GUILayout.Space(10);        

    }
}
