﻿using UnityEngine;
using System.Collections.Generic;

public class HealingItem : Item {
    public class TargetStat {
        public enum TargetMode { Life, Stamina, State, Exp };
        public TargetMode mode;

        public Monster.State state = Monster.State.None;
        
        public int value;
        public int randomValue;

        public float precision = 100;
    }

    public List<TargetStat> heals;
    
    
    protected override void Effect(Battler caster, List<Monster> target) {
        foreach (TargetStat heal in heals) {
            if (MathUtility.TestProbability100(heal.precision)) {
                switch (heal.mode) {
                    case TargetStat.TargetMode.Life:
                        foreach (Monster m in target)
                            m.Damage(m, -(heal.value + Random.Range(0, heal.randomValue)));
                        break;
                    case TargetStat.TargetMode.Stamina:
                        foreach (Monster m in target)
                            m.UseStamina(m, -(heal.value + Random.Range(0, heal.randomValue)), true);
                        break;
                    case TargetStat.TargetMode.State:
                        if (heal.state == Monster.State.None) {
                            Debug.LogWarning("Applying a state modification to State.None value !");
                            break;
                        }
                        foreach (Monster m in target) {
                            if (heal.value == 0) // apply state
                                m.state = heal.state;
                            else if (m.state == heal.state) // cure state
                                m.state = Monster.State.Healthy;
                        }
                        break;
                    case TargetStat.TargetMode.Exp:
                        foreach (Monster m in target)
                            m.Exp(heal.value + Random.Range(0, heal.randomValue));
                        break;
                }
            }
        }
    }
}
