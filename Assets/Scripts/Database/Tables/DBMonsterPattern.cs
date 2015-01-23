using UnityEngine;
using Mono.Data.Sqlite;
using System.Collections.Generic;

public class DBMonsterPattern : SQLTable {
    public string name;
    public Monster.Type type;

    public int start_life = 10;
    public Vector2   lifeUp = new Vector2(5, 15);
    public int start_stamina = 10;
    public Vector2   staminaUp = new Vector2(5, 15);
    public int start_might = 0;
    public Vector2   mightUp = new Vector2(1, 3);
    public int start_resistance = 0;
    public Vector2   resistanceUp = new Vector2(1, 3);
    public int start_luck = 0;
    public Vector2   luckUp = new Vector2(1, 3);
    public int start_speed = 0;
    public Vector2   speedUp = new Vector2(1, 3);

    public float capture_rate = 1;

    public int expMultiplier1;
    public int expMultiplier2;
    public int expMultiplier3;

    public string battleSprite;
    public string miniSprite;

    public bool encountered;

    public class AttackLevelUp {
        public int lvl;
        public DBAttack attack;
    }
    public List<AttackLevelUp> attackLevelUp = new List<AttackLevelUp>();


    public override void FromRow(SqliteDataReader reader) {
        int pos = 0;
        ID = reader.GetInt32(pos++);

        name = reader.GetString(pos++); 
        type = (Monster.Type)reader.GetInt32(pos++); 

        start_life = reader.GetInt32(pos++); 
        lifeUp = new Vector2(reader.GetInt32(pos++), reader.GetInt32(pos++)); 
        start_stamina = reader.GetInt32(pos++); 
        staminaUp = new Vector2(reader.GetInt32(pos++), reader.GetInt32(pos++)); 
        start_might = reader.GetInt32(pos++); 
        mightUp = new Vector2(reader.GetInt32(pos++), reader.GetInt32(pos++)); 
        start_resistance = reader.GetInt32(pos++); 
        resistanceUp = new Vector2(reader.GetInt32(pos++), reader.GetInt32(pos++)); 
        start_luck = reader.GetInt32(pos++); 
        luckUp = new Vector2(reader.GetInt32(pos++), reader.GetInt32(pos++)); 
        start_speed = reader.GetInt32(pos++); 
        speedUp = new Vector2(reader.GetInt32(pos++), reader.GetInt32(pos++));

        capture_rate = reader.GetInt32(pos++);

        expMultiplier1 = reader.GetInt32(pos++);
        expMultiplier2 = reader.GetInt32(pos++);
        expMultiplier3 = reader.GetInt32(pos++);

        battleSprite = reader.GetString(pos++); 
        miniSprite = reader.GetString(pos++);

        string[] attacks = reader.GetString(pos++).Split('#');
        attackLevelUp = new List<AttackLevelUp>();
        foreach (string attack in attacks) {
            if (attack == "" || attack == null)
                continue;
            string[] values = attack.Split(';');
            attackLevelUp.Add(new AttackLevelUp() { lvl = int.Parse(values[0]), attack = DataBase.SelectById<DBAttack>(int.Parse(values[1])) });
        }

        encountered = reader.GetBoolean(pos++);
    }
    public override string Fields() {
        return "name, type, " + 
            "start_life, lifeUpX, lifeUpY, " + 
            "start_stamina, staminaUpX, staminaUpY, " + 
            "start_might, mightUpX, mightUpY, " +
            "start_resistance, resistanceUpX, resistanceUpY, " +
            "start_luck, luckUpX, luckUpY, " +
            "start_speed, speedUpX, speedUpY, " + 
            "capture_rate, " + 
            "expM1, expM2, expM3, " +

            "battleSprite, miniSprite, " + 
            "attackLeveled, " + 
            "encountered";
    }
    public override string TypedFields() {
        return "name text NOT NULL, type integer, " +
            "start_life integer, lifeUpX integer, lifeUpY integer, " +
            "start_stamina integer, staminaUpX integer, staminaUpY integer, " + 
            "start_might integer, mightUpX integer, mightUpY integer, " +
            "start_resistance integer, resistanceUpX integer, resistanceUpY integer, " +
            "start_luck integer, luckUpX integer, luckUpY integer, " +
            "start_speed integer, speedUpX integer, speedUpY integer, " + 
            "capture_rate integer, " + 
            "expM1 integer, expM2 integer, expM3 integer, " +   
            
            "battleSprite text, miniSprite text, " + 
            "attackLeveled text, " + 
            "encountered bool";
    }
    public override string TableName() {
        return "T_MonsterPattern";
    }
    public override string ToRow() {
        string attacks = "";
        foreach (AttackLevelUp attack in attackLevelUp) {
            attacks += attack.lvl + ";" + attack.attack.ID + "#";
        }

        return
            Stringize(name) + ", " +
            Stringize((int)type) + ", " +

            Stringize(start_life) + ", " +
            Stringize(lifeUp.x) + ", " + Stringize(lifeUp.y) + ", " +
            Stringize(start_stamina) + ", " +
            Stringize(staminaUp.x) + ", " + Stringize(staminaUp.y) + ", " +
            Stringize(start_might) + ", " +
            Stringize(mightUp.x) + ", " + Stringize(mightUp.y) + ", " +
            Stringize(start_resistance) + ", " +
            Stringize(resistanceUp.x) + ", " + Stringize(resistanceUp.y) + ", " +
            Stringize(start_luck) + ", " +
            Stringize(luckUp.x) + ", " + Stringize(luckUp.y) + ", " +
            Stringize(start_speed) + ", " +
            Stringize(speedUp.x) + ", " + Stringize(speedUp.y) + ", " +

            Stringize(capture_rate) + ", " + 

            Stringize(expMultiplier1) + ", " +
            Stringize(expMultiplier2) + ", " +
            Stringize(expMultiplier3) + ", " +

            Stringize(battleSprite) + ", " + 
            Stringize(miniSprite) + ", " + 
            
            Stringize(attacks) + ", " + 

            Stringize(encountered);
    }
    public override void Delete() {
         DataBase.DeleteByID<DBMonsterPattern>(ID);
    }

    public override string ToString() {
        return name;
    }
}