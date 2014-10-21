using Mono.Data.Sqlite;

/*
 * this table manage captured monsters
 */
public class DBMonster : SQLTable {
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

    public int attack1 = -1;
    public int attack2 = -1;
    public int attack3 = -1;
    public int attack4 = -1;

    public bool inTeam;

    public override void FromRow(SqliteDataReader reader) {
        int pos = 0;
        ID = reader.GetInt32(pos++);

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

        attack1 = reader.GetInt32(pos++);
        attack2 = reader.GetInt32(pos++);
        attack3 = reader.GetInt32(pos++);
        attack4 = reader.GetInt32(pos++);

        inTeam = reader.GetBoolean(pos++);
    }
    public override string Fields() {
        return "patternId, type, state, nickName, level, exp, maxLife, life, maxStamina, stamina, stat_might, stat_resistance, stat_luck, stat_speed, attackId1, attackId2, attackId3, attackId4, inTeam";
    }
    public override string TypedFields() {
        return "patternId integer, " + 
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
               "attackId1 integer DEFAULT -1, " +
               "attackId2 integer DEFAULT -1, " +
               "attackId3 integer DEFAULT -1, " +
               "attackId4 integer DEFAULT -1, " + 
               "inTeam boolean";
    }
    public override string TableName() {
        return "T_Monster";
    }
    public override string ToRow() {
        return
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
            Stringize(attack1) + ", " +
            Stringize(attack2) + ", " +
            Stringize(attack3) + ", " +
            Stringize(attack4) + ", " + 
            Stringize(inTeam);
    }

    public static DBMonster ConvertFrom(Monster _source) {
        DBMonster m = new DBMonster();
        m.patternId = _source.monsterPattern.ID;

        m.type = _source.type;
        m.state = _source.state;
        m.nickName = _source.monsterName;

        m.lvl = _source.lvl;
        m.exp = _source.exp;

        m.maxLife = _source.maxLife;
        m.life = _source.life;
        m.maxStamina = _source.maxStamina;
        m.stamina = _source.stamina;

        // Stats
        m.stat_might = _source.stat_might;
        m.stat_resistance = _source.stat_resistance;
        m.stat_luck = _source.stat_luck;
        m.stat_speed = _source.stat_speed;

        try {
            m.attack1 = _source.attacks[0].ID;
        } catch { m.attack1 = -1; }
        try {
            m.attack2 = _source.attacks[1].ID;
        } catch { m.attack2 = -1; }
        try {
            m.attack3 = _source.attacks[2].ID;
        } catch { m.attack3 = -1; }
        try {
            m.attack4 = _source.attacks[3].ID;
        } catch { m.attack4 = -1; }

        m.inTeam = Player.Current.monsters.Contains(_source);

        return m;
    }

}
