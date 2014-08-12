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
        return "Heal " + (targetMonster == -1 ? "all the group " : target.monsters[targetMonster].name) + " of " + healValue + " pv.";
    }
}