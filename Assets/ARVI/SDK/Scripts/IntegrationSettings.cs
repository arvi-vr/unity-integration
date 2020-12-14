namespace ARVI.SDK
{
    using UnityEngine;
    using System.IO;

#if UNITY_EDITOR
    [UnityEditor.InitializeOnLoad]
#endif
    public class IntegrationSettings : ScriptableObject
    {
        [SerializeField]
        private string appKey = "";

        /// <value>Application Key, provided with API</value>
        public static string AppKey
        {
            get { return Instance.appKey; }
            set { Instance.appKey = value; }
        }

        private static IntegrationSettings instance;

        /// <value>Singleton instance of IntegrationSettings</value>
        public static IntegrationSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<IntegrationSettings>("ARVIIntegrationSettings");
                    if (instance == null)
                    {
                        instance = CreateInstance<IntegrationSettings>();
#if UNITY_EDITOR
                        string resourcesDirectoryPath = Path.Combine(Application.dataPath, "Resources");
                        if (!Directory.Exists(resourcesDirectoryPath))
                            UnityEditor.AssetDatabase.CreateFolder("Assets", "Resources");

                        string fullPath = Path.Combine(Path.Combine("Assets", "Resources"), "ARVIIntegrationSettings.asset");
                        UnityEditor.AssetDatabase.CreateAsset(instance, fullPath);
#endif
                    }
                }
                return instance;
            }
        }
    }
}