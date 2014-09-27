using Mono.Data.Sqlite;

/*
 * this table manage captured monsters
 */
public class DBMonster : SQLTable {
    public string name;
    public int patternId;

    public Monster.Type type;
    public Monster.State state;
    public string nickName;

    // Lvl
    public int lvl = 1;
    public int exp = 0;

    // Life
    public int maxLife;
    public int life;
    
    // Stamina
    public int maxStamina;
    public int stamina;

    // Stats
    public int stat_might;
    public int stat_resistance;
    public int stat_luck;
    public int stat_speed;

    // Capture
    public float capture_rate;

    // LevelUp
    public int expMultiplier1;
    public int expMultiplier2;
    public int expMultiplier3;
    
    public int attack1;
    public int attack2;
    public int attack3;
    public int attack4;

    public override void FromRow(SqliteDataReader reader) {
        int pos = 0;
        ID = reader.GetInt32(pos++);

        name = reader.GetString(pos++);
        patternId = reader.GetInt32(pos++);

        type = (Monster.Type)reader.GetInt32(pos++);
        state = (Monster.State)reader.GetInt32(pos++);
        nickName = reader.GetString(pos++);

        lvl = reader.GetInt32(pos++);
        exp = reader.GetInt32(pos++);
        maxLife= reader.GetInt32(pos++);
        life = reader.GetInt32(pos++);
        maxStamina = reader.GetInt32(pos++);
        stamina = reader.GetInt32(pos++);
        stat_might = reader.GetInt32(pos++);
        stat_resistance = reader.GetInt32(pos++);
        stat_luck = reader.GetInt32(pos++);
        stat_speed = reader.GetInt32(pos++);

        capture_rate = reader.GetInt32(pos++);

        expMultiplier1 = reader.GetInt32(pos++);
        expMultiplier2 = reader.GetInt32(pos++);
        expMultiplier3 = reader.GetInt32(pos++);

        attack1 = reader.GetInt32(pos++);
        attack2 = reader.GetInt32(pos++);
        attack3 = reader.GetInt32(pos++);
        attack4 = reader.GetInt32(pos++);
    }
    public override string Fields() {
        return "name, patternId, type, state, nickName, level, exp, maxLife, life, maxStamina, stamina, stat_might, stat_resistance, stat_luck, stat_speed, capture rate, expM1, expM2, expM3, attackId1, attackId2, attackId3, attackId4";
    }
    public override string TypedFields() {
        return "name text NOT NULL, " +
               "patternId integer, " + 
               "type integer, " +
               "state integer DEFAULT 0, " +
               "nickName text, " +
               "level integer, " +
               "exp integer, " +
               "maxLife integer, " +
               "life integer, " +
               "maxStamina integer, " +
               "stamina integer, " +
               "stat_might integer, " +
               "stat_resistance integer, " +
               "stat_luck integer, " +
               "stat_speed integer, " +
               "capture rate integer, " +
               "expM1 integer, " +
               "expM2 integer, " +
               "expM3 integer, " +
               "attackId1 integer DEFAULT -1, " +
               "attackId2 integer DEFAULT -1, " +
               "attackId3 integer DEFAULT -1, " +
               "attackId4 integer DEFAULT -1";
    }
    public override string TableName() {
        return "T_Monster";
    }
    public override string ToRow() {
        return
            Stringize(name) + ", " + 
            Stringize(patternId) + ", " + 
            Stringize((int)type) + ", " + 
            Stringize((int)state) + ", " +
            Stringize(nickName) + ", " +
            Stringize(lvl) + ", " +
            Stringize(exp) + ", " +
            Stringize(maxLife) + ", " +
            Stringize(life) + ", " +
            Stringize(maxStamina) + ", " +
            Stringize(stamina) + ", " +
            Stringize(stat_might) + ", " +
            Stringize(stat_resistance) + ", " +
            Stringize(stat_luck) + ", " +
            Stringize(stat_speed) + ", " +
            Stringize(capture_rate) + ", " +
            Stringize(expMultiplier1) + ", " +
            Stringize(expMultiplier2) + ", " +
            Stringize(expMultiplier3) + ", " +
            Stringize(attack1) + ", " +
            Stringize(attack2) + ", " +
            Stringize(attack3) + ", " +
            Stringize(attack4);
    }
}
