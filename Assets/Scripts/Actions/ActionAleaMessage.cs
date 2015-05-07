using UnityEngine;
using System.Collections.Generic;

/**
 * This action will add display a random message
 */
public class ActionAleaMessage : MapObjectAction {
    public Texture2D face;
    public bool faceOnRight;

    public static string[] m_messagesList = new string[] {
        "Je t'ai déjà dit ce que je savais !",
        "Laisse moi tranquille maintenant !",
        "Arrête de me harceler !",
        "Heu... on s'est déjà salué...",
    };

    
    public override void Execute() {
        World.Current.ExecuteActions(new ActionMessage[] { new ActionMessage(face, m_messagesList[Random.Range(0, m_messagesList.Length - 1)], faceOnRight) }, delegate() { Terminate(); });
    }
    public override string InLine() {
        return "Alea Message.";
    }

    public override string Serialize() {
        return GetType().ToString() + "|" + face.name + "|" + faceOnRight;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 3)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        face = face = InterfaceUtility.GetTexture(Config.GetResourcePath(ActionMessage.IMAGE_FOLDER) + values[1] + ".png");
        faceOnRight = bool.Parse(values[3]);
    }
}
