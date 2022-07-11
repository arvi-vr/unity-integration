using UnityEngine;
using ARVI.SDK;

public class EntitlementCheckExample : MonoBehaviour
{
    [SerializeField]
    private bool exitOnFailure = true;

    protected virtual void Awake()
    {
        // Initialize integration API
        if (Integration.Initialize())
            Debug.Log(string.Format("ARVI SDK {0} initialized", Integration.Version));
    }

    protected virtual void Start()
    {
        // Verify application. You can handle result in OnComplete callback
        Integration.IsApplicationEntitled().OnComplete(EntitlementCheckCallback);
    }

    protected virtual void EntitlementCheckCallback(Response response)
    {
        if (response.Success)
        {
            Debug.Log("Entitlement check passed");
        }
        else
        {
            Debug.LogError(string.Format("Entitlement check failed. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));

            if (exitOnFailure)
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                UnityEngine.Application.Quit();
#endif
        }
    }
}