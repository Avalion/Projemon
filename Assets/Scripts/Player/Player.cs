﻿using UnityEngine;
using System.Collections.Generic;

/**
 * This class design the Hero
 */
public class Player : Battler {
    public const int MAX_TEAM_NUMBER = 6;

    // Singleton
    private static Player current = null;
    public static Player Current {
        get {
            if (current == null) {
                current = new Player();

                current.mapObjectId = -1;

                current.name = "Player";

                //TEMPORARY
                CaptureItem scroll = new CaptureItem();
                scroll.name = "Capture Scroll";
                current.actions.Add(new ActionAddItem(scroll));
                //*/
            }
            return current;
        }
    }

    private static int locked = 0;
    public static bool Locked {
        get { return locked != 0; }
    }
    public static void Lock() {
        locked++;
    }
    public static void Unlock() {
        if (locked > 0) locked -= 1;
    }
    public static void ForceUnlock() {
        locked = 0;
    }
}