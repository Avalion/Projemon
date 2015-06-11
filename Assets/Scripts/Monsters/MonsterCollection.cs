using UnityEngine;
using System.Collections.Generic;

public class MonsterCollection {
    public static List<int> encounteredMonsters = new List<int>();
    public static List<Monster> capturedMonsters = new List<Monster>();

    // Register
    public static void Encounter(DBMonsterPattern m) {
        if (!encounteredMonsters.Contains(m.ID))
            encounteredMonsters.Add(m.ID);

        m.encountered = true;
        DataBase.Update<DBMonsterPattern>("encoutered", true, "id=" + m.ID);
    }
    public static void AddToCollection(Monster m) {
        Encounter(m.monsterPattern);
        capturedMonsters.Add(m);
    }
    public static void Release(Monster m) {
        capturedMonsters.Remove(m);
    }

    // Get informations
    public static bool isEncountered(Monster m) {
        return isEncountered(m.monsterPattern);
    }
    public static bool isEncountered(DBMonsterPattern m) {
        return encounteredMonsters.Contains(m.ID);
    }
    public static bool isAlreadyCaptured(Monster m) {
        return isAlreadyCaptured(m.monsterPattern);
    }
    public static bool isAlreadyCaptured(DBMonsterPattern m) {
        foreach (Monster monster in Player.Current.monsters)
            if (m.ID == monster.monsterPattern.ID)
                return true;
        foreach (Monster monster in capturedMonsters)
            if (m.ID == monster.monsterPattern.ID)
                return true;
        return false;
    }

    // Get Lists from DB
    public static void Load() {
        encounteredMonsters = new List<int>();
        capturedMonsters = new List<Monster>();
        Player.Current.monsters = new List<Monster>();
        
        List<DBMonsterPattern> patterns = DataBase.Select<DBMonsterPattern>("encountered=1");
        foreach (DBMonsterPattern m in patterns) {
            Encounter(m);
        }

        List<DBMonster> collection = DataBase.Select<DBMonster>();
        foreach (DBMonster m in collection) {
            Monster monster = Monster.Generate(m);
            AddToCollection(monster);

            if (m.inTeam) {
                if (Player.Current.monsters.Count < Player.MAX_TEAM_NUMBER) {
                    Player.Current.monsters.Add(monster);
                }
            }
        }
    }
}
