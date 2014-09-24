using UnityEngine;

/**
 * This action will add a monster to the 
 */
[System.Serializable]
public class ActionAddMonster : MapObjectAction {
    MonsterPattern m;

    int lvl;

    public ActionAddMonster() {
        
    }
    public ActionAddMonster(MonsterPattern _m, int _lvl) {
        m = _m;
        lvl = _lvl;
    }

    public override void Execute() {
        MonsterCollection.AddToCollection(Monster.Generate(m, lvl, lvl));
        Terminate();
    }


    public override string InLine() {
        return "Add Monster " + m.name + " at level " + lvl+".";
    }
}