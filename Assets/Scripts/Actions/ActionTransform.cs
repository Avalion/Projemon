using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

/**
 * This action will change sprite of a MapObject
 */
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

    public override string Serialize() {
        string serial = Serializer.Serialize<Texture2D>(newSprite);
        return GetType().ToString() + "|" + serial; // TODO : Add MapObjectID when MapObject are into DB
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 2)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        // TODO : Read and find MapObjectID when MapObject are into DB
        newSprite = Serializer.Deserialize<Texture2D>(values[1]);
    }
}