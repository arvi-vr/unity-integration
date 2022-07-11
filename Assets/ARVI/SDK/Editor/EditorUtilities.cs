namespace ARVI.SDK
{
    using System;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using UnityEditor;

    public static class EditorUtilities
    {
        public static bool TryGetEditorMainWindow(out Rect window)
        {
#if UNITY_2020_1_OR_NEWER
            window = EditorGUIUtility.GetMainWindowPosition();
            return true;
#else
            window = Rect.zero;
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var containerWinType = assemblies.SelectMany(assembly => assembly.GetTypes()).FirstOrDefault(type => type.IsSubclassOf(typeof(ScriptableObject)) && type.Name == "ContainerWindow");
            if (containerWinType == null)
                return false;
            var showModeField = containerWinType.GetField("m_ShowMode", BindingFlags.NonPublic | BindingFlags.Instance);
            if (showModeField == null)
                return false;
            var positionProperty = containerWinType.GetProperty("position", BindingFlags.Public | BindingFlags.Instance);
            if (positionProperty == null)
                return false;
            var windows = Resources.FindObjectsOfTypeAll(containerWinType);
            foreach (var win in windows)
            {
                try
                {
                    var showmode = (int)showModeField.GetValue(win);
                    if (showmode == 4) // main window
                    {
                        window = (Rect)positionProperty.GetValue(win, null);
                        return true;
                    }
                }
                catch { }
            }
            return false;
#endif
        }

        public static void CenterOnMainWindow(this EditorWindow window)
        {
            Rect mainWindow;
            if (!TryGetEditorMainWindow(out mainWindow))
                mainWindow = new Rect(0, 0, Screen.currentResolution.width, Screen.currentResolution.height);
            var position = window.position;
            position.center = mainWindow.center;
            window.position = position;
        }

        public static TooltipAttribute GetTooltipAttribute(FieldInfo fieldInfo)
        {
            return (TooltipAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(TooltipAttribute));
        }

        public static GUIContent BuildGUIContent<T>(string fieldName, string displayOverride = null)
        {
            string displayName = (displayOverride != null ? displayOverride : ObjectNames.NicifyVariableName(fieldName));
            FieldInfo fieldInfo = typeof(T).GetField(fieldName);
            TooltipAttribute tooltipAttribute = GetTooltipAttribute(fieldInfo);
            return (tooltipAttribute == null ? new GUIContent(displayName) : new GUIContent(displayName, tooltipAttribute.tooltip));
        }

        public static void AddHeader<T>(string fieldName, string displayOverride = null)
        {
            string displayName = (displayOverride != null ? displayOverride : ObjectNames.NicifyVariableName(fieldName));
            FieldInfo fieldInfo = typeof(T).GetField(fieldName);
            HeaderAttribute headerAttribute = (HeaderAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(HeaderAttribute));
            AddHeader(headerAttribute == null ? displayName : headerAttribute.header);
        }

        public static void AddHeader(string header, bool spaceBeforeHeader = true)
        {
            if (spaceBeforeHeader)
            {
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField(header, EditorStyles.boldLabel);
        }

        public static void DrawUsingDestructiveStyle(GUIStyle styleToCopy, Action<GUIStyle> drawAction)
        {
            Color previousBackgroundColor = GUI.backgroundColor;
            GUIStyle destructiveButtonStyle = new GUIStyle(styleToCopy)
            {
                normal =
                {
                    textColor = Color.white
                },
                active =
                {
                    textColor = Color.white
                }
            };

            GUI.backgroundColor = Color.red;
            drawAction(destructiveButtonStyle);
            GUI.backgroundColor = previousBackgroundColor;
        }

        public static void DrawScrollableSelectableLabel(ref Vector2 scrollPosition, ref float width, string text, GUIStyle style)
        {
            using (EditorGUILayout.ScrollViewScope scrollViewScope = new EditorGUILayout.ScrollViewScope(scrollPosition))
            {
                scrollPosition = scrollViewScope.scrollPosition;

                float textHeight = style.CalcHeight(new GUIContent(text), width);
                EditorGUILayout.SelectableLabel(text, style, GUILayout.MinHeight(textHeight));

                if (Event.current.type == EventType.Repaint)
                {
                    width = GUILayoutUtility.GetLastRect().width;
                }
            }
        }
    }
}