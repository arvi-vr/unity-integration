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
}