using UnityEngine;

/**
 * This action will heal a monster or a group of monsters
 */
[System.Serializable]
public class ActionHeal : MapObjectAction {
    public int targetMonster;//-1 to heal all the group
    public Battler target;
    public int healValue;

    public ActionHeal() {
        target = Player.Current;
    }

    public ActionHeal(Battler _target, int _targetMonster, int _healValue){
        target = _target;
        targetMonster = _targetMonster;
        healValue = _healValue;
    }

    public override void Execute() {
        if (targetMonster == -1) {
            foreach (Monster m in target.monsters)
                m.Damage(m, -healValue);
        } else {
            target.monsters[targetMonster].Damage(target.monsters[targetMonster], -healValue);
        }
        Terminate();
    }

    public override string InLine() {
        return "Heal " + (targetMonster == -1 ? "all the group " : target.monsters[targetMonster].monsterName) + " of " + healValue + " pv.";
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