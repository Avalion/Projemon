﻿using UnityEngine;
using System.Collections.Generic;

/**
 * This action will launch a battle
 */
public class ActionMonsterBattle : MapObjectAction {
    [System.Serializable]
    public class EncounterMonster {
        public int patternId;
        
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
            list.Add(Monster.Generate(GameData.GetPattern(pattern.patternId), pattern.lvlMin, pattern.lvlMax));

        Battler battler = new Battler() { name = "Wild Monster" };
        battler.monsters = list;
        Battle.Launch(this, battler);
    }

    public override string InLine() {
        // In line is an Editor feature. Database is available
        DBMonsterPattern pattern = DataBase.SelectById<DBMonsterPattern>(monsters[0].patternId);
        return "Battle wild monsters : " + (monsters.Count > 0 && pattern != null ? pattern.name : "") + (monsters.Count > 1 ? "(...)" : "") + ".";
    }
    public override string Serialize() {
        // TODO : Serialize list of EncounterMonster
        return GetType().ToString();
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 1)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        // TODO : Deserialize list of EncounterMonster
    }
}
