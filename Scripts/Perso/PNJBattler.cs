using UnityEngine;
using System.Collections.Generic;

/**
 * This class design a PNJ which will start a battle.
 */
public class PNJBattler : MapObject {
    public List<Monster> monsters;

    public int distance;

    public int nbWin;

    
    public override void OnUpdate() {
        if (nbWin == 0 && (
            orientation == Orientation.Down  && Player.Current.mapCoords.x == mapCoords.x && Player.Current.mapCoords.y - mapCoords.y <= distance && Player.Current.mapCoords.y - mapCoords.y > 0 ||
            orientation == Orientation.Up    && Player.Current.mapCoords.x == mapCoords.x && mapCoords.y - Player.Current.mapCoords.y <= distance && mapCoords.y - Player.Current.mapCoords.y > 0 ||
            orientation == Orientation.Left  && Player.Current.mapCoords.y == mapCoords.y && Player.Current.mapCoords.x - mapCoords.x <= distance && Player.Current.mapCoords.x - mapCoords.x > 0 ||
            orientation == Orientation.Right && Player.Current.mapCoords.y == mapCoords.y && mapCoords.x - Player.Current.mapCoords.x <= distance && mapCoords.x - Player.Current.mapCoords.x > 0)
            ) {

            Battle.Launch(new List<Monster>(monsters));
            nbWin++;
        }
    }

    
}