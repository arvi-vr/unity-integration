namespace ARVI.SDK
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using UnityEditor;
    using UnityEngine;

    [InitializeOnLoad]
    public class IntegrationUpdateChecker : EditorWindow
    {
        [Serializable]
        private sealed class VersionInfo
        {
            [NonSerialized]
            public Version version;
            [NonSerialized]
            public DateTime publishedDateTime;
            [NonSerialized]
            public List<string> changelogPages;

#pragma warning disable 649
            public string html_url;
            public string tag_name;
            public string name;
            public string published_at;
            public string body;
#pragma warning restore 649

            public static VersionInfo FromJSON(string json)
            {
                VersionInfo versionInfo = JsonUtility.FromJson<VersionInfo>(json);
                versionInfo.version = new Version(versionInfo.tag_name);
                versionInfo.publishedDateTime = DateTime.Parse(versionInfo.published_at);

                string changelog = versionInfo.body;
                versionInfo.body = null;

                changelog = Regex.Replace(changelog, @"(?<!(\r\n){2}) \*.*\*{2}(.*)\*{2}", "\n<size=13>$2</size>");
                changelog = Regex.Replace(changelog, @"(\r\n){2} \*.*\*{2}(.*)\*{2}", "\n\n<size=13>$2</size>");
                changelog = new Regex(@"(#+)\s?(.*)\b").Replace(
                    changelog,
                    match => string.Format(
                        "<size={0}>{1}</size>",
                        Math.Max(8, 24 - match.Groups[1].Value.Length * 6),
                        match.Groups[2].Value
                    )
                );
                changelog = Regex.Replace(changelog, @"(\*\s+)\b", "  • ");

                // Each char gets turned into two triangles and the mesh vertices limit is 2^16.
                // Let's use another 100 to be on the safe side.
                const int textLengthLimit = 65536 / 4 - 100;
                versionInfo.changelogPages = new List<string>((int)Mathf.Ceil(changelog.Length / (float)textLengthLimit));

                while (true)
                {
                    if (changelog.Length > textLengthLimit)
                    {
                        int startIndex = Math.Min(changelog.Length, textLengthLimit);
                        int lastIndexOf = changelog.LastIndexOf("\n", startIndex, StringComparison.Ordinal);

                        if (lastIndexOf == -1)
                        {
                            lastIndexOf = startIndex;
                        }

                        versionInfo.changelogPages.Add(changelog.Substring(0, lastIndexOf));
                        changelog = changelog.Substring(lastIndexOf).TrimStart('\n', '\r');
                    }
                    else
                    {
                        versionInfo.changelogPages.Add(changelog);
                        break;
                    }
                }

                return versionInfo;
            }
        }

        private const string latestReleaseGitHubURL = "https://api.github.com/repos/arvi-vr/unity-integration/releases/latest";
        private const string autoCheckKey = "ARVI.SDK.AutoCheckForUpdates";
        private const string lastCheckKey = "ARVI.SDK.LastUpdateCheckTicks";
        private const int checkUpdateHours = 4; // checking for updates frequency

        private static bool isManualCheck;
        private static bool versionChecked;
        private static WWW versionResource;
        private static VersionInfo updateInfo;
        private static IntegrationUpdateChecker checkerWindow;

        private static Vector2 scrollPosition;
        private static Vector2 changelogScrollPosition;
        private static float changelogWidth;
        private static int changelogPageIndex;
        private static bool isChangelogFoldOut = true;

        static IntegrationUpdateChecker()
        {
            EditorApplication.update += CheckForUpdate;
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        public void OnGUI()
        {
            using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollViewScope.scrollPosition;

                if (versionResource != null && !versionResource.isDone)
                {
                    EditorGUILayout.HelpBox("Checking for updates...", MessageType.Info);
                    return;
                }

                if (updateInfo == null)
                {
                    EditorGUILayout.HelpBox("There was a problem checking for updates.", MessageType.Error);
                    DrawCheckAgainButton();

                    return;
                }

                bool isUpToDate = new Version(Integration.Version) >= updateInfo.version;

                EditorStyles.helpBox.richText = true;

                EditorGUILayout.HelpBox(
                    string.Format(
                        "{0}.\nInstalled Version: {1}\nAvailable version: {2} (published on {3})",
                        isUpToDate ? "Already up to date" : "<b>ARVI Integration update available</b>",
                        Integration.Version,
                        updateInfo.version,
                        updateInfo.publishedDateTime.ToLocalTime()),
                    isUpToDate ? MessageType.Info : MessageType.Warning);
                EditorStyles.helpBox.richText = false;

                DrawCheckAgainButton();

                if (isUpToDate)
                    return;

                isChangelogFoldOut = EditorGUILayout.Foldout(isChangelogFoldOut, "Release notes", true);
                if (isChangelogFoldOut)
                {
                    using (new EditorGUILayout.HorizontalScope())
                    {
                        GUILayout.Space(10);

                        using (new EditorGUILayout.VerticalScope())
                        {
                            EditorUtilities.DrawScrollableSelectableLabel(
                                ref changelogScrollPosition,
                                ref changelogWidth,
                                updateInfo.changelogPages[changelogPageIndex],
                                new GUIStyle(EditorStyles.textArea)
                                {
                                    richText = true
                                });

                            if (updateInfo.changelogPages.Count > 1)
                            {
                                using (new EditorGUILayout.HorizontalScope())
                                {
                                    using (new EditorGUI.DisabledGroupScope(changelogPageIndex == 0))
                                    {
                                        if (GUILayout.Button("Previous Page"))
                                        {
                                            changelogPageIndex = Math.Max(0, --changelogPageIndex);
                                            changelogScrollPosition = Vector3.zero;
                                        }
                                    }
                                    using (new EditorGUI.DisabledGroupScope(changelogPageIndex == updateInfo.changelogPages.Count - 1))
                                    {
                                        if (GUILayout.Button("Next Page"))
                                        {
                                            changelogPageIndex = Math.Min(updateInfo.changelogPages.Count - 1, ++changelogPageIndex);
                                            changelogScrollPosition = Vector3.zero;
                                        }
                                    }
                                }
                            }

                            if (GUILayout.Button("View on GitHub"))
                                Application.OpenURL(updateInfo.html_url);
                        }
                    }
                }

                GUILayout.FlexibleSpace();

                using (EditorGUI.ChangeCheckScope changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    bool autoCheckValue = EditorPrefs.GetBool(autoCheckKey, true);

                    bool toggleValue = GUILayout.Toggle(autoCheckValue, "Automatically check for updates");

                    if (changeCheckScope.changed)
                    {
                        if (toggleValue)
                            EditorPrefs.DeleteKey(autoCheckKey);
                        else
                            EditorPrefs.SetBool(autoCheckKey, false);
                    }
                }
            }
        }

        private static void DrawCheckAgainButton()
        {
            if (GUILayout.Button("Check again"))
                CheckManually();
        }

        private static void CheckForUpdate()
        {
            if (isManualCheck)
            {
                ShowWindow();
            }
            else
            {
                if (versionChecked || !EditorPrefs.GetBool(autoCheckKey, true))
                {
                    EditorApplication.update -= CheckForUpdate;
                    return;
                }

                if (EditorPrefs.HasKey(lastCheckKey))
                {
                    string lastCheckTicksString = EditorPrefs.GetString(lastCheckKey);
                    DateTime lastCheckDateTime = new DateTime(Convert.ToInt64(lastCheckTicksString));

                    if (lastCheckDateTime.AddHours(checkUpdateHours) >= DateTime.UtcNow)
                    {
                        versionChecked = true;
                        return;
                    }
                }
            }

            versionResource = versionResource ?? new WWW(latestReleaseGitHubURL);
            if (!versionResource.isDone)
                return;

            EditorApplication.update -= CheckForUpdate;

            if (string.IsNullOrEmpty(versionResource.error))
                updateInfo = VersionInfo.FromJSON(versionResource.text);

            versionResource.Dispose();
            versionResource = null;
            versionChecked = true;
            EditorPrefs.SetString(lastCheckKey, DateTime.UtcNow.Ticks.ToString());

            if (!isManualCheck && (updateInfo != null) && (new Version(Integration.Version) >= updateInfo.version))
                return;

            ShowWindow();
            isManualCheck = false;
        }

        private static void ShowWindow()
        {
            if (checkerWindow != null)
            {
                checkerWindow.ShowUtility();
                checkerWindow.Repaint();
                return;
            }

            checkerWindow = GetWindow<IntegrationUpdateChecker>(true);
            checkerWindow.titleContent = new GUIContent("ARVI Integration Update");
        }

        [MenuItem("ARVI/Integration/Check For Update")]
        private static void CheckManually()
        {
            isManualCheck = true;

            if (versionResource == null)
                EditorApplication.update += CheckForUpdate;
        }
    }
}