using Mono.Data.Sqlite;
using UnityEngine;

/*
 * this table manage attack patterns
 */
public class DBAttack : SQLTable {
    public string name = "";
    public Monster.Type type;

    public int power = 20;
    public int accuracy = 80; // 0 a 100

    public int staminaCost = 10;

    public Monster.State enemyStateChange = Monster.State.None;
    public int enemyStateChangeAccuracy = 0; // 0 a 100
    public Monster.State launcherStateChange = Monster.State.None;
    public int launcherStateChangeAccuracy = 0; // 0 a 100

    public int battleAnimationID = -1;

    public override void FromRow(SqliteDataReader reader) {
        int pos = 0;
        ID = reader.GetInt32(pos++);

        name = reader.GetString(pos++);
        type = (Monster.Type)reader.GetInt32(pos++);
        
        power = reader.GetInt32(pos++);
        accuracy = reader.GetInt32(pos++);

        staminaCost = reader.GetInt32(pos++);

        enemyStateChange = (Monster.State)reader.GetInt32(pos++);
        enemyStateChangeAccuracy = reader.GetInt32(pos++);
        launcherStateChange = (Monster.State)reader.GetInt32(pos++);
        launcherStateChangeAccuracy = reader.GetInt32(pos++);

        battleAnimationID = reader.GetInt32(pos++);
    }
    public override string Fields() {
        return "name, type, power, accuracy, staminaCost, enemyStateChange, enemyStateChangeAccuracy, launcherStateChange, launcherStateChangeAccuracy, battleAnimationId";
    }
    public override string TypedFields() {
        return "name text NOT NULL, " +
               "type integer, " +
               
               "power integer, " +
               "accuracy integer, " +
               
               "staminaCost integer, " +

               "enemyStateChange integer, " +
               "enemyStatechangeAccuracy integer, " +
               "launcherStateChange integer, " +
               "launcherStatechangeAccuracy integer, " +
               
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

            Stringize(staminaCost) + ", " +
            
            Stringize((int)enemyStateChange) + ", " +
            Stringize(enemyStateChangeAccuracy) + ", " +
            Stringize((int)launcherStateChange) + ", " +
            Stringize(launcherStateChangeAccuracy) + ", " +

            Stringize(battleAnimationID);
    }
    public override void Delete() {
        DataBase.DeleteByID<DBAttack>(ID);
    }

    public bool Launch(Monster caster, Monster target, Rect effectZone) {
        if (!caster.UseStamina(caster, staminaCost))
            return false;
        
        int damage = 0;
        string message = "";
        if (Random.Range(0, 100) <= accuracy) {
            damage = Mathf.RoundToInt(caster.stat_might + power);
            if (Random.Range(0, 100) <= caster.stat_luck / 10) {
                message = "Coup Critique ! ";
                damage *= 3;
            }
            new BattleAnimation(battleAnimationID).Display(effectZone);
            message += target.monsterName + " a subi " + damage + " dégats !";

            Battle.Current.Message = message;

            if (enemyStateChange != Monster.State.None && MathUtility.TestProbability100(enemyStateChangeAccuracy)) {
                target.state = enemyStateChange;
                Battle.Current.Message = target.monsterName + Monster.GetStateAltName(enemyStateChange);
            }
            if (launcherStateChange != Monster.State.None && MathUtility.TestProbability100(launcherStateChangeAccuracy)) {
                target.state = launcherStateChange;
                Battle.Current.Message = target.monsterName + Monster.GetStateAltName(launcherStateChange);
            }
        } else {
            Battle.Current.Message = caster.monsterName + " a raté son attaque...";
        }

        return true;
    }

    public override string ToString() {
        return name;
    }
}
