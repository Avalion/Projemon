//using UnityEngine;
//using System.Collections.Generic;

//public class Serializer {
//    public static string Serialize<T> (object toSerialize) where T : class {
//        switch (typeof(T).ToString()) {
//            case "UnityEngine.Texture2D" :
//                return SerializeTexture((Texture2D)toSerialize);
//        }
//        throw new System.Exception("The type " + typeof(T).ToString() + " cannot be serialized...");
//    }
//    public static T Deserialize<T>(string serial) where T : class {
//        switch (typeof(T).ToString()) {
//            case "UnityEngine.Texture2D":
//                return DeserializeTexture(serial) as T;
//        }

//        throw new System.Exception("The type " + typeof(T).ToString() + " cannot be serialized...");
//    }

//    #region Texture2D
//    private static string SerializeTexture(Texture2D toSerialize) {
//        byte[] bytes = toSerialize.EncodeToPNG();
//        string serial = "";
//        if (bytes.Length > 0) {
//            serial = bytes[0].ToString();
//            for (int i = 1; i < bytes.Length; ++i) {
//                serial += "#" + bytes[i].ToString();
//            }
//        }
//        return serial;
//    }
//    private static Texture2D DeserializeTexture(string serial) {
//        string[] values = serial.Split('#');
//        List<byte> bytes = new List<byte>();
//        foreach (string se in values)
//            bytes.Add(byte.Parse(se));

//        Texture2D t = new Texture2D(1, 1);
//        t.LoadImage(bytes.ToArray());
//        return t;
//    }
//    #endregion
//}
