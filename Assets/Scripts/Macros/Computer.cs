using UnityEngine;
using System.Collections.Generic;

/**
 * this class is a MapObject to work on monster collection
 */
public class Computer : MapObject {
//    public static Rect DISPLAY_POSITION_GENERAL = new Rect(1,0,0.1f,0.3f);
//    public static Rect DISPLAY_POSITION_MONSTERS = new Rect(0.1f, 0.1f, 0.9f, 0.9f);
//    public static Rect DISPLAY_POSITION_PLAYERMONSTERS = new Rect(0.7f, 0.5f, 0.2f, 0.4f);

//    public enum MenuPage { NULL, General, MonsterChoice };
//    public MenuPage currentPage;

//    private List<Monster> displayableList = new List<Monster>();
//    private Monster _selectedMonster;
//    private Vector2 scrollPos = Vector2.zero;

//    // Sort
    
//    public void OnGUI() {
//        switch (currentPage) {
//            case MenuPage.General: 
//                DisplayGeneralMenu(); return;
//            case MenuPage.MonsterChoice: 
//                DisplayMonsterMenu(); return;
//        }
//    }

//    public void DisplayGeneralMenu() {
//        Rect position = InterfaceUtility.GetScreenRelativeRect(DISPLAY_POSITION_GENERAL, true);
//        GUILayout.BeginArea(position);
//        InterfaceUtility.BeginBox(GUILayout.Width(position.width), GUILayout.Height(position.height));

//        if (GUILayout.Button("Monsters")) {
//            currentPage = MenuPage.MonsterChoice;
//            RefreshElements();
//        }
//        if (GUILayout.Button("Exit") || InputManager.Current.GetKey(KeyCode.Escape)) {
//            currentPage = MenuPage.NULL;
//            Player.Unlock();
//        }
        
//        GUILayout.FlexibleSpace();
//        InterfaceUtility.EndBox();
//        GUILayout.EndArea();
//    }
//    public void DisplayMonsterMenu() {
//        Rect position = InterfaceUtility.GetScreenRelativeRect(DISPLAY_POSITION_MONSTERS, true);
//        GUILayout.BeginArea(position);
//        InterfaceUtility.BeginBox(GUILayout.Height(Screen.height));
        
//        GUILayout.BeginHorizontal();
//        GUILayout.Label("Tri par : ");
//        List<GUIContent> contents = new List<GUIContent>();
//        foreach (string s in System.Enum.GetNames(typeof(SortMode)))
//            contents.Add(new GUIContent(s));
//        var v1 = (SortMode)GUILayout.Toolbar((int)sortMode, contents.ToArray());
//        if (sortMode != v1) {
//            sortMode = v1;
//            RefreshElements();
//        }
//        GUILayout.EndHorizontal();

//        switch (sortMode) {
//            case SortMode.Type:
//                contents = new List<GUIContent>();
//                foreach (Monster.Type type in System.Enum.GetValues(typeof(Monster.Type)))
//                    contents.Add(new GUIContent(Monster.GetTypeIcon(type)));
//                var v2 = (Monster.Type)GUILayout.Toolbar((int)sortTypeValue, contents.ToArray());
//                if (sortTypeValue != v2) {
//                    sortTypeValue = v2;
//                    RefreshElements();
//                }
//                break;
//            case SortMode.Name:
//                contents = new List<GUIContent>() { };
//                for (int i = 65; i <= 90; ++i) // A to Z
//                    contents.Add(new GUIContent(((char)i).ToString()));

//                var v3 = (char)(65 + GUILayout.Toolbar((int)sortCharValue - 65, contents.ToArray()));
//                if (sortCharValue != v3) {
//                    sortCharValue = v3;
//                    RefreshElements();
//                }
//                break;
//        }
        
//        GUILayout.BeginScrollView(scrollPos);
//        GUILayout.BeginVertical();
//        int count = 0;
//        foreach (Monster monster in displayableList) {
//            ++count;
//            if (count % 3 == 1)
//                GUILayout.BeginHorizontal();
//            if (GUILayout.Button(new GUIContent(monster.monsterName, monster.miniSprite))) {
//                if (_selectedMonster == null || displayableList.Contains(_selectedMonster))
//                    _selectedMonster = monster;
//                else {
//                    Monster m = MonsterCollection.capturedMonsters[MonsterCollection.capturedMonsters.IndexOf(monster)];
//                    MonsterCollection.capturedMonsters[MonsterCollection.capturedMonsters.IndexOf(monster)] = _selectedMonster;
//                    Player.Current.monsters[Player.Current.monsters.IndexOf(_selectedMonster)] = m;

//                    _selectedMonster = null;

//                    RefreshElements();
//                }
//            }
//            if (count % 3 == 0)
//                GUILayout.EndHorizontal();
//        }
//        if (count % 3 > 0)
//            GUILayout.EndHorizontal();

//        GUILayout.EndVertical();
//        GUILayout.EndScrollView();

//        GUILayout.BeginHorizontal();
//        GUILayout.FlexibleSpace();
//        if (GUILayout.Button("Exit") || InputManager.Current.GetKey(KeyCode.Escape))
//            currentPage = MenuPage.General;
//        GUILayout.EndHorizontal();

//        InterfaceUtility.EndBox();
//        GUILayout.EndArea();


//        DisplayMonsterPlayerList();
//    }
//    public void DisplayMonsterPlayerList() {
//        Rect position = InterfaceUtility.GetScreenRelativeRect(DISPLAY_POSITION_PLAYERMONSTERS, true);

//        GUILayout.BeginArea(position);
//        InterfaceUtility.BeginBox(GUILayout.Width(position.width), GUILayout.Height(position.height));

//        for (int i = 0; i < Player.MAX_TEAM_NUMBER; ++i) {
//            if (Player.Current.monsters.Count > i) {
//                if (GUILayout.Button(new GUIContent(Player.Current.monsters[i].monsterName, Player.Current.monsters[i].miniSprite))) {
//                    if (_selectedMonster == null || Player.Current.monsters.Contains(_selectedMonster))
//                        _selectedMonster = Player.Current.monsters[i];
//                    else {    
//                        Monster m = Player.Current.monsters[i];
//                        Player.Current.monsters[i] = _selectedMonster;
//                        MonsterCollection.capturedMonsters[MonsterCollection.capturedMonsters.IndexOf(_selectedMonster)] = m;

//                        _selectedMonster = null;

//                        RefreshElements();
//                    }
//                }
//            } else {
//                if (GUILayout.Button(new GUIContent())) {
//                    Player.Current.monsters.Add(_selectedMonster);
//                    MonsterCollection.capturedMonsters.Remove(_selectedMonster);

//                    _selectedMonster = null;

//                    RefreshElements();
//                }
//            }
//        }

//        InterfaceUtility.EndBox();
//        GUILayout.EndArea();
//    }


//    public override void ExecuteActions() {
//        Player.Lock();

//        RefreshElements();

//        if (currentPage == MenuPage.NULL)
//            currentPage = MenuPage.General;
//    }

//    public void RefreshElements() {
//        displayableList.Clear();

//        switch (sortMode) {
//            case SortMode.NULL:
//                displayableList = new List<Monster>(MonsterCollection.capturedMonsters); break;
//            case SortMode.Type:
//                foreach (Monster m in MonsterCollection.capturedMonsters) {
//                    if (m.type == sortTypeValue)
//                        displayableList.Add(m);
//                }
//                break;
//            case SortMode.Name:
//                foreach (Monster m in MonsterCollection.capturedMonsters) {
//                    if (m.monsterPattern.name.ToLower()[0] == sortCharValue)
//                        displayableList.Add(m);
//                }
//                break;
//        }

//        displayableList.Sort(delegate(Monster m1, Monster m2) { return m1.monsterPattern.name.CompareTo(m1.monsterPattern.name); });
//    }
}
