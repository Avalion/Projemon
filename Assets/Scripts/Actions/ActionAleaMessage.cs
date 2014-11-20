using UnityEngine;
using System.Collections.Generic;

public class ActionAleaMessage : MapObjectAction {
    public Texture2D face;
    bool faceOnRight;

    public static string[] m_messagesList = new string[] {
        "Je t'ai déjà dit ce que je savais !",
        "Laisse moi tranquille maintenant !",
        "Arrête de me harceler !",
        "Heu... on s'est déjà salué !",
    };

    
    
    public override void Execute() {
        new ActionMessage(face, m_messagesList[Random.Range(0, m_messagesList.Length)], faceOnRight).Execute();
    }
    public override string InLine() {
        return "Alea Message.";
    }

    public override string Serialize() {
        return GetType().ToString() + "|" + Serializer.Serialize<Texture2D>(face) + "|" + faceOnRight;
    }
    public override void Deserialize(string s) {
        string[] values = s.Split('|');
        if (values.Length != 3)
            throw new System.Exception("SerializationError : elements count doesn't match... " + s);

        face = Serializer.Deserialize<Texture2D>(values[1]);
        faceOnRight = bool.Parse(values[3]);
    }
}
