using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Audio.Util;
using UnityEditor;
using UnityEngine;

namespace Audio
{
    public class AudioCodeGenerator : EditorWindow
    {
        private Vector2 scrollPosition = Vector2.zero;
        private readonly Dictionary<int, bool> foldouts = new() { { 0, true } };
        private AudioClip newClip;

        [MenuItem("Tools/Audio Settings Editor", false, 50)]
        public static void ShowWindow()
        {
            GetWindowWithRect<AudioCodeGenerator>(new Rect(0, 0, 600, 600), false, "Audio Settings");
        }

        private void OnGUI()
        {
            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));

            var audioSettings = GetOrCreateAudioSettings();
            if (audioSettings.allClips != null)
            {
                for (var i = 0; i < audioSettings.allClips.Length; i++)
                {
                    GUILayout.Space(5);

                    var clip = audioSettings.allClips[i];
                    foldouts[i] = EditorGUILayout.BeginFoldoutHeaderGroup(foldouts.GetValueOrDefault(i), clip.name);
                    EditorGUILayout.EndFoldoutHeaderGroup();
                    if (!foldouts[i]) continue;

                    Editor clipEditor = null;
                    Editor.CreateCachedEditor(clip, null, ref clipEditor);
                    clipEditor.OnInspectorGUI();

                    GUILayout.Space(20);
                }
            }

            if (GUI.changed)
            {
                EditorUtility.SetDirty(audioSettings);
            }
            
            EditorGUILayout.EndScrollView();

            GUILayout.Space(5);
            
            GUILayout.BeginHorizontal();
            newClip = EditorGUILayout.ObjectField(newClip, typeof(AudioClip), false) as AudioClip;
            if (GUILayout.Button("Add New Clip Settings") && newClip != null)
            {
                var newClipSettings = CreateInstance<AudioClipSettings>();
                var path = Path.Combine("Assets/Audio", $"{newClip.name.Capitalize()}.asset");
                newClipSettings.Clip = newClip;
                audioSettings.allClips = audioSettings.allClips.Append(newClipSettings).ToArray();
                AssetDatabase.CreateAsset(newClipSettings, path);
                EditorUtility.SetDirty(audioSettings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
                foldouts.Clear();
                foldouts[audioSettings.allClips.Length - 1] = true;
                newClip = null;
            }
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            if (GUILayout.Button("Generate Code"))
            {
                Generate();
            }
        }

        [MenuItem("Tools/Generate Audio Code", false, 51)]
        public static void Generate()
        {
            const string savePath = "Assets/Generated/Scripts/";
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            var allClips = LoadAllClips();
            var scriptContent = @"namespace Audio
{
    public partial class AudioManager
    {" + allClips
                .Select(clip => clip.name)
                .Aggregate("", (current, clipId) => current + $@"
        public void Play{clipId.Capitalize()}() => PlayAudio(""{clipId}"");
        public void Stop{clipId.Capitalize()}() => StopAudio(""{clipId}"");
        public void FadeIn{clipId.Capitalize()}() => FadeInAudio(""{clipId}"");
        public void FadeOut{clipId.Capitalize()}() => FadeOutAudio(""{clipId}"");") + @"
    }
}";

            var fullPath = Path.Combine(savePath, "AudioManagerGenerated.cs");
            File.WriteAllText(fullPath, scriptContent);

            var audioSettings = GetOrCreateAudioSettings();
            audioSettings.allClips = allClips;
            EditorUtility.SetDirty(audioSettings);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Successfully wrote audio code to: {fullPath}");
        }

        private static AudioSettings GetOrCreateAudioSettings()
        {
            try
            {
                return AudioSettings.Instance;
            }
            catch (Exception)
            {
                const string savePath = "Assets/Generated/Resources/";
                if (!Directory.Exists(savePath))
                {
                    Directory.CreateDirectory(savePath);
                }

                var instance = CreateInstance<AudioSettings>();
                var path = Path.Combine(savePath, $"{nameof(AudioSettings)}.asset");
                AssetDatabase.CreateAsset(instance, path);
                return instance;
            }
        }

        private static AudioClipSettings[] LoadAllClips()
        {
            var allClips = LoadAllAssetsOfType<AudioClipSettings>()
                .OrderBy(x => x.name)
                .ToArray();
            var anyDuplicates = allClips
                .GroupBy(x => x.name)
                .Any(x => x.Count() > 1);
            if (anyDuplicates)
            {
                throw new Exception("Found audio clips with duplicate IDs");
            }

            return allClips;
        }

        private static IEnumerable<T> LoadAllAssetsOfType<T>() where T : UnityEngine.Object
        {
            return AssetDatabase
                .FindAssets($"t:{typeof(T).Name}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>);
        }
    }
}