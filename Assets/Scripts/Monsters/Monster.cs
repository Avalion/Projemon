﻿using UnityEngine;

/**
 * This class design a Monster type. 
 */
public class Monster {
    public const string IMAGE_FOLDER = "Battlers";
    public const bool REFILL_LIFE_LVL_UP = true;
    public const bool REFILL_STAMINA_LVL_UP = true;


    public MonsterPattern monsterPattern;

    // Type
    public enum Type {
        Water,
        Fire,
        Earth,
        Air,
        Elec,
        Plant,
        Shadow,
        Life,
        Death,
        Stone,
        Ice,
        Metal,
        COUNT
    }

    public static float[][] resistances = new float[0][];
    public static float GetTypeResistance(Type _attack, Type _against) {
        if (resistances.Length == 0) {
            resistances = new float[(int)Type.COUNT][];
            //                              Water, Fire , Earth, Air  , Elec , Plant, Shade, Life , Death, Stone, Ice  , Metal
            resistances[0] = new float[] {  0.5f , 2    , 0.25f, 1    , 0.25f, 0.25f, 1    , 1    , 1.5f , 2    , 0.5f , 1    }; // Water
            resistances[1] = new float[] {  0.25f, 0.5f , 0.5f , 1    , 1    , 2    , 1    , 1.5f , 1    , 1    , 2    , 2    }; // Fire
            resistances[2] = new float[] {  2    , 1.5f , 0.5f , 0.5f , 2    , 0.25f, 1    , 1.5f , 1    , 1    , 2    , 2    }; // Earth
            resistances[3] = new float[] {  1    , 1    , 0.5f , 1    , 1    , 1.5f , 1    , 1    , 1.5f , 0.25f, 0.5f , 0.5f }; // Air
            resistances[4] = new float[] {  2    , 1    , 0.25f, 1    , 0.25f, 2    , 1    , 1    , 1    , 0.25f, 1    , 0.25f}; // Elec
            resistances[5] = new float[] {  2    , 0.25f, 2    , 0.5f , 0.25f, 1    , 1    , 1    , 0.25f, 1    , 0.5f , 0.25f}; // Plant
            resistances[6] = new float[] {  1    , 1    , 1    , 1    , 1    , 1    , 0.5f , 1.5f , 1.5f , 1    , 1    , 0.5f }; // Shadow
            resistances[7] = new float[] {  1    , 0.5f , 0.5f , 1    , 1    , 1    , 0.25f, 1    , 2    , 1    , 1    , 1    }; // Life
            resistances[8] = new float[] {  0.5f , 1    , 1    , 0.5f , 1    , 2    , 0.25f, 2    , 0.25f, 1    , 1    , 1    }; // Death
            resistances[9] = new float[] {  0.25f, 1    , 1.5f , 2    , 2    , 1    , 1    , 1    , 1    , 0.5f , 2    , 2    }; // Stone
            resistances[10] = new float[] { 1.5f , 0.25f, 1    , 1.5f , 1    , 1.5f , 1    , 1    , 1    , 0.25f, 0.5f , 1    }; // Ice
            resistances[11] = new float[] { 1    , 0.5f , 1    , 0.5f , 1.5f , 1.5f , 0.5f , 1    , 1    , 0.25f, 1    , 1.5f }; // Metal
        }

        return resistances[(int)_attack][(int)_against];
    }

    public static Texture2D GetTypeIcon(Type _type) {
        return InterfaceUtility.GetTexture(Config.GetResourcePath("System/Icons") + _type.ToString() + ".png");
    }

    public Type type;

    public enum State {
        Healthy,
        Dead,
        Poisoned,
        Insane,
        Paralized,
        Sleepy,
        COUNT
    }
    public State state;

    public string monsterName;

    public Battler battler = null;

    // Lvl
    public int lvl = 1;
    public int exp = 0;

    // Life
    public int maxLife = 100;
    public int life;
    // Stamina
    public int maxStamina = 50;
    public int stamina;

    // Stats
    public int stat_might = 0;
    public int stat_resistance = 0;
    public int stat_luck = 0;
    public int stat_speed = 0;

