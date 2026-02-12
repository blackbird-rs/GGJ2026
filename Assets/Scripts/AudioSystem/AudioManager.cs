using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Audio
{
	public partial class AudioManager : MonoSingleton<AudioManager>
	{
		private readonly Dictionary<AudioClipSettings, Coroutine> fadeCoroutines = new();

		private readonly List<AudioSource> musicSources = new();

		private readonly List<AudioSource> soundSources = new();

		private AudioClipSettings[] allClips;

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

		protected override void OnAwake()
		{
			base.OnAwake();
			allClips = AudioSettings.Instance.allClips;
		}

		private void Start()
		{
			foreach (var clip in allClips)
			{
				if (clip.PlayOnStart)
				{
					PlayAudio(clip.name);
				}
			}
		}

		public void PlayAudio(string settingsName)
		{
			var settings = allClips.First(x => x.name == settingsName);
			if (settings.ChannelMode == ChannelMode.StartStop)
			{
				foreach (var otherClip in allClips.Where(x => x.Channel == settings.Channel))
				{
					StopAudio(otherClip.name);
				}
			}

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

			if (settings.PlayNext != null)
			{
				StartCoroutine(PlayNextLater(settings));
			}
		}

		public void StopAudio(string settingsName)
		{
			var settings = allClips.First(x => x.name == settingsName);
			var sourceList = DetermineSourceList(settings.AudioType);
			var source = FindSourceByClip(sourceList, settings);
			if (source != null) source.Stop();
		}

		private IEnumerator PlayNextLater(AudioClipSettings settings)
		{
			yield return new WaitForSeconds(settings.Clip.length);

			StopAudio(settings.name);
			PlayAudio(settings.PlayNext.name);
		}

		private List<AudioSource> DetermineSourceList(AudioType audioType)
		{
			return audioType switch
			{
				AudioType.Music or AudioType.MusicIntro => musicSources,
				AudioType.Sound => soundSources,
				_ => throw new ArgumentOutOfRangeException()
			};
		}

		private bool DetermineSourceMuteValue(AudioType audioType)
		{
			return audioType switch
			{
				AudioType.Music or AudioType.MusicIntro => !MusicEnabled,
				AudioType.Sound => !SoundEnabled,
				_ => throw new ArgumentOutOfRangeException()
			};
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
			var typeName = audioType switch
			{
				AudioType.Music or AudioType.MusicIntro => "Music",
				AudioType.Sound => "Sound",
				_ => throw new ArgumentOutOfRangeException()
			};

			var obj = new GameObject($"{typeName}{sourceList.Count + 1}");
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
			source.clip = settings.Variants[Random.Range(0, settings.Variants.Count)];
			source.volume = settings.GetDefaultVolume(true);
			source.Play();
		}

		public void FadeInAudio(string settingsName)
		{
			var settings = allClips.First(x => x.name == settingsName);
			var clipsToFadeOut = allClips.Where(x =>
					x.ChannelMode == ChannelMode.CrossFade && x.Channel == settings.Channel && x.name != settings.name);

			foreach (var clip in clipsToFadeOut)
			{
				FadeAudioTo(clip, 0);
			}

			FadeAudioTo(settings, settings.GetDefaultVolume(false));
		}

		public void FadeOutAudio(string settingsName)
		{
			var settings = allClips.First(x => x.name == settingsName);
			FadeAudioTo(settings, 0);
		}

		private void FadeAudioTo(AudioClipSettings settings, float fadeTo)
		{
			var sourceList = DetermineSourceList(settings.AudioType);
			var source = FindSourceByClip(sourceList, settings);
			if (source == null) return;

			if (fadeCoroutines.TryGetValue(settings, out var coroutine)) StopCoroutine(coroutine);
			coroutine = StartCoroutine(FadeAudioFlow(source, source.volume, fadeTo, settings.FadeDurationSeconds));
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