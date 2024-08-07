using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using TMPro;
using System;

// Storing response from the server and showing it on the screen after
[Serializable]
public class BotResponse
{
    public string response;
    public string firstPart;
    public string secondPart;
    public string thirdPart;
}

public class ChatConnect : MonoBehaviour
{
    [Serializable]


    public class User
    {
        public int userId;
    }

    public class Room
    {
        public User userObject;
        public int roomId;
        public string RoomContext;
    }

    public class Person_interest
    {
        public User userObject;
        public int counter;
        public string list_interest;
    }

    public Room Room_1;
    public Room Room_2;
    public UserInput inputfield;
    public TextMeshProUGUI botResponseText;
    public TextMeshProUGUI firstPart;
    public TextMeshProUGUI secondPart;
    public TextMeshProUGUI thirdPart;

    User player = new User
    {
        userId = 1
    };

    


    string url = "http://127.0.0.1:5000/send_message";

    void Start()
    {
        Room_1 = new Room {userObject = player, roomId = 1, RoomContext = "World War 2" };
        Room_2 = new Room {userObject = player, roomId = 2, RoomContext = "The Simpsons" };

    

        if (inputfield == null)
        {
            inputfield = FindObjectOfType<UserInput>();
            if (inputfield == null)
            {
                Debug.LogError("UserInput component not found.");
            }
        }
    }

    public void SendMessageToServer(string message)
    {
        if (inputfield != null && !string.IsNullOrEmpty(inputfield.userInput))
        {
            StartCoroutine(SendMessageCoroutine(inputfield.userInput, url));
        }
        else if (!string.IsNullOrEmpty(message))
        {
            StartCoroutine(SendMessageCoroutine(message, url));
        }
        else
        {
            Debug.LogError("No input provided.");
        }
    }

    IEnumerator SendMessageCoroutine(string message, string url)
    {
        Person_interest person = new Person_interest { userObject = player, counter = 0, list_interest = "" };

        string preparedMessage = $"Divide this text into three smaller topics: 1. Name of the topic, 2. Main information, 3. Interesting fact (if there is one). {message}";
        string messageJson = $"{{\"message\": \"{preparedMessage}\"}}";
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
            person.counter += 1;
            string responseJson = request.downloadHandler.text;
            Debug.Log("Response: " + responseJson);
            BotResponse responseObject = JsonUtility.FromJson<BotResponse>(responseJson);

            if (responseObject != null)
            {
                person.list_interest += $"{responseObject.firstPart}";
                string[] parts = responseObject.response.Split(new[] { "1.", "2.", "3." }, StringSplitOptions.None);

                if (parts.Length >= 4)
                {
                    responseObject.firstPart = parts[1].Trim();
                    responseObject.secondPart = parts[2].Trim();
                    responseObject.thirdPart = parts[3].Trim();

                    firstPart.text = responseObject.firstPart;
                    secondPart.text = responseObject.secondPart;
                    thirdPart.text = responseObject.thirdPart;
                }
                else
                {
                    Debug.LogError("Response does not contain the expected parts.");
                }
                Debug.Log(person.counter);
                Debug.Log(person.list_interest);
            }

            inputfield.ClearInputField();
        }
    }
}
