using UnityEngine;

/**
 * this class is a MapObject to work on monster collection
 */
public class Computer : MapObject {
    public static Rect DISPLAY_POSITION_GENERAL = new Rect(1,0,150,300); // Rect class cannot be a const...

    public enum MenuPage { NULL, General, MonsterChoice, MonsterChoicePlayerList };
    public MenuPage currentPage;

    private int _selectedMonster;
    private Vector2 scrollPos = Vector2.zero;


    public void OnGUI() {
        switch (currentPage) {
            case MenuPage.General: 
                DisplayGeneralMenu(); return;
            case MenuPage.MonsterChoice: 
            case MenuPage.MonsterChoicePlayerList: 
                DisplayMonsterMenu(); return;
        }
    }

    public void DisplayGeneralMenu() {
        GUILayout.BeginArea(InterfaceUtility.GetScreenRelativeRect(DISPLAY_POSITION_GENERAL));
        InterfaceUtility.BeginBox(GUILayout.Height(DISPLAY_POSITION_GENERAL.height));

        if (GUILayout.Button("Monsters"))
            currentPage = MenuPage.MonsterChoice;
        if (GUILayout.Button("Exit")) {
            currentPage = MenuPage.NULL;
            Player.Unlock();
        }
        
        GUILayout.FlexibleSpace();
        InterfaceUtility.EndBox();
        GUILayout.EndArea();
    }
    public void DisplayMonsterMenu() {
        GUI.enabled = currentPage == MenuPage.MonsterChoice;

        InterfaceUtility.BeginBox(GUILayout.Height(Screen.height));
        GUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginVertical();
        foreach (Monster monster in MonsterCollection.capturedMonsters) {
            if (Player.Current.monsters.Contains(monster))
                continue;

            GUILayout.BeginHorizontal();
            if (GUILayout.Button(new GUIContent(monster.monsterName, monster.miniSprite))) {
                _selectedMonster = MonsterCollection.capturedMonsters.IndexOf(monster);
                currentPage = MenuPage.MonsterChoicePlayerList;
            }
            GUILayout.EndHorizontal();

        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("Exit"))
            currentPage = MenuPage.General;
        GUILayout.EndHorizontal();

        InterfaceUtility.EndBox();

        GUI.enabled = true;
        if (currentPage == MenuPage.MonsterChoicePlayerList)
            DisplayMonsterPlayerList();
    }
    public void DisplayMonsterPlayerList() {
        GUILayout.BeginArea(DISPLAY_POSITION_GENERAL);
        InterfaceUtility.BeginBox(GUILayout.Height(DISPLAY_POSITION_GENERAL.height));

        for (int i = 0; i < Player.MAX_TEAM_NUMBER; ++i) {
            if (Player.Current.monsters.Count > i) {
                if (GUILayout.Button(new GUIContent(Player.Current.monsters[i].monsterName, Player.Current.monsters[i].miniSprite))) {
                    Monster m = Player.Current.monsters[i];
                    Player.Current.monsters[i] = MonsterCollection.capturedMonsters[_selectedMonster];
                    MonsterCollection.capturedMonsters[_selectedMonster] = m;

                    currentPage = MenuPage.MonsterChoice;
                    _selectedMonster = -1;
                }
            } else {
                if (GUILayout.Button(new GUIContent())) {
                    Player.Current.monsters.Add(MonsterCollection.capturedMonsters[_selectedMonster]);
                    MonsterCollection.capturedMonsters.RemoveAt(_selectedMonster);

                    _selectedMonster = -1;
                    currentPage = MenuPage.MonsterChoice;
                }
            }
        }

        InterfaceUtility.EndBox();
        GUILayout.EndArea();
    }


    public override void ExecuteActions() {
        Player.Lock();
        
        if (currentPage == MenuPage.NULL)
            currentPage = MenuPage.General;
    }
}
