using UnityEngine;
using System.Collections.Generic;

/**
 * This class design the Hero
 */
public class Player : Battler {
    public const int MAX_TEAM_NUMBER = 6;

    private static Player current = null;
    public static Player Current {
        get {
            if (current == null) {
                current = new Player();
                
                //TEMPORARY
                CaptureItem scroll = new CaptureItem();
                scroll.name = "Capture Scroll";
                current.actions.Add(new ActionAddItem(scroll, Player.Current));

                current.monsters.Add(Monster.Generate(DataBase.SelectById<DBMonsterPattern>(0), 5, 5));

                MonsterCollection.capturedMonsters.Add(Monster.Generate(DataBase.SelectById<DBMonsterPattern>(3), 5, 5));
            }
            return current;
        }
    }

    private static int locked;
    public static bool Locked {
        get { return locked != 0; }
    }
    public static void Lock() {
        locked++;
    }
    public static void Unlock() {
        locked = Mathf.Max(0, locked - 1);
    }
    public static void ForceUnlock() {
        locked = 0;
    }
}