﻿using UnityEngine;

/**
 * This action will add exp to a monster or a group of monsters
 */
public class ActionEXP : MapObjectAction {
    public int targetMonster; //-1 to add exp for all the group
    public Battler target;
    public int expValue;

    public ActionEXP() {
        target = Player.Current;
    }

    public ActionEXP(Battler _target, int _targetMonster, int _expValue) {
        target = _target;
        targetMonster = _targetMonster;
        expValue = _expValue;
    }

    public override void Execute() {
        if (target == null) {
            Debug.LogError("Unexpected Behaviour : Try to add EXP to a not found MapObject");
        }
        if (targetMonster == -1) {
            foreach (Monster m in target.monsters)
                m.Exp(expValue);
        } else {
            target.monsters[targetMonster].Exp(expValue);
        }
        Terminate();
    }

    public override string InLine() {
        return "Add "+ expValue + " exp to " + (targetMonster == -1 ? "all the group " : target.monsters[targetMonster].monsterName) + ".";
    }

    public override string Serialize() {
        return GetType().ToString() + "|" + target.mapObjectId + "|" + targetMonster + "|" + expValue;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 3)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        // TODO : Read and find MapObjectID when MapObject are into DB
        target = World.Current.GetMapObjectById(int.Parse(values[1])) as Battler;
        targetMonster = int.Parse(values[2]);
        expValue = int.Parse(values[3]);
    }
}