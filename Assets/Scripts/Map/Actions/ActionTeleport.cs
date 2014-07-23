using UnityEngine;
using System.Collections.Generic;

public class ActionTeleport : MapObjectAction {
    public MapObject target;
    public MapObject.Orientation orientation;
    public Vector2 arrival;

    public ActionTeleport(MapObject _target, MapObject.Orientation _orientation, Vector2 _arrival) {
        target = _target;
        orientation = _orientation;
        arrival = _arrival;
    }

    public override void Execute() {
        if (World.Current.CanMoveOn(arrival)) { 
            target.mapCoords = arrival;
            target.orientation = orientation;
        }
        Terminate();
    }


}
