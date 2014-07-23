using UnityEngine;
using System.Collections.Generic;

/**
 * This class design a PNJ which will start a battle.
 */
public class PNJBattler : MapObject {
    public List<Monster> monsters;

    public int distance;

    public int nbWin;

    public AudioClip temp;

    public override void OnUpdate() {
        if (!isRunning && nbWin == 0 && (
            orientation == Orientation.Down  && Player.Current.mapCoords.x == mapCoords.x && Player.Current.mapCoords.y - mapCoords.y <= distance && Player.Current.mapCoords.y - mapCoords.y > 0 ||
            orientation == Orientation.Up    && Player.Current.mapCoords.x == mapCoords.x && mapCoords.y - Player.Current.mapCoords.y <= distance && mapCoords.y - Player.Current.mapCoords.y > 0 ||
            orientation == Orientation.Left  && Player.Current.mapCoords.y == mapCoords.y && Player.Current.mapCoords.x - mapCoords.x <= distance && Player.Current.mapCoords.x - mapCoords.x > 0 ||
            orientation == Orientation.Right && Player.Current.mapCoords.y == mapCoords.y && mapCoords.x - Player.Current.mapCoords.x <= distance && mapCoords.x - Player.Current.mapCoords.x > 0)
            ) {
                ExecuteActions();
        }
    }

    // TEMPORARY
    public void Start() {
        actions.Add(new ActionMessage(null , "Waouh, un autre dresseur de monstre ! Laisse moi tester tes talents !", false));
        for (int i = 0; i < distance; i++)
            actions.Add(new ActionMove(this, PossibleMovement.Forward));
        actions.Add(new ActionWait(0.5f));
        actions.Add(new ActionPlaySound(temp, true, false));
        actions.Add(new ActionFade(new Color(1, 0, 0, 0.5f), 0));

        actions.Add(new ActionWait(1f));
        actions.Add(new ActionFade(new Color(0, 0, 0, 0), 2));
        //actions.Add(new ActionWait(0.5f));
        
        //actions.Add(new ActionBattle(this));
    }
}