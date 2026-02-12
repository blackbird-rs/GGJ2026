using UnityEditor;
using UnityEngine;

namespace Audio
{
    [CustomEditor(typeof(AudioClipSettings))]
    public class AudioClipSettingsInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Generate Code"))
            {
                AudioCodeGenerator.Generate();
            }
        }
    }
}