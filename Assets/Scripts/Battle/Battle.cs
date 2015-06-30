using UnityEngine;
using System.Collections.Generic;
using System.Collections;

/**
 * This class manage battles
 */
public class Battle : MonoBehaviour {
    public const float MESSAGE_SPEED = 1.5f;

    private static Battle current = null;
    public static Battle Current {
        get {
            if (current == null) {
                current = GameObject.FindObjectOfType<Battle>();
            }
            return current;
        }
    }

    private static bool locked = false;
    public static void Lock() { locked = true; }
    public static void Unlock() { locked = false; }

    public Battler enemy;
    public MapObjectAction action;

    private GUIStyle messageStyle = new GUIStyle();
    private GUIStyle lifeStyle = new GUIStyle();
    private GUIStyle monsterBoxStyle = new GUIStyle();
    private GUIStyle monsterBoxCurrentStyle = new GUIStyle();

    private List<string> messages = new List<string>();
    public string Message { 
        get { return messages.Count > 0 ? messages[0] : ""; }
        set { if (value == "") messages.RemoveAt(0); else messages.Add(value); time = Time.time; }
    }
    private static float time;

    private enum DisplayMode { Choice, Attack, SwitchMonster, Items};
    private DisplayMode displayMode = DisplayMode.Choice;


    // Static constructors
    public static void Launch(MapObjectAction _action, Battler _enemy) {
        Battle b = new GameObject("Battle").AddComponent<Battle>();
        b.action = _action;
        Player.Current.activeMonster = 0;
        b.enemy = _enemy;
        b.enemy.activeMonster = 0;
        World.ShowMap = false;
    }

    // Start
    public void Start() {
        messageStyle = new GUIStyle();
        messageStyle.normal.textColor = Color.white;
        messageStyle.fontSize = 25;
        lifeStyle = new GUIStyle(messageStyle);
        lifeStyle.alignment = TextAnchor.UpperRight;
        monsterBoxStyle = new GUIStyle();
        monsterBoxStyle.normal.background = InterfaceUtility.GetTexture(Config.GetResourcePath("System/Box") + "monsterBoxBackground.png");
        monsterBoxStyle.padding = new RectOffset(7, 7, 7, 7);
        monsterBoxCurrentStyle = new GUIStyle(monsterBoxStyle);
        monsterBoxCurrentStyle.normal.background = InterfaceUtility.GetTexture(Config.GetResourcePath("System/Box") + "monsterBoxCurrentBackground.png");
    }

    // Update
    public void Update() {
        if (enemy.monsters[enemy.activeMonster].life <= 0)
            Win();

        if (Player.Current.monsters[Player.Current.activeMonster].life <= 0)
            Lose();
    }

    // End
    public void Win() {
        Lock();
        if (enemy != null) {
            Message = "Vous avez battu " + enemy.name + " !";
            Message = "Vous recevez " + enemy.goldCount + " PO !";
            Player.Current.goldCount += enemy.goldCount;
        }
        
        StartCoroutine(Dispose());
    }

    public void Lose() {
        Lock();

        Message = "You Lose !!!";

        StartCoroutine(Dispose());
    }

    public IEnumerator Dispose() {
        while (Message != "")
            yield return new WaitForEndOfFrame();
        
        World.ShowMap = true;
        Destroy(gameObject);
        Unlock();
        action.Terminate();
    }

    // Display
    public void OnGUI() {
        GUILayout.BeginArea(new Rect(0, 0, Screen.width, Screen.height));
        GUILayout.BeginVertical();
        if (displayMode == DisplayMode.Choice) {
            DisplayMessage();
            DisplayBattleArea();
            DisplayBottomPanel();
        }else if (displayMode==DisplayMode.SwitchMonster){
            DisplaySwitchMonsterMenu();
        } else if (displayMode == DisplayMode.Attack) {
            DisplayMessage();
            DisplayBattleArea();
            DisplayAttackPanel();
        } else if (displayMode == DisplayMode.Items) {
            DisplayMessage();
            DisplayBattleArea();
            DisplayItemsPanel();
        }
        GUILayout.EndVertical();
        GUILayout.EndArea();
    }

    #region Battle Mode

    public void DisplayMessage() {
        if (Message != "") {
            InterfaceUtility.BeginBox(GUILayout.Height(70));
            GUILayout.FlexibleSpace();
            GUILayout.Label(Message, messageStyle);
            if (Event.current.type == EventType.MouseDown || Time.time > time + MESSAGE_SPEED && Event.current.type == EventType.Repaint)
                Message = "";
            GUILayout.FlexibleSpace();
            InterfaceUtility.EndBox();
        } else {
            GUILayout.Space(70);
        }
    }

