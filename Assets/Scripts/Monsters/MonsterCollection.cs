using UnityEngine;
using System.Collections.Generic;

public class MonsterCollection {
    public static List<int> encounteredMonsters = new List<int>();
    public static List<Monster> capturedMonsters = new List<Monster>();


    public static void Encounter(Monster m) {
        if (!encounteredMonsters.Contains(m.monsterPattern.ID))
            encounteredMonsters.Add(m.monsterPattern.ID);
    }
    public static void AddToCollection(Monster m) {
        Encounter(m);
        capturedMonsters.Add(m);
    }
    public static void Release(Monster m) {
        capturedMonsters.Remove(m);
    }
}
