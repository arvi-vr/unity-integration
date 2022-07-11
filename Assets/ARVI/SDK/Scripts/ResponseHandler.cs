namespace ARVI.SDK
{
    using UnityEngine;

    public class ResponseHandler : MonoBehaviour
    {
        protected virtual void Awake()
        {
            if (FindObjectOfType<ResponseHandler>() != this)
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