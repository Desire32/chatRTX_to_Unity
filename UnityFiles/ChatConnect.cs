using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using System;

public class ChatConnect : MonoBehaviour
{
    public UserInput inputfield;

    void Start()
    {
        if (inputfield == null)
        {
            Debug.LogError("Inputfield is not assigned.");
        }
    }

    public void SendMessageToServer()
    {
        if (inputfield != null && !string.IsNullOrEmpty(inputfield.userInput))
        {
            StartCoroutine(SendMessageCoroutine(inputfield.userInput));
        }
        else
        {
            Debug.LogError("No message to send or inputfield is null.");
        }
    }

    IEnumerator SendMessageCoroutine(string message)
    {
        string url = "http://127.0.0.1:5000/send_message";
        string messageJson = $"{{\"message\": \"{message}\"}}";
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(messageJson);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
            inputfield.ClearInputField();
        }
    }
}
