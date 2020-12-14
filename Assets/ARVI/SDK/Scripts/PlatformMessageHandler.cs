namespace ARVI.SDK
{
    using UnityEngine;

    public class PlatformMessageHandler : MonoBehaviour
    {
        protected virtual void Awake()
        {
            if (FindObjectOfType<PlatformMessageHandler>() != this)
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