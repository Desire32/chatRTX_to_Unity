// libs
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;
using System;

// storing response from the server and showing it on the screen after
[Serializable]
public class BotResponse
{
    // we use this name, because in the json we catch it says "response", that's why its the only name variable which works
    public string response;
}


public class ChatConnect : MonoBehaviour
{
    public UserInput inputfield;
    public TextMeshProUGUI botResponseText;
    public TextMeshProUGUI botSortedText;

    void Start()
    {
        if (inputfield == null)
        {
            inputfield = FindObjectOfType<UserInput>();
            if (inputfield == null)
            {
                Debug.LogError("UserInput component not found.");
            }
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
    // Send the initial message to the server       
    string url = "http://127.0.0.1:5000/send_message";
    string messageJson = $"{{\"message\": \"{message}\"}}";
    UnityWebRequest request = new UnityWebRequest(url, "POST");
    byte[] bodyRaw = Encoding.UTF8.GetBytes(messageJson);
    request.uploadHandler = new UploadHandlerRaw(bodyRaw);
    request.downloadHandler = new DownloadHandlerBuffer();
    request.SetRequestHeader("Content-Type", "application/json");

    yield return request.SendWebRequest();

    if (request.result != UnityWebRequest.Result.Success)
    {
        Debug.Log(request.error);
    }
    else
    {
        // Handle the response from the server
        Debug.Log("Response: " + request.downloadHandler.text);
        string responseJson = request.downloadHandler.text;
        BotResponse responseObject = JsonUtility.FromJson<BotResponse>(responseJson);

        // If the response is valid, prepare a new message
        if (responseObject != null)
        {
            string preparedMessage = $"Divide this text into three smaller topics: {responseObject.response}";
            string preloadedJSON = $"{{\"message\": \"{preparedMessage}\"}}";
            UnityWebRequest preparedRequest = new UnityWebRequest(url, "POST");
            byte[] bodyFresh = Encoding.UTF8.GetBytes(preloadedJSON);
            preparedRequest.uploadHandler = new UploadHandlerRaw(bodyFresh);
            preparedRequest.downloadHandler = new DownloadHandlerBuffer();
            preparedRequest.SetRequestHeader("Content-Type", "application/json");

            yield return preparedRequest.SendWebRequest();

            if (preparedRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(preparedRequest.error);
            }
            else
            {
                Debug.Log("Sorted Response: " + preparedRequest.downloadHandler.text);
                BotResponse sortedResponseObject = JsonUtility.FromJson<BotResponse>(preparedRequest.downloadHandler.text);

                // Update the UI with the sorted response
                if (botResponseText != null)
                {
                        botResponseText.text = responseObject.response;
                }
                // Second UI Unity object
                if(botSortedText != null)
                    {
                        botSortedText.text = sortedResponseObject.response;
                    }

                else
                {
                    Debug.LogError("BotResponseText is not assigned.");
                }
            }
        }

        // Clear the input field
        inputfield.ClearInputField();
    }
}
}
