using UnityEngine;

namespace Audio
{
    [CreateAssetMenu]
    public class AudioClipSettings : ScriptableObject
    {
        [SerializeField] private AudioType audioType;

        [SerializeField] private AudioClip[] variants;

        [SerializeField, Range(0, 1)] private float defaultVolume = 0.8f;

        [SerializeField] private AudioLimitBehaviour limitBehaviour;

        public AudioType AudioType => audioType;

        public AudioClip[] Variants => variants;

        public float DefaultVolume => defaultVolume;

        public AudioLimitBehaviour LimitBehaviour => limitBehaviour;
    }
}