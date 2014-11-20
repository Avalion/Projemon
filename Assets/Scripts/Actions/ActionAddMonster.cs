using UnityEngine;

/**
 * This action will add a monster to the 
 */
[System.Serializable]
public class ActionAddMonster : MapObjectAction {
    public int lvl;

    private Monster m;

    //public ActionAddMonster() {
    //    throw new System.Exception("ActionAddMonster have to be initialized");
    //}
    public ActionAddMonster(int _patternId, int _lvl) {
        lvl = _lvl;

        m = Monster.Generate(DataBase.SelectById<DBMonsterPattern>(_patternId), lvl, lvl);
        m.battler = Player.Current;
    }

    public override void Execute() {
        MonsterCollection.AddToCollection(m);
        
        if (Player.Current.monsters.Count < Player.MAX_TEAM_NUMBER) {
            Player.Current.monsters.Add(m);
        }
        
        Terminate();
    }


    public override string InLine() {
        return "Add Monster " + m.monsterPattern.name + " at level " + lvl + ".";
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