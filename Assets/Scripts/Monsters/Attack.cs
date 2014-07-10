using UnityEngine;

/**
 * This class 
 */
[System.Serializable]
public class Attack {
    public string name="null";

    public Monster.Type type;

    public int power;
    public int precision;

    public int battleAnimationID;

    public int Launch(Monster caster, Monster target) {
        int damage = 0;
        string message = "";
        if (Random.Range(0, 100) <= precision) {
            damage = Mathf.RoundToInt(caster.stat_might + power);
            if (Random.Range(0, 100) <= caster.stat_luck/10) {
                message = "Critical Hit ! ";
                damage *= 3;
            }
            //new BattleAnimation(battleAnimationID).Display();
            message += target.monsterName + " took " + damage + " damages !";
        } else {
            message = caster.monsterName + " has failed his attack.";
        }

        Battle.Message = message;
        return damage;
    }

    public override string ToString() {
        return name;
    }
}
