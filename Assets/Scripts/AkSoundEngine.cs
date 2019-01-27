using UnityEngine;

namespace DefaultNamespace {
    public class AkSoundEngine {
        public static void PostEvent(string name, GameObject gameObject) {
            Debug.Log("Sound effect: " + name);
        }
    }
}