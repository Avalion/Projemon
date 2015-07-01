using UnityEngine;

public class ActionSetVariable : MapObjectAction {
    public int varId = -1;

    // TODO : MonsterIndex (get from PatternID), 
    //        MonsterPattern (get from Player.monsters index), 
    //        MonsterExp (get from Player.monsters index) !
    public enum Mode { Value, Random, Variable, MOPositionX, MOPositionY, MOOrientation, Gold, TeamCount, CollectionCount, EncouteredCount, MonsterLevel }
    public Mode mode = Mode.Value;

    public enum SetMode { Set, Add, Minus, Mult, Divide, Mod }
    public SetMode setMode = SetMode.Set;
    

    public int value = 0;
    public int value2 = 0;

    public MapObject linked = null;


    public ActionSetVariable() {}
    
    public override void Execute() {
        if (varId < 0)
            return;

        int realValue = 0;

        switch (mode) {
            case Mode.Random:
                realValue = Random.Range(value, value2); break;
            case Mode.Variable:
                realValue = DataBase.GetVariable(value); break;
            case Mode.MOPositionX:
                if (linked == null) linked = World.Current.GetMapObjectById(value);
                if (linked == null) throw new System.Exception("UnexpectedError: Couldn't refer to a null MapObject");  
                realValue = (int)linked.mapCoords.x; break;
            case Mode.MOPositionY:
                if (linked == null) linked = World.Current.GetMapObjectById(value);
                if (linked == null) throw new System.Exception("UnexpectedError: Couldn't refer to a null MapObject");
                realValue = (int)linked.mapCoords.y; break;
            case Mode.MOOrientation:
                if (linked == null) linked = World.Current.GetMapObjectById(value);
                if (linked == null) throw new System.Exception("UnexpectedError: Couldn't refer to a null MapObject");
                realValue = (int)linked.orientation; break;
            case Mode.Gold:
                realValue = Player.Current.goldCount; break;
            case Mode.TeamCount:
                realValue = Player.Current.monsters.Count; break;
            case Mode.CollectionCount:
                realValue = MonsterCollection.capturedMonsters.Count; break;
            case Mode.EncouteredCount:
                realValue = MonsterCollection.encounteredMonsters.Count; break;
            case Mode.MonsterLevel:   if (Player.Current.monsters.Count < value) throw new System.Exception("UnexpectedError: couldn't refer to monster n°" + value + ". Please Encapsulate this in a IfCondition."); 
                realValue = Player.Current.monsters[value].lvl; break;
        }

        switch (setMode) {
            case SetMode.Set:
                break;
            case SetMode.Add:
                realValue = value + realValue; break;
            case SetMode.Minus:
                realValue = value - realValue; break;
            case SetMode.Mult:
                realValue = value * realValue; break;
            case SetMode.Divide:
                realValue = value / realValue; break;
            case SetMode.Mod:
                realValue = value % realValue; break;
        }


        DataBase.SetVariable(varId, realValue);
    }

    public override string InLine() {
        DBVariable var = DataBase.SelectById<DBVariable>(varId);
        string message = "Set variable " + (var != null ? varId + ":" + var.name : "[TO DEFINE]") + " to [" + mode.ToString() + "]";
        switch (mode) {
            case Mode.Random:
                message += "(" + value + " / " + value2 + ")"; break;
            case Mode.Variable:
                DBVariable var2 = DataBase.SelectById<DBVariable>(value);
                message += value + ": " + (var2 == null ? "[TO DEFINE]" : var2.name); break;
            case Mode.MOPositionX:
            case Mode.MOPositionY:
            case Mode.MOOrientation:
                if (linked == null) linked = World.Current.GetMapObjectById(value);
                message += value + ": " + (linked == null ? "[TO DEFINE]" : linked.name); break;
            case Mode.Gold:
            case Mode.TeamCount:
            case Mode.CollectionCount:
            case Mode.EncouteredCount:
                break;
            case Mode.MonsterLevel: 
                message += value + "°"; break;
        }
        return message;
    }

    public override string Serialize() {
        // TODO : Add MapObjectID when MapObject are into DB
        return GetType().ToString() + "|" + varId + "|" + value + "|" + value2;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 4)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        int.TryParse(values[1], out varId);
        int.TryParse(values[2], out value);
        int.TryParse(values[3], out value2);
    }
}
