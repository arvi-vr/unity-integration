namespace ARVI.SDK
{
    using UnityEngine;

    public class ResponseHandler : MonoBehaviour
    {
        protected virtual void Awake()
        {
#if UNITY_2023_1_OR_NEWER
            if (FindAnyObjectByType<ResponseHandler>() != this)
#else
            if (FindObjectOfType<ResponseHandler>() != this)
#endif
                Destroy(gameObject);
            else
                DontDestroyOnLoad(gameObject);
        }

        protected virtual void Update()
        {
            Requests.Execute();
        }

        protected virtual void OnApplicationQuit()
        {
#if UNITY_EDITOR
            Requests.FinalizeRequests(false);
#else
            Requests.FinalizeRequests(true);
#endif
            Requests.Clear();
        }
    }
}