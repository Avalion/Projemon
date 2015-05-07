using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;

/**
 * This action will change sprite of a MapObject
 */
public class ActionTransform : MapObjectAction {
    public int mapObjectId;
    public string newSpritePath = "";
    private Texture2D newSprite;

    public ActionTransform() {
        mapObjectId = Player.Current.mapObjectId;
    }

    public ActionTransform(MapObject _target, Texture2D _newSprite) {
        newSpritePath = _newSprite.name;
        newSprite = _newSprite;
        mapObjectId = _target.mapObjectId;
    }

    public override void Execute() {
        if (newSprite == null) { newSprite = InterfaceUtility.GetTexture(Config.GetResourcePath(MapObject.IMAGE_FOLDER) + newSpritePath); }
        MapObject target = World.Current.GetMapObjectById(mapObjectId);
        target.sprite = newSprite;
        Terminate();
    }

    public override string InLine() {
        DBMapObject moa = DataBase.SelectById<DBMapObject>(mapObjectId);
        return "Modify sprite of " + (mapObjectId == -1 ? "Player" : (moa != null ? mapObjectId + ":" + moa.name : "[TO DEFINE]")) + " to " + newSpritePath + ".";
    }

    public override string Serialize() {
        return GetType().ToString() + "|" + mapObjectId + "|" + newSpritePath;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 3)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        mapObjectId = int.Parse(values[1]);
        newSpritePath = values[2];
    }
}