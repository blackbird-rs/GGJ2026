using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Audio
{
    public static class AudioCodeGenerator
    {
        [MenuItem("Assets/Generate Audio Code")]
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
            
            AudioSettings.Instance.allClips = allClips;
            EditorUtility.SetDirty(AudioSettings.Instance);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log($"Successfully wrote audio code to: {fullPath}");
        }

        private static AudioClipSettings[] LoadAllClips()
        {
            var allClips = Util.LoadAllAssetsOfType<AudioClipSettings>().ToArray();
            var anyDuplicates = allClips
                .GroupBy(x => x.name)
                .Any(x => x.Count() > 1);
            if (anyDuplicates)
            {
                throw new Exception("Found audio clips with duplicate IDs");
            }

            return allClips;
        }
    }
}