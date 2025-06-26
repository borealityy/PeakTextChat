using UnityEngine.UI.ProceduralImage;

namespace PeakTextChat;

public static class ProceduralImageExtensions {
    public static T SetModifierType<T>(this ProceduralImage image) where T : ProceduralImageModifier {
        image.ModifierType = typeof(T);
        return image.gameObject.GetComponent<T>() ?? image.gameObject.AddComponent<T>();
    }
}