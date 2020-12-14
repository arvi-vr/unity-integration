namespace ARVI.SDK
{
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(IntegrationSettings))]
    public class IntegrationSettingsEditor : Editor
    {
        private GUIStyle wordWrappedTextAreaStyle;
        private SerializedProperty appKeyProperty;
        private string appKey;

        protected virtual void OnEnable()
        {
            appKeyProperty = serializedObject.FindProperty("appKey");
            if (appKeyProperty != null)
                appKey = appKeyProperty.stringValue;
            else
                appKey = "";
        }

        [MenuItem("ARVI/Integration/Settings")]
        public static void EditSettings()
        {
            Selection.activeObject = IntegrationSettings.Instance;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.LabelField("Application Key:", EditorStyles.boldLabel);
            appKey = EditorGUILayout.TextArea(appKey, GetTextAreaStyle());
            if (string.IsNullOrEmpty(appKey.Trim()))
                EditorGUILayout.HelpBox("Please enter a valid Application Key. You can find it in your game info page.", MessageType.Error);

            if (appKeyProperty != null)
                appKeyProperty.stringValue = appKey.Trim();

            serializedObject.ApplyModifiedProperties();
        }

        private GUIStyle GetTextAreaStyle()
        {
            if (wordWrappedTextAreaStyle == null)
                wordWrappedTextAreaStyle = new GUIStyle(EditorStyles.textArea)
                {
                    wordWrap = true,
                };
            return wordWrappedTextAreaStyle;
        }
    }
}