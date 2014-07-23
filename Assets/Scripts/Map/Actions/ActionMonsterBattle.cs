using UnityEngine;
using System.Collections.Generic;

/**
 * This action will launch a battle
 */
public class ActionMonsterBattle : MapObjectAction {
    public List<Monster> monsters;

    public ActionMonsterBattle(List<Monster> _monsters) {
        monsters = _monsters;
    }

    public override void Execute() {
        Battler battler = new GameObject("Wild Monster").AddComponent<Battler>();
        battler.monsters = monsters;
        Battle.Launch(this, battler);
    }
}
