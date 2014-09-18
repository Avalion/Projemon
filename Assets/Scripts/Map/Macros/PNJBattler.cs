using UnityEngine;
using System.Collections.Generic;

/**
 * This class design a PNJ which will start a battle.
 */
public class PNJBattler : Battler {
    public int distance;
    public int nbWin;

    // TEMPORARY
    public override void OnStart() {
        monsters.Add(Monster.GenerateFromPattern(SystemDatas.GetMonsterPatterns()[5], 5, 5));

        List<PossibleMovement> listMovement = new List<PossibleMovement>() { PossibleMovement.LookPlayer };
        actions.Add(new ActionMessage(null, "Waouh, un autre dresseur de monstre ! Laisse moi tester tes talents !", false));
        for (int i = 0; i < distance; i++)
            listMovement.Add(PossibleMovement.Forward);
        actions.Add(new ActionMove(this, listMovement));
        actions.Add(new ActionWait(0.5f));
        actions.Add(new ActionFadeScreen(new Color(0, 0, 0, 1), 1.5f, true));
        actions.Add(new ActionWait(0.2f));
        actions.Add(new ActionPNJBattle(this));
        actions.Add(new ActionFadeScreen(new Color(0, 0, 0, 0), 1.5f, true));
    }

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
}