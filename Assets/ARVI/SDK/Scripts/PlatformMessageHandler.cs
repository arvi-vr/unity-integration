namespace ARVI.SDK
{
    using UnityEngine;

    public class PlatformMessageHandler : MonoBehaviour
    {
        protected virtual void Awake()
        {
#if UNITY_2023_1_OR_NEWER
            if (FindAnyObjectByType<PlatformMessageHandler>() != this)
#else
            if (FindObjectOfType<PlatformMessageHandler>() != this)
#endif
                Destroy(gameObject);
            else
                DontDestroyOnLoad(gameObject);
        }

        protected virtual void Update()
        {
            PlatformMessages.Execute();
        }

        protected virtual void OnApplicationQuit()
        {
            PlatformMessages.FinalizeMessages();
        }
    }
}