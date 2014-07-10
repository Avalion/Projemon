using UnityEngine;
using System.Collections.Generic;

/**
 * This class manage battles
 */
public class Battle : MonoBehaviour {
    public const float MESSAGE_SPEED = 1.5f; 
    public List<Monster> allies = new List<Monster>();
    public List<Monster> enemies = new List<Monster>();

    public int currentAlly = 0;
    public int currentEnemy = 0;

    private GUIStyle messageStyle = new GUIStyle();
    private GUIStyle lifeStyle = new GUIStyle();
    private GUIStyle monsterBoxStyle = new GUIStyle();
    private GUIStyle monsterBoxCurrentStyle = new GUIStyle();

    private static List<string> message = new List<string>();
    public static string Message { 
        get { return message.Count > 0 ? message[0] : ""; }
        set { if (value == "") message.RemoveAt(0); else message.Add(value); time = Time.time; }
    }
    private static float time;

    private enum DisplayMode { Choice, Attack, SwitchMonster };
    private DisplayMode displayMode;

    // Static constructors
    public static void Launch(List<Monster> enemies) {
        Battle b = new GameObject("Battle").AddComponent<Battle>();
        b.displayMode = DisplayMode.Choice;
        b.allies = Player.Current.monsters;
        b.enemies = enemies;

        b.messageStyle = new GUIStyle();
        b.messageStyle.normal.textColor = Color.white;
        b.messageStyle.fontSize = 25;
        b.lifeStyle = new GUIStyle(b.messageStyle);
        b.lifeStyle.alignment = TextAnchor.UpperRight;
        b.monsterBoxStyle = new GUIStyle();
        b.monsterBoxStyle.normal.background = InterfaceUtility.GetTexture(Config.GetResourcePath("System/Box") + "monsterBoxBackground.png");
        b.monsterBoxStyle.padding = new RectOffset(7, 7, 7, 7);
        b.monsterBoxCurrentStyle = new GUIStyle(b.monsterBoxStyle);
        b.monsterBoxCurrentStyle.normal.background = InterfaceUtility.GetTexture(Config.GetResourcePath("System/Box") + "monsterBoxCurrentBackground.png");

        World.Show = false;
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
        DisplayBattleMonster(allies[currentAlly], false, true);
        GUILayout.FlexibleSpace();
        DisplayBattleMonster(enemies[currentEnemy], true, false);
        GUILayout.Space(100);
        GUILayout.EndHorizontal();
        GUILayout.FlexibleSpace();
        GUILayout.EndVertical();
    }

    public void DisplayBottomPanel() {
        int menuHeight = 150;
        
        GUILayout.BeginHorizontal();
        int choice = InterfaceUtility.DisplayMenu(new List<GUIContent>() { new GUIContent("Attack"), new GUIContent("Switch Monster"), new GUIContent("Capture Test")}, GUILayout.Width(200), GUILayout.Height(menuHeight));
        if (choice == 0) {
            displayMode = DisplayMode.Attack;
        }
        if (choice == 1) {
            displayMode = DisplayMode.SwitchMonster;

        }

        if (choice == 2) {
            new CaptureScroll().Effect(new List<Monster>() {allies[currentAlly]},new List<Monster>() {enemies[currentEnemy]});
        }

        InterfaceUtility.BeginBox(GUILayout.Height(menuHeight));
        GUILayout.BeginHorizontal();
        GUILayout.Label(Monster.GetTypeIcon(allies[currentAlly].type), InterfaceUtility.EmptyStyle);
        GUILayout.Label(allies[currentAlly].monsterName, messageStyle);
        GUILayout.FlexibleSpace();

        InterfaceUtility.ProgressBar(200, 20, allies[currentAlly].life, allies[currentAlly].maxLife, InterfaceUtility.ColorToTexture(Color.green), InterfaceUtility.ColorToTexture(Color.gray));
        GUILayout.Label(allies[currentAlly].life + "/" + allies[currentAlly].maxLife, lifeStyle, GUILayout.Width(120));
        GUILayout.EndHorizontal();
        InterfaceUtility.EndBox();

        GUILayout.EndHorizontal();
    }

    public void DisplayAttackPanel() {
        int menuHeight = 150;
        Attack[] attacks = allies[currentAlly].attacks;
        List<GUIContent> list = new List<GUIContent>();

        foreach (Attack a in attacks) {
            if (a.name != "")
                list.Add(new GUIContent(a.name));
        }
        list.Add(new GUIContent("Cancel"));

        GUILayout.BeginHorizontal();
        int choice = InterfaceUtility.DisplayMenu(list , GUILayout.Width(200), GUILayout.Height(menuHeight));
        if (choice >= 0 && choice < list.Count - 1) 
            allies[currentAlly].Damage(enemies[currentEnemy], allies[currentAlly].attacks[choice].Launch(allies[currentAlly], enemies[currentEnemy], new Rect(Screen.width - 300, (Screen.height / 2f) - 150, 300, 200)));
        if (choice == list.Count - 1)
            displayMode = DisplayMode.Choice;

        InterfaceUtility.BeginBox(GUILayout.Height(menuHeight));
        GUILayout.BeginHorizontal();
        GUILayout.Label(Monster.GetTypeIcon(allies[currentAlly].type), InterfaceUtility.EmptyStyle);
        GUILayout.Label(allies[currentAlly].monsterName, messageStyle);
        GUILayout.FlexibleSpace();

        InterfaceUtility.ProgressBar(200, 20, allies[currentAlly].life, allies[currentAlly].maxLife, InterfaceUtility.ColorToTexture(Color.green), InterfaceUtility.ColorToTexture(Color.gray));
        GUILayout.Label(allies[currentAlly].life + "/" + allies[currentAlly].maxLife, lifeStyle, GUILayout.Width(120));
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
        DisplayMonsterBox(0.48f, 0.12f, enemies[currentEnemy], true);
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        
        for (int i = 0; i < allies.Count; i++)
        {
            if(MathUtility.IsPair(i)) {
                GUILayout.BeginHorizontal();
            } else {
                GUILayout.BeginVertical();
                GUILayout.Space(20);
            }
            DisplayMonsterBox(0.48f, 0.12f, allies[i], currentAlly==i);
            if(!MathUtility.IsPair(i)){
                GUILayout.Space(-20);
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();    
            }
        }
        if (!MathUtility.IsPair(allies.Count))
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
            currentAlly = allies.IndexOf(monster);
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
