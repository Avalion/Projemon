using UnityEngine;

/**
 * This action will change sprite of a MapObject
 */
[System.Serializable]
public class ActionTransform : MapObjectAction {
    public Texture2D newSprite;
    public MapObject target;

    public ActionTransform() {
        target = Player.Current;
    }

    public ActionTransform(MapObject _target, Texture2D _newSprite) {
        newSprite = _newSprite;
        target = _target;
    }

    public override void Execute() {
        target.sprite = newSprite;
        Terminate();
    }

    public override string InLine() {
            return "Modify sprite of " + target.name + " to " + (newSprite != null ? newSprite.name : "null")+".";
    }
}