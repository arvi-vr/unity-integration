namespace ARVI.SDK
{
    using System;
#if !UNITY_2020_1_OR_NEWER
    using System.Linq;
#endif
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
                    var showMode = (int)showModeField.GetValue(win);
                    if (showMode == 4) // main window
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
    }
}