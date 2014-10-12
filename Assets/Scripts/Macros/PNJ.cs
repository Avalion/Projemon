using UnityEngine;
using System.Collections.Generic;

/**
 * This class design a PNJ which will start a battle.
 */
public class PNJ : MapObject {

    // TEMPORARY
    public override void OnStart() {
        List<PossibleMovement> listMovement = new List<PossibleMovement>() { PossibleMovement.FollowPlayer };
        actions.Add(new ActionMove(this, listMovement));        
    }

    public override void OnUpdate() {   
        if(!isRunning){
          ExecuteActions();
        }
    }
}