using System.Collections.Generic;

public class ActionBattle : MapObjectAction {
    public PNJBattler battler;

    public ActionBattle(PNJBattler battler) {
        this.battler = battler;
    }

    public override void Execute() {
        // TODO : Battle.Launch(battler);
        Battle.Launch(new List<Monster>(battler.monsters));
    }
}
