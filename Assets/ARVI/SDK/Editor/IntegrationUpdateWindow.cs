namespace ARVI.SDK
{
    using System;
    using UnityEditor;
    using UnityEngine;

    public class IntegrationUpdateWindow : EditorWindow
    {
        private static GUIStyle largeStyle;
        private static GUIStyle normalStyle;

        private Version version;
        private string description;
        private DateTime date;
        private string versionURL;
        private bool isNewVersion;

        private const string WINDOW_TITLE = "ARVI Integration Update Checker";
        private const float WINDOW_WIDTH = 500f;
        private const float WINDOW_HEIGHT = 300f;

        public static IntegrationUpdateWindow Show(Version version, string description, DateTime date, string versionURL, bool isNewVersion)
        {
            var window = GetWindow<IntegrationUpdateWindow>(true, WINDOW_TITLE, true);

            window.position = new Rect(0, 0, WINDOW_WIDTH, WINDOW_HEIGHT);
            window.CenterOnMainWindow();
            window.version = version;
            window.description = description;
            window.date = date;
            window.versionURL = versionURL;
            window.isNewVersion = isNewVersion;

            return window;
        }

        protected virtual void OnGUI()
        {
            if (version == null)
                return;

            InitializeStyles();

            GUILayout.Label(isNewVersion ? "New version available!" : "Already up to date", largeStyle);
#if UNITY_4_6 || UNITY_5_0
            GUILayout.Label(string.Format("Installed Version: <b>{0}</b>\nLatest Version: <b>{1}</b> (published on {2})\n\n{3}", Integration.Version, version, date.ToLocalTime(), description), normalStyle);
#else
            GUILayout.Label(new GUIContent(string.Format("Installed Version: <b>{0}</b>\nLatest Version: <b>{1}</b> (published on {2})\n\n{3}", Integration.Version, version, date.ToLocalTime(), description)), normalStyle);
#endif
            GUILayout.FlexibleSpace();

            if (isNewVersion)
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = new Color(0.5f, 0.5f, 1f);
                if (GUILayout.Button("Download", GUILayout.Height(30), GUILayout.MaxWidth(150)))
                    Application.OpenURL(versionURL);
                GUI.backgroundColor = originalColor;
                GUILayout.EndVertical();
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                GUILayout.FlexibleSpace();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Skip this version", GUILayout.Height(20), GUILayout.MaxWidth(120)))
                {
                    EditorPrefs.SetString(IntegrationUpdateChecker.SKIPPED_VERSION_KEY, version.ToString());
                    Close();
                }

                GUILayout.FlexibleSpace();
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    var toggleValue = GUILayout.Toggle(IntegrationUpdateChecker.ShouldCheckForUpdates, "Automatically check for updates");
                    if (changeCheckScope.changed)
                        IntegrationUpdateChecker.ShouldCheckForUpdates = toggleValue;
                }
                GUILayout.EndHorizontal();
            }
            else
            {
                GUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    var toggleValue = GUILayout.Toggle(IntegrationUpdateChecker.ShouldCheckForUpdates, "Automatically check for updates");
                    if (changeCheckScope.changed)
                        IntegrationUpdateChecker.ShouldCheckForUpdates = toggleValue;
                }
                GUILayout.EndHorizontal();
            }
        }

        private static void InitializeStyles()
        {
            if (largeStyle == null)
            {
                largeStyle = new GUIStyle(EditorStyles.largeLabel)
                {
                    fontSize = 28,
                    alignment = TextAnchor.UpperCenter,
                    richText = true
                };
            }

            if (normalStyle == null)
            {
                normalStyle = new GUIStyle(EditorStyles.label)
                {
                    wordWrap = true,
                    richText = true
                };
            }
        }
    }
}