    // Capture
    public float capture_rate = 1;

    // LevelUp
    public int expMultiplier1 = 30;
    public int expMultiplier2 = 10;
    public int expMultiplier3 = 5;
    public int[] expRequired = new int[99];
    
    // Design
    public Texture2D battleSprite;
    public Texture2D miniSprite;

    public Attack[] attacks = new Attack[4] { new Attack() { name = "null" }, new Attack() { name = "null" }, new Attack() { name = "null" }, new Attack() { name = "null" } };


    /* Experience
     */
    public void Exp(int gain) {
        exp += gain;
        if (exp < 0)
            exp = 0;
        if (exp >= expRequired[lvl - 1]) {
            exp -= expRequired[lvl - 1];
            LevelUp();
        }
    }
    public void LevelUp() {
        lvl++;
        int tempLife = Random.Range((int)monsterPattern.lifeUp.x, (int)monsterPattern.lifeUp.y + 1);
        maxLife += tempLife;
        life = REFILL_LIFE_LVL_UP ? maxLife : life + tempLife;
        int tempStamina = Random.Range((int)monsterPattern.staminaUp.x, (int)monsterPattern.staminaUp.y + 1);
        maxStamina += tempStamina;
        stamina = REFILL_STAMINA_LVL_UP ? maxStamina : stamina + tempLife;
        stat_might += Random.Range((int)monsterPattern.stat_mightUp.x, (int)monsterPattern.stat_mightUp.y + 1);
        stat_resistance += Random.Range((int)monsterPattern.stat_resistanceUp.x, (int)monsterPattern.stat_resistanceUp.y + 1);
        stat_luck += Random.Range((int)monsterPattern.stat_luckUp.x, (int)monsterPattern.stat_luckUp.y + 1);
        stat_speed += Random.Range((int)monsterPattern.stat_speedUp.x, (int)monsterPattern.stat_speedUp.y + 1);

        foreach (MonsterPattern.AttackLevelUp a in monsterPattern.attackLevelUp) {
            if (a.lvl == lvl)
                attacks[0]=a.attack;
        }
    }

    public void CalcExpRequired() {
        expRequired = new int[99];
        expRequired[0] = expMultiplier1;
        for (int i = 1; i < 99; i++) {
            expRequired[i] = expRequired[i - 1] + expMultiplier1 + 2 * i * expMultiplier2 + (3 * (i - 1)) * expMultiplier3;
        }
    }

    public int CalcExpGiven() {
        return 0;
    }

    /* Damages
     */
    public void Damage(Monster target, int value) {
        // Inflict damages
        if (value < 0) {
            target.life = Mathf.Min(target.life - value, target.maxLife);
        } else {
            target.life -= value;
            if (target.life <= 0) {
                if (this != target)
                    Exp(target.CalcExpGiven());
                target.Death();
            }
        }
    }

    public void Death() {
        state = State.Dead;
        life = 0;
    }

    /* Contstructors
     */
    public static Monster Generate(MonsterPattern _pattern, int lvlMin, int lvlMax) {
        int lvl = Random.Range(lvlMin, lvlMax);
        Monster m = new Monster();
        
        m.monsterPattern = _pattern;
        m.monsterName = _pattern.name;
        m.stat_might = _pattern.stat_might;
        m.stat_resistance = _pattern.stat_resistance;
        m.stat_luck = _pattern.stat_luck;
        m.stat_speed = _pattern.stat_speed;
        m.maxLife = _pattern.maxLife;
        m.maxStamina = _pattern.maxStamina;
        m.battleSprite = Resources.LoadAssetAtPath(Config.GetResourcePath(IMAGE_FOLDER) + _pattern.battleSprite, typeof(Texture2D)) as Texture2D;
        m.miniSprite = Resources.LoadAssetAtPath(Config.GetResourcePath(IMAGE_FOLDER) + _pattern.miniSprite, typeof(Texture2D)) as Texture2D;
        m.type = _pattern.type;

        m.lvl = 1;
        for (int i = 1; i < lvl; i++) {
            m.LevelUp();
        }
        return m;
    }
}
