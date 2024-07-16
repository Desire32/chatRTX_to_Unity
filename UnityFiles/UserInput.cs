using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TMPro;
using System.IO;

public class UserInput : MonoBehaviour
{
    private TMP_InputField inputField;
    private Button submitButton;
    private TextMeshProUGUI responseText;

    void Start()
    {
        inputField = GetComponent<TMP_InputField>();

        if (submitButton != null)
        {
            submitButton.onClick.AddListener(OnSubmit);
        }
    }

    public void OnSubmit()
    {
        string path = "C:/Visual_Studio_ChatRTX/Chat-With-RTX-python-api/chat_history.txt";

        if (inputField != null)
        {
            string userInput = inputField.text;
            Debug.Log($"User input: {userInput}");
            StartCoroutine(SendMessageToServer(userInput));
        }
        else
        {
            Debug.LogError("InputField is not assigned.");
        }
    }

    IEnumerator SendMessageToServer(string message)
    {
        string url = "http://127.0.0.1:5000/send_message"; // Flask server endpoint
        var jsonData = new Dictionary<string, string> { { "message", message } };
        string json = JsonUtility.ToJson(jsonData);

        using (UnityWebRequest request = UnityWebRequest.Put(url, json))
        {
            request.method = UnityWebRequest.kHttpVerbPOST;
            request.uploadHandler = new UploadHandlerRaw(System.Text.Encoding.UTF8.GetBytes(json));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            Debug.Log("Sending request to server...");  // Debug statement
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("Message sent to server, response received.");
                string response = request.downloadHandler.text;
                Debug.Log($"Server response: {response}");  // Debug statement
                if (responseText != null)
                {
                    responseText.text = response; // Display response in a TextMeshProUGUI element
                }
            }
        }
    }


    void ReadResponseFromFile(string path) // Accept path as parameter
    {
        if (File.Exists(path))
        {
            string response = File.ReadAllText(path);
            Debug.Log($"Response from file: {response}");
            if (responseText != null)
            {
                responseText.text = response; // Display response in a TextMeshProUGUI element
            }
            else
            {
                Debug.LogError("Response Text is not assigned.");
            }
        }
        else
        {
            Debug.LogError("Response file not found.");
        }
    }
}
