using UnityEngine;
using System.Collections.Generic;

/**
 * This action will launch a battle
 */
[System.Serializable]
public class ActionPNJBattle : MapObjectAction {
    public PNJBattler battler;

    public ActionPNJBattle() {}
    public ActionPNJBattle(PNJBattler battler) {
        this.battler = battler;
    }

    public override void Execute() {
        Battle.Launch(this, battler);
    }

    public override void Terminate() {
        base.Terminate();
        battler.nbWin++;
    }
}
