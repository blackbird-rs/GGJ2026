using UnityEditor;
using UnityEngine;

namespace Audio
{
    [CustomEditor(typeof(AudioSettings))]
    public class AudioSettingsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(20);

            if (GUILayout.Button("Audio Settings Editor"))
            {
                AudioCodeGenerator.ShowWindow();
            }

            GUILayout.Space(5);

            if (GUILayout.Button("Generate Audio Code"))
            {
                AudioCodeGenerator.Generate();
            }
        }
    }
}