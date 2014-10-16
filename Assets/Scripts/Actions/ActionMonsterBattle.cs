using UnityEngine;
using System.Collections.Generic;

/**
 * This action will launch a battle
 */
[System.Serializable]
public class ActionMonsterBattle : MapObjectAction {
    [System.Serializable]
    public class EncounterMonster {
        public DBMonsterPattern pattern;
        
        public int lvlMin;
        public int lvlMax;
    }
    
    public List<EncounterMonster> monsters = new List<EncounterMonster>();


    public ActionMonsterBattle() { }
    public ActionMonsterBattle(List<EncounterMonster> _monsters) {
        monsters = _monsters;
    }

    public override void Execute() {
        if (monsters.Count == 0) {
            Terminate();
            return;
        }

        List<Monster> list = new List<Monster>();
        foreach (EncounterMonster pattern in monsters)
            list.Add(Monster.Generate(pattern.pattern, pattern.lvlMin, pattern.lvlMax));

        Battler battler = new GameObject("Wild Monster").AddComponent<Battler>();
        battler.monsters = list;
        Battle.Launch(this, battler);
    }

    public override string InLine() {
        return "Battle wild monsters : " + (monsters.Count > 0 ? monsters[0].pattern.name : "") + (monsters.Count > 1 ? "(...)" : "") + ".";
    }
}
