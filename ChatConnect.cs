using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;
using System;

public class ChatConnect : MonoBehaviour
{
    public class BotResponse
    {
        public string response;
        public string firstPart, secondPart, thirdPart;
    }

    public class Person_interest
    {
        public int userId;
        public int counter;
        public string list_interest;
    }

    public class Room
    {
        public Person_interest user;
        public int roomId;
        public string RoomContext;
    }

    public Room Room_1, Room_2;
    public Person_interest person;
    public UserInput inputfield;
    public TextMeshProUGUI firstPart, secondPart, thirdPart, botResponseText, finalResponse;

    string url = "http://127.0.0.1:5000/send_message";

    void Start()
    {
        person = new Person_interest { userId = 1, counter = 0, list_interest = "" };
        Room_1 = new Room { user = person, roomId = 1, RoomContext = "World War 2" };
        Room_2 = new Room { user = person, roomId = 2, RoomContext = "The Simpsons" };

        if (inputfield == null)
        {
            Debug.LogError("UserInput not found.");
        }
    }

    public void SendMessageToServer(string message, bool isFinalMessage = false)
    {
        string preparedMessage = isFinalMessage
            ? $"Summarize this string and tell, in what topic person might be interested: {person.list_interest}"
            : $"Divide this text into three smaller topics: 1. Name of the topic, 2. Main information, 3. Interesting fact (if there is one). {message}";

        if ((inputfield != null && !string.IsNullOrEmpty(inputfield.userInput)) || !string.IsNullOrEmpty(message))
        {
            StartCoroutine(SendRequest(preparedMessage, url));
        }
        else
        {
            Debug.LogError("No input provided.");
        }
    }

    IEnumerator SendRequest(string details, string url)
    {
        string messageJson = $"{{\"message\": \"{details}\"}}";
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
            HandleResponse(request.downloadHandler.text);
        }
    }

    void HandleResponse(string responseJson)
    {
        Debug.Log("Response: " + responseJson);
        BotResponse responseObject = JsonUtility.FromJson<BotResponse>(responseJson);

        if (responseObject != null)
        {
            string[] parts = responseObject.response.Split(new[] { "1.", "2.", "3." }, StringSplitOptions.None);

            if (parts.Length >= 4)
            {
                responseObject.firstPart = parts[1].Trim();
                responseObject.secondPart = parts[2].Trim();
                responseObject.thirdPart = parts[3].Trim();

                firstPart.text = responseObject.firstPart;
                secondPart.text = responseObject.secondPart;
                thirdPart.text = responseObject.thirdPart;

                person.counter += 1;

                string[] splitParts = responseObject.firstPart.Split(new[] { ": " }, StringSplitOptions.None);
                string topic = splitParts.Length > 1 ? splitParts[1].Trim() : string.Empty;
                person.list_interest += topic;
                if (!string.IsNullOrEmpty(person.list_interest))
                {
                    person.list_interest += ", ";
                }

                Debug.Log(person.counter);
                Debug.Log(person.list_interest);
            }
            else
            {
                finalResponse.text = responseObject.response;
            }
        }

        inputfield.ClearInputField();
    }
}