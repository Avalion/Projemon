using UnityEngine;
using System.Collections.Generic;

/**
 * this class is a MapObject to work on monster collection
 */
public class Computer : MapObject {
    public static Rect DISPLAY_POSITION_GENERAL = new Rect(1,0,0.1f,0.3f);
    public static Rect DISPLAY_POSITION_MONSTERS = new Rect(0.1f, 0.1f, 0.9f, 0.9f);
    public static Rect DISPLAY_POSITION_PLAYERMONSTERS = new Rect(0.7f, 0.5f, 0.2f, 0.4f);

    public enum MenuPage { NULL, General, MonsterChoice };
    public MenuPage currentPage;

    private List<Monster> displayableList = new List<Monster>();
    private Monster _selectedMonster;
    private Vector2 scrollPos = Vector2.zero;

    private static bool sortByType;
    private static Monster.Type displayedType = Monster.Type.Water;
    private static bool sortByName;
    private static char displayedChar = 'a';

    public void OnGUI() {
        switch (currentPage) {
            case MenuPage.General: 
                DisplayGeneralMenu(); return;
            case MenuPage.MonsterChoice: 
                DisplayMonsterMenu(); return;
        }
    }

    public void DisplayGeneralMenu() {
        Rect position = InterfaceUtility.GetScreenRelativeRect(DISPLAY_POSITION_GENERAL, true);
        GUILayout.BeginArea(position);
        InterfaceUtility.BeginBox(GUILayout.Width(position.width), GUILayout.Height(position.height));

        if (GUILayout.Button("Monsters"))
            currentPage = MenuPage.MonsterChoice;
        if (GUILayout.Button("Exit") || InputManager.Current.GetKey(KeyCode.Escape)) {
            currentPage = MenuPage.NULL;
            Player.Unlock();
        }
        
        GUILayout.FlexibleSpace();
        InterfaceUtility.EndBox();
        GUILayout.EndArea();
    }
    public void DisplayMonsterMenu() {
        Rect position = InterfaceUtility.GetScreenRelativeRect(DISPLAY_POSITION_MONSTERS, true);
        GUILayout.BeginArea(position);
        InterfaceUtility.BeginBox(GUILayout.Height(Screen.height));
        GUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginVertical();
        int count = 0;
        foreach (Monster monster in displayableList) {
            ++count;
            if (count % 3 == 1)
                GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(monster.monsterName, monster.miniSprite))) {
                if (_selectedMonster == null || displayableList.Contains(_selectedMonster))
                    _selectedMonster = monster;
                else {
                    Monster m = MonsterCollection.capturedMonsters[MonsterCollection.capturedMonsters.IndexOf(monster)];
                    MonsterCollection.capturedMonsters[MonsterCollection.capturedMonsters.IndexOf(monster)] = _selectedMonster;
                    Player.Current.monsters[Player.Current.monsters.IndexOf(_selectedMonster)] = m;

                    _selectedMonster = null;
                }
            }
            if (count % 3 == 0)
                GUILayout.EndHorizontal();
        }
        if (count % 3 > 0)
            GUILayout.EndHorizontal();

        GUILayout.EndVertical();
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Exit") || InputManager.Current.GetKey(KeyCode.Escape))
            currentPage = MenuPage.General;
        GUILayout.EndHorizontal();

        InterfaceUtility.EndBox();
        GUILayout.EndArea();
        DisplayMonsterPlayerList();
    }
    public void DisplayMonsterPlayerList() {
        Rect position = InterfaceUtility.GetScreenRelativeRect(DISPLAY_POSITION_PLAYERMONSTERS, true);

        GUILayout.BeginArea(position);
        InterfaceUtility.BeginBox(GUILayout.Width(position.width), GUILayout.Height(position.height));

        for (int i = 0; i < Player.MAX_TEAM_NUMBER; ++i) {
            if (Player.Current.monsters.Count > i) {
                if (GUILayout.Button(new GUIContent(Player.Current.monsters[i].monsterName, Player.Current.monsters[i].miniSprite))) {
                    if (_selectedMonster == null || Player.Current.monsters.Contains(_selectedMonster))
                        _selectedMonster = Player.Current.monsters[i];
                    else {    
                        Monster m = Player.Current.monsters[i];
                        Player.Current.monsters[i] = _selectedMonster;
                        MonsterCollection.capturedMonsters[MonsterCollection.capturedMonsters.IndexOf(_selectedMonster)] = m;

                        _selectedMonster = null;
                    }
                }
            } else {
                if (GUILayout.Button(new GUIContent())) {
                    Player.Current.monsters.Add(_selectedMonster);
                    MonsterCollection.capturedMonsters.Remove(_selectedMonster);

                    _selectedMonster = null;
                }
            }
        }

        InterfaceUtility.EndBox();
        GUILayout.EndArea();
    }


    public override void ExecuteActions() {
        Player.Lock();

        RefreshElements();

        if (currentPage == MenuPage.NULL)
            currentPage = MenuPage.General;
    }

    public void RefreshElements() {
        displayableList.Clear();

        if (sortByType) {
            foreach (Monster m in MonsterCollection.capturedMonsters) {
                if (m.type == displayedType)
                    displayableList.Add(m);
            }
        } else if (sortByName) {
            foreach (Monster m in MonsterCollection.capturedMonsters) {
                if (m.monsterPattern.name.ToLower()[0] == displayedChar)
                    displayableList.Add(m);
            }
        }

        displayableList.Sort(delegate(Monster m1, Monster m2) { return m1.monsterPattern.name.CompareTo(m1.monsterPattern.name); });
    }
}
