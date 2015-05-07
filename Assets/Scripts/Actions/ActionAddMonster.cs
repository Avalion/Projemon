using UnityEngine;

/**
 * This action will add a monster to the collection
 */
public class ActionAddMonster : MapObjectAction {
    public int lvl = 0;

    private int patternID = 0;

    public ActionAddMonster() { }
    public ActionAddMonster(int _patternId, int _lvl) {
        lvl = _lvl;

        patternID = _patternId;
    }

    public override void Execute() {
        DBMonsterPattern pattern = DataBase.SelectById<DBMonsterPattern>(patternID);
        if (pattern == null) {
            Terminate();
            throw new System.Exception("Unexpected Behaviour : Trying to add a inexisting Monster type to the team...");
        }

        Monster m = Monster.Generate(pattern, lvl, lvl);
        m.battler = Player.Current;

        MonsterCollection.AddToCollection(m);

        if (Player.Current.monsters.Count < Player.MAX_TEAM_NUMBER)
            Player.Current.monsters.Add(m);
        
        Terminate();
    }


    public override string InLine() {
        DBMonsterPattern pattern = DataBase.SelectById<DBMonsterPattern>(patternID);
        return "Add Monster " + (pattern != null ? pattern.name : "[TO DEFINE]") + " at level " + lvl + ".";
    }

    public override string Serialize() {
        // TODO : Serialize Monster class
        return GetType().ToString() + "|" + lvl;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 2)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        // TODO : Serialize Monster class
        lvl = int.Parse(values[1]);
    }
}