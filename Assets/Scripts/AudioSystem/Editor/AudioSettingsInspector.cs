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

            if (GUILayout.Button("Generate Code"))
            {
                AudioCodeGenerator.Generate();
            }
        }
    }
}