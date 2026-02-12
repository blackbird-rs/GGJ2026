using UnityEngine;

namespace Audio
{
    [CreateAssetMenu]
    public class AudioSettings : ScriptableSingleton<AudioSettings>
    {
        public AudioClipSettings[] allClips;
    }
}