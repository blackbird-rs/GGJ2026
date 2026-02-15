using System;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu]
    public class AudioSettings : Util.ScriptableSingleton<AudioSettings>
    {
        public AudioClipSettings[] allClips = Array.Empty<AudioClipSettings>();
    }
}