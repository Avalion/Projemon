//using UnityEngine;
//using System.Collections.Generic;

///**
// * This action will launch a battle
// */
//[System.Serializable]
//public class ActionPNJBattle : MapObjectAction {
//    public PNJBattler battler;

//    public ActionPNJBattle() {}
//    public ActionPNJBattle(PNJBattler battler) {
//        this.battler = battler;
//    }

//    public override void Execute() {
//        Battle.Launch(this, battler);
//    }

//    public override void Terminate() {
//        base.Terminate();
//        battler.nbWin++;
//    }

//    public override string InLine() {
//        return "Battle pnj : " + battler.name+".";
//    }
//    public override string Serialize() {
//        // TODO : Add MapObjectID when MapObject are into DB
//        return GetType().ToString();
//    }
//    public override void Deserialize(string s) {
//        string[] values = s.Split('|');
//        if (values.Length != 1)
//            throw new System.Exception("SerializationError : elements count doesn't match... " + s);
//        // TODO : Read and find MapObjectID when MapObject are into DB
//    }
//}
