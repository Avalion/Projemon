using UnityEngine;

/**
 * this class is a MapObject to work on monster collection
 */
public class Computer : MapObject {
    public const Rect DISPLAY_POSITION_GENERAL = new Rect(1,0,150,300);

    public enum MenuPage { NULL, General, MonsterChoice };
    public MenuPage currentPage;

    private int _selectedMonster;
    private Vector2 scrollPos;


    public void OnGUI() {
        switch (currentPage) {
            case MenuPage.General: DisplayGeneralMenu(); return;
            case MenuPage.MonsterChoice: DisplayGeneralMenu(); return;
        }
    }

    public void DisplayGeneralMenu() {
        GUILayout.BeginArea(InterfaceUtility.GetScreenRelativeRect(DISPLAY_POSITION_GENERAL));
        InterfaceUtility.BeginBox(GUILayout.Height(DISPLAY_POSITION_GENERAL.height));

        if (GUILayout.Button("Monsters"))
            currentPage = MenuPage.MonsterChoice;
        if (GUILayout.Button("Exit"))
            currentPage = MenuPage.NULL;

        InterfaceUtility.EndBox();
        GUILayout.EndArea();
    }
    public void DisplayMonsterMenu() {
        InterfaceUtility.BeginBox(GUILayout.Height(Screen.height));
        GUILayout.BeginScrollView(scrollPos);
        GUILayout.BeginVertical();
        foreach (Monster monster in MonsterCollection.capturedMonsters) {
            if (Player.Current.monsters.Contains(monster))
                continue;

            GUILayout.BeginHorizontal();

            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        InterfaceUtility.EndBox();
    }

    public new void ExecuteActions() {
        if (currentPage == MenuPage.NULL)
            currentPage = MenuPage.General;
    }
}
