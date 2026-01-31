using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio
{
	public class AudioManager : MonoSingleton<AudioManager>
	{
		private readonly Dictionary<AudioClipSettings, Coroutine> fadeCoroutines = new();

		private readonly List<AudioSource> musicSources = new();

		private readonly List<AudioSource> soundSources = new();

		private bool? musicEnabled;
    
		private bool? soundEnabled;

		public bool MusicEnabled
		{
			get => musicEnabled.GetValueOrDefault(true);
			set
			{
				if (value != musicEnabled)
				{
					musicEnabled = value;
					musicSources.ForEach(s => s.mute = !value);
				}
			}
		}

		public bool SoundEnabled
		{
			get => soundEnabled.GetValueOrDefault(true);
			set
			{
				if (value != soundEnabled)
				{
					soundEnabled = value;
					soundSources.ForEach(s => s.mute = !value);
				}
			}
		}

		public void PlayAudio(AudioClipSettings settings)
		{
			var sourceList = DetermineSourceList(settings.AudioType);
			AudioSource source = null;
			switch (settings.LimitBehaviour)
			{
				case AudioLimitBehaviour.DoNotLimit:
					break;
				case AudioLimitBehaviour.DiscardOldInstance:
					source = FindSourceByClip(sourceList, settings);
					if (source != null) source.Stop();
					break;
				case AudioLimitBehaviour.DiscardNewInstance:
					source = FindSourceByClip(sourceList, settings);
					if (source != null) return;
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}

			if (source == null) source = FindInactiveSource(sourceList);

			if (source == null) source = InstantiateAudioSource(settings.AudioType);

			SetupAndPlayClip(settings, source);
		}

		public void StopAudio(AudioClipSettings settings)
		{
			var sourceList = DetermineSourceList(settings.AudioType);
			var source = FindSourceByClip(sourceList, settings);
			if (source != null) source.Stop();
		}

		private List<AudioSource> DetermineSourceList(AudioType audioType)
		{
			switch (audioType)
			{
				case AudioType.Music:
					return musicSources;
				case AudioType.Sound:
					return soundSources;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private bool DetermineSourceMuteValue(AudioType audioType)
		{
			switch (audioType)
			{
				case AudioType.Music:
					return !MusicEnabled;
				case AudioType.Sound:
					return !SoundEnabled;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		private static AudioSource FindSourceByClip(List<AudioSource> sourceList, AudioClipSettings settings)
		{
			return sourceList.Find(s => settings.Variants.Contains(s.clip));
		}

		private static AudioSource FindInactiveSource(List<AudioSource> sourceList)
		{
			return sourceList.Find(s => !s.isPlaying);
		}

		private AudioSource InstantiateAudioSource(AudioType audioType)
		{
			var sourceList = DetermineSourceList(audioType);
			var obj = new GameObject($"{nameof(AudioSource)}{sourceList.Count + 1}");
			obj.transform.SetParent(transform);

			var source = obj.AddComponent<AudioSource>();
			source.mute = DetermineSourceMuteValue(audioType);
			source.playOnAwake = false;
			sourceList.Add(source);
			return source;
		}

		private static void SetupAndPlayClip(AudioClipSettings settings, AudioSource source)
		{
			source.loop = settings.AudioType == AudioType.Music;
			source.clip = settings.Variants[Random.Range(0, settings.Variants.Length)];
			source.volume = settings.DefaultVolume;
			source.Play();
		}

		public void FadeAudio(AudioClipSettings settings, float fadeTo, float fadeDuration)
		{
			var sourceList = DetermineSourceList(settings.AudioType);
			var source = FindSourceByClip(sourceList, settings);
			if (source == null) return;

			if (fadeCoroutines.TryGetValue(settings, out var coroutine)) StopCoroutine(coroutine);
			coroutine = StartCoroutine(FadeAudioFlow(source, source.volume, fadeTo, fadeDuration));
			fadeCoroutines[settings] = coroutine;
		}

		private static IEnumerator FadeAudioFlow(AudioSource source, float fadeFrom, float fadeTo, float fadeDuration)
		{
			if (fadeDuration <= 0f)
			{
				source.volume = fadeTo;
				yield break;
			}

			var progress = 0f;
			while (progress < 1f)
			{
				yield return null;

				progress += Time.deltaTime / fadeDuration;
				source.volume = Mathf.Lerp(fadeFrom, fadeTo, progress);
			}
		}
	}
}