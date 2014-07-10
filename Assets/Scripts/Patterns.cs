using UnityEngine;
using System.IO;
using System.Collections.Generic;

/**
 * This script regroup all Patterns classes usefull for defining
 */

public class MonsterPattern {
    public string name;
    public Monster.Type type;

    public int maxLife = 10;
    public int maxStamina = 10;
    public int stat_might = 0;
    public int stat_resistance = 0;
    public int stat_luck = 0;
    public int stat_speed = 0;
    public float capture_rate = 1;
    
    public string battleSprite;

    public override string ToString() {
        return name;
    }

    public class AttackLevelUp {
        public int lvl;
        public Attack attack;
    }

    public List<AttackLevelUp> attackLevelUp = new List<AttackLevelUp>();
}