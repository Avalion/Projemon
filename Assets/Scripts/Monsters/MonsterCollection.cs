using UnityEngine;
using System.Collections.Generic;

public class MonsterCollection {
    public static List<int> encounteredMonsters = new List<int>();
    public static List<Monster> capturedMonsters = new List<Monster>();

    // Register
    public static void Encounter(Monster m) {
        if (!encounteredMonsters.Contains(m.monsterPattern.ID))
            encounteredMonsters.Add(m.monsterPattern.ID);
    }
    public static void AddToCollection(Monster m) {
        Encounter(m);
        if (Player.Current.monsters.Count < Player.MAX_TEAM_NUMBER)
            Player.Current.monsters.Add(m);
        else
            capturedMonsters.Add(m);
    }
    public static void Release(Monster m) {
        capturedMonsters.Remove(m);
    }

    // Get informations
    public static bool isEncountered(Monster m) {
        return encounteredMonsters.Contains(m.monsterPattern.ID);
    }
    public static bool isAlreadyCaptured(Monster m) {
        foreach (Monster monster in Player.Current.monsters)
            if (m.monsterPattern.ID == monster.monsterPattern.ID)
                return true;
        foreach (Monster monster in capturedMonsters)
            if (m.monsterPattern.ID == monster.monsterPattern.ID)
                return true;
        return false;
    }
}
