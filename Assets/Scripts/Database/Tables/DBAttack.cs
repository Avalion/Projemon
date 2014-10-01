using Mono.Data.Sqlite;
using UnityEngine;

/*
 * this table manage attack patterns
 */
public class DBAttack : SQLTable {
    public string name = "null";
    public Monster.Type type;

    public int power;
    public int accuracy; // 0 a 100

    public Monster.State stateChange = Monster.State.Healthy;
    public int stateChangeAccuracy = 0; // 0 a 100

    public int battleAnimationID;

    public override void FromRow(SqliteDataReader reader) {
        int pos = 0;
        ID = reader.GetInt32(pos++);

        name = reader.GetString(pos++);
        type = (Monster.Type)reader.GetInt32(pos++);
        
        power = reader.GetInt32(pos++);
        accuracy = reader.GetInt32(pos++);

        stateChange = (Monster.State)reader.GetInt32(pos++);
        stateChangeAccuracy = reader.GetInt32(pos++);

        battleAnimationID = reader.GetInt32(pos++);
    }
    public override string Fields() {
        return "name, type, power, accuracy, stateChange, stateChangeAccuracy, battleAnimationId";
    }
    public override string TypedFields() {
        return "name text NOT NULL, " +
               "type integer, " +
               "power integer, " +
               "accuracy integer, " +
               "stateChange integer, " +
               "statechangeAccuracy integer, " +
               "battleAnimationId integer DEFAULT -1";
    }
    public override string TableName() {
        return "T_Attack";
    }
    public override string ToRow() {
        return
            Stringize(name) + ", " + 
            Stringize((int)type) + ", " + 
            Stringize(power) + ", " +
            Stringize(accuracy) + ", " +
            Stringize(stateChange) + ", " +
            Stringize(stateChangeAccuracy) + ", " +
            Stringize(battleAnimationID);
    }


    public int Launch(Monster caster, Monster target, Rect effectZone) {
        int damage = 0;
        string message = "";
        if (Random.Range(0, 100) <= accuracy) {
            damage = Mathf.RoundToInt(caster.stat_might + power);
            if (Random.Range(0, 100) <= caster.stat_luck/10) {
                message = "Coup Critique ! ";
                damage *= 3;
            }
            new BattleAnimation(battleAnimationID).Display(effectZone);
            message += target.monsterName + " a subit " + damage + " dégats !";

            Battle.Current.Message = message;

            if (Random.Range(0, 100) <= stateChangeAccuracy) {
                target.state = stateChange;
                Battle.Current.Message = target.monsterName + Monster.GetStateAltName(stateChange);
            }
        } else {
            Battle.Current.Message = caster.monsterName + " a raté son attaque...";
        }

        return damage;
    }

    public override string ToString() {
        return name;
    }
}
