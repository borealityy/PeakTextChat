using System.Collections.Generic;
using UnityEngine;

namespace PeakTextChat;

public static class KeysHelper {
    public static KeyCodeInfo GetKeyCodeShortInfo(KeyCodeShort key) {
        return infos.Find((i) => i.shortKey == key);
    }

    public class KeyCodeInfo {
        public KeyCodeShort shortKey;
        public KeyCode key;
        public string keyText;

        public KeyCodeInfo(KeyCodeShort shortKey,KeyCode key,string keyText) {
            this.shortKey = shortKey;
            this.key = key;
            this.keyText = keyText;
        }
    }

    public static List<KeyCodeInfo> infos = new List<KeyCodeInfo>() {
        new(KeyCodeShort.A,KeyCode.A,"A"),
        new(KeyCodeShort.B,KeyCode.B,"B"),
        new(KeyCodeShort.C,KeyCode.C,"C"),
        new(KeyCodeShort.D,KeyCode.D,"D"),
        new(KeyCodeShort.E,KeyCode.E,"E"),
        new(KeyCodeShort.F,KeyCode.F,"F"),
        new(KeyCodeShort.G,KeyCode.G,"G"),
        new(KeyCodeShort.H,KeyCode.H,"H"),
        new(KeyCodeShort.I,KeyCode.I,"I"),
        new(KeyCodeShort.J,KeyCode.J,"J"),
        new(KeyCodeShort.K,KeyCode.K,"K"),
        new(KeyCodeShort.L,KeyCode.L,"L"),
        new(KeyCodeShort.M,KeyCode.M,"M"),
        new(KeyCodeShort.N,KeyCode.N,"N"),
        new(KeyCodeShort.O,KeyCode.O,"O"),
        new(KeyCodeShort.P,KeyCode.P,"P"),
        new(KeyCodeShort.Q,KeyCode.Q,"Q"),
        new(KeyCodeShort.R,KeyCode.R,"R"),
        new(KeyCodeShort.S,KeyCode.S,"S"),
        new(KeyCodeShort.T,KeyCode.T,"T"),
        new(KeyCodeShort.U,KeyCode.U,"U"),
        new(KeyCodeShort.V,KeyCode.V,"V"),
        new(KeyCodeShort.W,KeyCode.W,"W"),
        new(KeyCodeShort.X,KeyCode.X,"X"),
        new(KeyCodeShort.Y,KeyCode.Y,"Y"),
        new(KeyCodeShort.Z,KeyCode.Z,"Z"),
        new(KeyCodeShort.One,KeyCode.Alpha1,"1"),
        new(KeyCodeShort.Two,KeyCode.Alpha2,"2"),
        new(KeyCodeShort.Three,KeyCode.Alpha3,"3"),
        new(KeyCodeShort.Four,KeyCode.Alpha4,"4"),
        new(KeyCodeShort.Five,KeyCode.Alpha5,"5"),
        new(KeyCodeShort.Six,KeyCode.Alpha6,"6"),
        new(KeyCodeShort.Seven,KeyCode.Alpha7,"7"),
        new(KeyCodeShort.Eight,KeyCode.Alpha8,"8"),
        new(KeyCodeShort.Nine,KeyCode.Alpha9,"9"),
        new(KeyCodeShort.Zero,KeyCode.Alpha0,"0"),
        new(KeyCodeShort.Minus,KeyCode.Minus,"-"),
        new(KeyCodeShort.Equals,KeyCode.Equals,"="),
        new(KeyCodeShort.Tab,KeyCode.Tab,"Tab"),
        new(KeyCodeShort.Quote,KeyCode.Quote,"'"),
        new(KeyCodeShort.Semicolon,KeyCode.Semicolon,";"),
        new(KeyCodeShort.LeftBracket,KeyCode.LeftBracket,"["),
        new(KeyCodeShort.RightBracket,KeyCode.RightBracket,"]"),
        new(KeyCodeShort.Slash,KeyCode.Slash,"/"),
        new(KeyCodeShort.Numpad0,KeyCode.Keypad0,"Num0"),
        new(KeyCodeShort.Numpad1,KeyCode.Keypad1,"Num1"),
        new(KeyCodeShort.Numpad2,KeyCode.Keypad2,"Num2"),
        new(KeyCodeShort.Numpad3,KeyCode.Keypad3,"Num3"),
        new(KeyCodeShort.Numpad4,KeyCode.Keypad4,"Num4"),
        new(KeyCodeShort.Numpad5,KeyCode.Keypad5,"Num5"),
        new(KeyCodeShort.Numpad6,KeyCode.Keypad6,"Num6"),
        new(KeyCodeShort.Numpad7,KeyCode.Keypad7,"Num7"),
        new(KeyCodeShort.Numpad8,KeyCode.Keypad8,"Num8"),
        new(KeyCodeShort.Numpad9,KeyCode.Keypad9,"Num9"),
    };
}

public enum KeyCodeShort {
    A,
    B,
    C,
    D,
    E,
    F,
    G,
    H,
    I,
    J,
    K,
    L,
    M,
    N,
    O,
    P,
    Q,
    R,
    S,
    T,
    U,
    V,
    W,
    X,
    Y,
    Z,
    One,
    Two,
    Three,
    Four,
    Five,
    Six,
    Seven,
    Eight,
    Nine,
    Zero,
    Minus,
    Equals,
    Tab,
    Quote,
    Semicolon,
    LeftBracket,
    RightBracket,
    Slash,
    Numpad0,
    Numpad1,
    Numpad2,
    Numpad3,
    Numpad4,
    Numpad5,
    Numpad6,
    Numpad7,
    Numpad8,
    Numpad9
}