    public void DisplayBattleArea() {
        GUILayout.BeginVertical();
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.Space(100);
        DisplayBattleMonster(Player.Current.monsters[Player.Current.activeMonster], false, true);
        GUILayout.FlexibleSpace();
        DisplayBattleMonster(enemy.monsters[enemy.activeMonster], true, false);
        GUILayout.Space(100);
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }

    public void DisplayBottomPanel() {
        int menuHeight = 150;

        GUI.enabled = !locked;
        
        GUILayout.BeginHorizontal();
        int choice = InterfaceUtility.DisplayMenu(new List<GUIContent>() { new GUIContent("Attack"), new GUIContent("Switch Monster"), new GUIContent("Items")}, GUILayout.Width(200), GUILayout.Height(menuHeight));
        if (choice == 0) {
            displayMode = DisplayMode.Attack;
        }
        if (choice == 1) {
            displayMode = DisplayMode.SwitchMonster;

        }

        if (choice == 2) {
            displayMode = DisplayMode.Items;
        }

        GUI.enabled = true;
        InterfaceUtility.BeginBox(GUILayout.Height(menuHeight));
        GUILayout.BeginHorizontal();
        GUILayout.Label(Monster.GetTypeIcon(Player.Current.monsters[Player.Current.activeMonster].type), InterfaceUtility.LabelStyle);
        GUILayout.Label(Player.Current.monsters[Player.Current.activeMonster].monsterName, messageStyle);
        GUILayout.FlexibleSpace();

        InterfaceUtility.ProgressBar(200, 20, Player.Current.monsters[Player.Current.activeMonster].life, Player.Current.monsters[Player.Current.activeMonster].maxLife, InterfaceUtility.ColorToTexture(Color.green), InterfaceUtility.ColorToTexture(Color.gray));
        GUILayout.Label(Player.Current.monsters[Player.Current.activeMonster].life + "/" + Player.Current.monsters[Player.Current.activeMonster].maxLife, lifeStyle, GUILayout.Width(120));
        GUILayout.EndHorizontal();
        InterfaceUtility.EndBox();

        GUILayout.EndHorizontal();
    }

    public void DisplayAttackPanel() {
        int menuHeight = 150;
        DBAttack[] attacks = Player.Current.monsters[Player.Current.activeMonster].attacks.ToArray();
        List<GUIContent> list = new List<GUIContent>();

        foreach (DBAttack a in attacks) {
            if (a == null)
                continue;
            if (a.name != "null")
                list.Add(new GUIContent(a.name));
        }
        list.Add(new GUIContent("Cancel"));

        GUILayout.BeginHorizontal();
        int choice = InterfaceUtility.DisplayMenu(list, GUILayout.Width(200), GUILayout.Height(menuHeight));
        if (choice >= 0 && choice < list.Count - 1) {
            // TODO : place Battle Animation in 2D.
            attacks[choice].Launch(Player.Current.monsters[Player.Current.activeMonster], enemy.monsters[enemy.activeMonster], new Rect(0,0,Screen.width, Screen.height));
        }
        if (choice == list.Count - 1)
            displayMode = DisplayMode.Choice;
        GUI.enabled = true;
        InterfaceUtility.BeginBox(GUILayout.Height(menuHeight));
        GUILayout.BeginHorizontal();
        GUILayout.Label(Monster.GetTypeIcon(Player.Current.monsters[Player.Current.activeMonster].type), InterfaceUtility.LabelStyle);
        GUILayout.Label(Player.Current.monsters[Player.Current.activeMonster].monsterName, messageStyle);
        GUILayout.FlexibleSpace();

        InterfaceUtility.ProgressBar(200, 20, Player.Current.monsters[Player.Current.activeMonster].life, Player.Current.monsters[Player.Current.activeMonster].maxLife, InterfaceUtility.ColorToTexture(Color.green), InterfaceUtility.ColorToTexture(Color.gray));
        GUILayout.Label(Player.Current.monsters[Player.Current.activeMonster].life + "/" + Player.Current.monsters[Player.Current.activeMonster].maxLife, lifeStyle, GUILayout.Width(120));
        GUILayout.EndHorizontal();
        InterfaceUtility.EndBox();

        GUILayout.EndHorizontal();
    }

