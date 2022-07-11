using UnityEngine;
using ARVI.SDK;

public class SendingRequestsToPlatformExample : MonoBehaviour
{
    protected virtual void Awake()
    {
        // Initialize integration API
        if (Integration.Initialize())
            Debug.Log(string.Format("ARVI SDK {0} initialized", Integration.Version));
    }

    public virtual void ServerStarted()
    {
        // Notify the platform about the game server launch
        Integration.ServerStarted().OnComplete(
            (response) => {
                if (response.Success)
                    Debug.Log("ServerStarted completed.");
                else
                    Debug.LogWarning(string.Format("ServerStarted failed. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });
    }

    public virtual void GameCompleted()
    {
        // Notify the platform when the game is completed
        Integration.GameCompleted().OnComplete(
            (response) => {
                if (response.Success)
                    Debug.Log("GameCompleted completed.");
                else
                    Debug.LogWarning(string.Format("GameCompleted failed. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });
    }

    public virtual void CallOperator()
    {
        // Notify the platform about the need for the operator's assistance
        Integration.CallOperator().OnComplete(
            (response) => {
                if (response.Success)
                    Debug.Log("CallOperator completed.");
                else
                    Debug.LogWarning(string.Format("CallOperator failed. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });
    }

    public virtual void SetAudioChatChannel()
    {
        // Ask the platform to set the audio chat channel
        Integration.SetAudioChatChannel(AudioChatChannel.Public).OnComplete(
            (response) => {
                if (response.Success)
                    Debug.Log("SetAudioChatChannel completed.");
                else
                    Debug.LogWarning(string.Format("SetAudioChatChannel failed. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });
    }

    public virtual void ActivateInGameCommand()
    {
        // Activate in-game command in operator's panel
        Integration.ActivateInGameCommand("COMMAND_TEST_ACTIVATE").OnComplete(
            (response) => {
                if (response.Success)
                    Debug.Log("Test command activation succeeded.");
                else
                    Debug.LogWarning(string.Format("Test command activation failed. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });
    }

    public virtual void SendGameMessageToPlatform()
    {
        // Send some text message to platform
        Integration.SendGameMessage("Puzzle 1 solved").OnComplete(
            (response) => {
                if (response.Success)
                    Debug.Log("Message send.");
                else
                    Debug.LogWarning(string.Format("Message failed. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });

        // Send grouped text message to platform. It will be grouped in group Level1
        Integration.SendGameMessage("Message about Level1", "Level1").OnComplete(
            (response) => {
                if (response.Success)
                    Debug.Log("Message send.");
                else
                    Debug.LogWarning(string.Format("Message failed. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });
    }

    public virtual void SendLogMessageToPlatform()
    {
        // Send log message to platform. It will be stored only in service log file
        Integration.SendLogMessage("Write this string to log file").OnComplete(
            (response) => {
                if (response.Success)
                    Debug.Log("Log message send.");
                else
                    Debug.LogWarning(string.Format("Log message failed. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });
    }

    public virtual void SendTrackingMessageToPlatform()
    {
        // Send tracking message to platform. It will be used for your events visualization
        Integration.SendTrackingMessage("Some Tracking Event").OnComplete(
            (response) => {
                if (response.Success)
                    Debug.Log("Tracking message send.");
                else
                    Debug.LogWarning(string.Format("Tracking message failed. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });
    }

    public virtual void SetSessionData()
    {
        // Set some session-related data that will be saved throughout the entire gaming session
        Integration.SetSessionData("string_key", "Hello, world!").OnComplete(
            (response) => {
                if (response.Success)
                    Debug.Log("string_key set successfully");
                else
                    Debug.LogWarning(string.Format("Failed to set string_key. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });

        Integration.SetSessionData("int_key", 123).OnComplete(
            (response) => {
                if (response.Success)
                    Debug.Log("int_key set successfully");
                else
                    Debug.LogWarning(string.Format("Failed to set int_key. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });
    }

    public virtual void GetSessionData()
    {
        // Set some session-related data that will be saved throughout the entire gaming session
        string stringValue;
        if (Integration.TryGetSessionData("string_key", out stringValue))
            Debug.Log("string_key: " + stringValue);
        else
            Debug.LogWarning("string_key key not exists");

        int intValue;
        if (Integration.TryGetSessionData("int_key", out intValue))
            Debug.Log("int_key: " + intValue);
        else
            Debug.LogWarning("int_key key not exists");
    }

    public void GetUISettings()
    {
        string stringValue;
        if (Integration.TryGetUISettings("GAMEMODE", out stringValue))
            Debug.Log("GAMEMODE: " + stringValue);
        else
            Debug.LogWarning("GAMEMODE not found in UI Settings");

        int intValue;
        if (Integration.TryGetUISettings("ROUNDDURATION", out intValue))
            Debug.Log("ROUNDDURATION: " + intValue);
        else
            Debug.LogWarning("ROUNDDURATION not found in UI Settings");
    }

    public virtual void SetPlayerName()
    {
        var oldName = Integration.PlayerName;

        Integration.SetPlayerName("Demo Player").OnComplete(
            (response) =>
            {
                if (response.Success)
                    Debug.Log(string.Format("Player name was successfully changed from \"{0}\" to \"{1}\"", oldName, Integration.PlayerName));
                else
                    Debug.LogWarning(string.Format("Failed to set player name. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });
    }

    public virtual void SetPlayerDominantHand()
    {
        var oldDominantHand = Integration.PlayerDominantHand;

        Integration.SetPlayerDominantHand(DominantHand.Right).OnComplete(
            (response) =>
            {
                if (response.Success)
                    Debug.Log(string.Format("Dominant hand was successfully changed from \"{0}\" to \"{1}\"", oldDominantHand, Integration.PlayerDominantHand));
                else
                    Debug.LogWarning(string.Format("Failed to set player dominant hand. Error code: {0}. Error message: {1}", response.Error.Code, response.Error.Message));
            });
    }
}