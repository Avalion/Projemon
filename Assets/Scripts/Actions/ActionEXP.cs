using UnityEngine;

/**
 * This action will add exp to a monster or a group of monsters
 */
public class ActionEXP : MapObjectAction {
    // TODO : get id from variable !
    // TODO : get exp from variable !
    public int targetMonster = -1; //-1 to add exp for all the group
    public int expValue = 0;

    public ActionEXP() { }

    public ActionEXP(int _targetMonster, int _expValue) {
        targetMonster = _targetMonster;
        expValue = _expValue;
    }

    public override void Execute() {
        if (targetMonster == -1) {
            foreach (Monster m in Player.Current.monsters)
                m.Exp(expValue);
        } else {
            Player.Current.monsters[targetMonster].Exp(expValue);
        }
        Terminate();
    }

    public override string InLine() {
        return "Add "+ expValue + " exp to " + (targetMonster == -1 ? "all the group " : Player.Current.monsters[targetMonster].monsterName) + ".";
    }

    public override string Serialize() {
        return GetType().ToString() + "|" + targetMonster + "|" + expValue;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 3)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        targetMonster = int.Parse(values[1]);
        expValue = int.Parse(values[2]);
    }
}