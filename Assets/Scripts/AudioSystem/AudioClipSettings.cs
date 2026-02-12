using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Audio
{
    [CreateAssetMenu]
    public class AudioClipSettings : ScriptableObject
    {
        [field: SerializeField] public AudioClip Clip { get; private set; }
        [field: SerializeField] public bool UseVariants { get; private set; }
        [field: SerializeField] public AudioClip[] OtherVariants { get; private set; }
        [field: SerializeField] public AudioType AudioType { get; private set; }
        [field: SerializeField] public bool PlayOnStart { get; private set; }
        [field: SerializeField] public bool StartMuted { get; private set; }
        [field: SerializeField] public AudioLimitBehaviour LimitBehaviour { get; private set; }
        [field: SerializeField, Range(0, 1f)] public float DefaultVolume { get; private set; } = 0.8f;
        [field: SerializeField] public float FadeDurationSeconds { get; private set; } = 1f;
        [field: SerializeField] public ChannelMode ChannelMode { get; private set; }
        [field: SerializeField, Range(1, 10)] public int ChannelToUse { get; private set; } = 1;
        [field: SerializeField] public AudioClipSettings PlayNext { get; private set; }

        public List<AudioClip> Variants => UseVariants
            ? new List<AudioClip> { Clip }.Concat(OtherVariants).ToList()
            : new List<AudioClip> { Clip };

        public int? Channel => ChannelMode != ChannelMode.Ignore ? ChannelToUse : null;

        public float GetDefaultVolume(bool isStarting) =>
            isStarting && StartMuted ? 0 : DefaultVolume;
    }
}