using System.Collections.Generic;

public class ActionBattle : MapObjectAction {
    public PNJBattler battler;

    public ActionBattle(PNJBattler battler) {
        this.battler = battler;
    }

    public override void Execute() {
        Battle.Launch(this, new List<Monster>(battler.monsters));
    }

    public override void Terminate() {
        base.Terminate();
        battler.nbWin++;
    }
}
