﻿using UnityEngine;
using System.Collections.Generic;

/**
 * This action will launch a battle
 */
[System.Serializable]
public class ActionMonsterBattle : MapObjectAction {
    // TODO : Change this into a list of MonsterPattern and Generate monsters from the Execute function
    public List<Monster> monsters = new List<Monster>();


    public ActionMonsterBattle() { }
    public ActionMonsterBattle(List<Monster> _monsters) {
        monsters = _monsters;
    }

    public override void Execute() {
        if (monsters.Count == 0) {
            Terminate();
            return;
        }

        Battler battler = new GameObject("Wild Monster").AddComponent<Battler>();
        battler.monsters = monsters;
        Battle.Launch(this, battler);
    }
}
