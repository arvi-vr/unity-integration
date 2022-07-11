using UnityEngine;
using ARVI.SDK;

public class ReceivingMessagesFromPlatformExample : MonoBehaviour
{
    protected virtual void Awake()
    {
        // Initialize integration API
        if (Integration.Initialize())
            Debug.Log(string.Format("ARVI SDK {0} initialized", Integration.Version));
    }

    protected virtual void OnEnable()
    {
        // Subscribe to requests for remaining time
        Integration.TimeLeftRequest += HandleTimeLeftRequest;
        // Subscribe to requests for player position in game
        Integration.PlayerPositionRequest += HandlePlayerPositionRequest;
        // Subscribe to messages from the platform
        Integration.PlatformMessageReceived += HandleMessageFromPlatform;
        // Subscribe to "Player name changed" event
        Integration.PlayerNameChanged += HandlePlayerNameChanged;
        // Subscribe to "Player dominant hand changed" event
        Integration.PlayerDominantHandChanged += HandlePlayerDominantHandChanged;
    }

    protected virtual void OnDisable()
    {
        // Unsubscribe from requests for remaining time
        Integration.TimeLeftRequest -= HandleTimeLeftRequest;
        // Unsubscribe from requests for player position in game
        Integration.PlayerPositionRequest -= HandlePlayerPositionRequest;
        // Unsubscribe from messages from platform
        Integration.PlatformMessageReceived -= HandleMessageFromPlatform;
        // Unsubscribe from "Player name changed" event
        Integration.PlayerNameChanged -= HandlePlayerNameChanged;
        // Unsubscribe from "Player dominant hand changed" event
        Integration.PlayerDominantHandChanged -= HandlePlayerDominantHandChanged;
    }

    protected virtual void HandleTimeLeftRequest(out int seconds)
    {
        // You need to return a value showing how many seconds are left until the end of the game
        seconds = 3600;
    }

    protected void HandlePlayerPositionRequest(out Vector3 playerPosition, out Vector3 playerForward, out Vector3 playerUp)
    {
        // You need to return the player's position in the game, as well as his up and forward vectors
        playerPosition = transform.position;
        playerForward = transform.forward;
        playerUp = transform.up;
    }

    protected virtual void HandleMessageFromPlatform(PlatformMessage message)
    {
        PrintMessage(message);

        var messageName = message.Name.ToLowerInvariant();
        switch (messageName)
        {
            case "simple_message":
                message.SetResponse("OK");
                break;
            case "some_message_with_json_response":
                message.SetResponse("{ \"name\" : \"value\" }", "application/json");
                break;
            case "some_message_with_binary_response":
                message.SetResponse(new byte[] { 65, 82, 86, 73});
                break;
            case "some_message_with_error_response":
                message.SetError("Error description");
                break;
            default:
                return;
        }
    }

    protected virtual void HandlePlayerNameChanged(string name)
    {
        Debug.Log(string.Format("Player's name changed to \"{0}\"", name));
    }

    protected virtual void HandlePlayerDominantHandChanged(DominantHand hand)
    {
        Debug.Log(string.Format("Player's dominant hand changed to \"{0}\"", hand));
    }

    private void PrintMessage(PlatformMessage message)
    {
        // Message method (GET/POST) and name (e.g., "skip")
        string messageInfo = string.Format("{0} message \"{1}\" received.", message.Method, message.Name);
        // Message parameters (if available)
        if (message.Params.Count > 0)
        {
            messageInfo += " Parameters:";
            for (int i = 0; i < message.Params.Count; ++i)
                messageInfo += string.Format(" {0} = {1};", message.Params.GetKey(i), message.Params.Get(i));
        }
        // Message data (if available)
        string messageData = message.GetDataAsString();
        if (!string.IsNullOrEmpty(messageData))
            messageInfo += string.Format(" Data: {0}", messageData);
        // Print message info
        Debug.Log(messageInfo);
    }
}