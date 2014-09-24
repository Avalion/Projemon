using Mono.Data.Sqlite;

/*
 * this table manage captured monsters
 */
public class DBMonster : SQLTable {
    public string name;
    public int patternId;

    public int type;
    public int state;
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

        type = reader.GetInt32(pos++);
        state = reader.GetInt32(pos++);
        nickName = reader.GetString(pos++);

        lvl = reader.GetInt32(pos++);
        exp = reader.GetInt32(pos++);
        maxLife= reader.GetInt32(pos++);
        life = reader.GetInt32(pos++);
        maxStamina = reader.GetInt32(pos++);
        stamina = reader.GetInt32(pos++);
    }
    public override string Fields() {
        return "name, patternId, type, state, nickName";
    }
    public override string TypedFields() {
        return "name text, patternId integer, type integer, state integer, nickName text NOT NULL";
    }
    public override string TableName() {
        return "T_Monster";
    }
    public override string ToRow() {
        return
            Stringize(name) + ", " + 
            Stringize(patternId) + ", " + 
            Stringize(type) + ", " + 
            Stringize(state) + ", " + 
            Stringize(nickName);
    }
}