    public void DisplayItemsPanel() {
        int menuHeight = 150;
        List<GUIContent> list = new List<GUIContent>();

        foreach (var a in Player.Current.inventory) {
            System.Diagnostics.Debug.Assert(a.Value > 0);
            list.Add(new GUIContent(a.Key));
        }
        list.Add(new GUIContent("Cancel"));

        GUILayout.BeginHorizontal();
        int choice = InterfaceUtility.DisplayMenu(list, GUILayout.Width(200), GUILayout.Height(menuHeight));
 //       if (choice >= 0 && choice < list.Count - 1)
 //           Player.Current.inventory;
        if (choice == list.Count - 1)
            displayMode = DisplayMode.Choice;
        GUI.enabled = true;
        InterfaceUtility.BeginBox(GUILayout.Height(menuHeight));
        GUILayout.BeginHorizontal();
        GUILayout.Label(Monster.GetTypeIcon(Player.Current.monsters[Player.Current.activeMonster].type), InterfaceUtility.LabelStyle);
        GUILayout.Label(Player.Current.monsters[Player.Current.activeMonster].monsterName, messageStyle);
        GUILayout.FlexibleSpace();

        InterfaceUtility.ProgressBar(200, 20, Player.Current.monsters[Player.Current.activeMonster].life, Player.Current.monsters[Player.Current.activeMonster].maxLife, InterfaceUtility.ColorToTexture(Color.green), InterfaceUtility.ColorToTexture(Color.gray));
        GUILayout.Label(Player.Current.monsters[Player.Current.activeMonster].life + "/" + Player.Current.monsters[Player.Current.activeMonster].maxLife, lifeStyle, GUILayout.Width(120));
        GUILayout.EndHorizontal();
        InterfaceUtility.EndBox();

        GUILayout.EndHorizontal();
    }

    #endregion
    // Switch Monster Mode
    public void DisplaySwitchMonsterMenu() {
        InterfaceUtility.BeginBox(GUILayout.Height(Screen.height));
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal(GUILayout.Height(Screen.height*0.3f));
        GUILayout.FlexibleSpace();
        DisplayMonsterBox(0.48f, 0.12f, enemy.monsters[enemy.activeMonster], true);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        
        for (int i = 0; i < Player.Current.monsters.Count; i++)
        {
            if(MathUtility.IsPair(i)) {
                GUILayout.BeginHorizontal();
            } else {
                GUILayout.BeginVertical();
                GUILayout.Space(20);
            }
            DisplayMonsterBox(0.48f, 0.12f, Player.Current.monsters[i], Player.Current.activeMonster == i);
            if(!MathUtility.IsPair(i)){
                GUILayout.Space(-20);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();    
            }
        }
        if (!MathUtility.IsPair(Player.Current.monsters.Count))
        {
            GUILayout.EndHorizontal();
        }
        GUILayout.FlexibleSpace();
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if(GUILayout.Button("Cancel")){
            displayMode = DisplayMode.Choice;
        }
        GUILayout.EndHorizontal();
        InterfaceUtility.EndBox();
    }

    #region Boxes (Group of repetitive display)

    public void DisplayBattleMonster(Monster monster, bool displaylife, bool inverted) {
        GUILayout.BeginVertical(GUILayout.Width(monster.battleSprite.width));
        if (displaylife) {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            InterfaceUtility.ProgressBar(100, 5, monster.life, monster.maxLife, InterfaceUtility.ColorToTexture(Color.green), InterfaceUtility.ColorToTexture(Color.gray));
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        } else
            GUILayout.Space(5);
        GUILayout.Label(inverted ? InterfaceUtility.InvertTexture(monster.battleSprite) : monster.battleSprite);
        GUILayout.EndVertical();
    }

    public void DisplayMonsterBox(float width, float height, Monster monster, bool selected) {
        GUIContent content = new GUIContent(monster.monsterName, Monster.GetTypeIcon(monster.type));
        GUILayout.BeginHorizontal(selected ? monsterBoxCurrentStyle : monsterBoxStyle, GUILayout.Width(width * Screen.width), GUILayout.Height(height * Screen.height));
        GUILayout.Label(monster.battleSprite, GUILayout.Width(50), GUILayout.Height(50));
        GUILayout.BeginVertical();
        if (GUILayout.Button(content, messageStyle)&& !selected) {
            Player.Current.activeMonster = Player.Current.monsters.IndexOf(monster);
            displayMode = DisplayMode.Choice;
        }

        GUILayout.BeginHorizontal();
        InterfaceUtility.ProgressBar(150, 20, monster.life, monster.maxLife, InterfaceUtility.ColorToTexture(Color.green), InterfaceUtility.ColorToTexture(Color.gray));
        GUILayout.Label(monster.life + "/" + monster.maxLife, lifeStyle, GUILayout.Width(120));
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
    }

    #endregion

    #region Inventory

    #endregion

}
