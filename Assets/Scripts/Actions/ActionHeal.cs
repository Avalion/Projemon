using UnityEngine;

/**
 * This action will heal a monster or a group of monsters
 */
public class ActionHeal : MapObjectAction {
    // TODO : get id from variable !
    // TODO : get healValue from variable !
    public int targetMonster = -1; // -1 to heal all the group
    public int healValue = 0;

    public ActionHeal() {
    }
    public ActionHeal(int _targetMonster, int _healValue){
        targetMonster = _targetMonster;
        healValue = _healValue;
    }

    public override void Execute() {
        if (targetMonster == -1) {
            foreach (Monster m in Player.Current.monsters)
                m.Damage(m, -healValue);
        } else {
            Player.Current.monsters[targetMonster].Damage(Player.Current.monsters[targetMonster], -healValue);
        }
        Terminate();
    }

    public override string InLine() {
        return "Heal " + (targetMonster == -1 ? "all the group " : targetMonster + "°") + " of " + healValue + " pv.";
    }

    public override string Serialize() {
        // TODO : Add MapObjectID when MapObject are into DB
        return GetType().ToString() + "|" + targetMonster + "|" + healValue;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 3)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        // TODO : Read and find MapObjectID when MapObject are into DB
        targetMonster = int.Parse(values[1]);
        healValue = int.Parse(values[2]);
    }
}