using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Drawing;

public class ButtonRooms : MonoBehaviour
{
    public ChatConnect chatConnect;
    public Button room1;
    public Button room2;
    public Button finish_button;
    public TextMeshProUGUI lastResponse;
    public TextMeshProUGUI room_description;

    void Start()
    {
        if (room1 == null || room2 == null || finish_button == null)
        {
            Debug.LogError("Room buttons not assigned.");
        }

        if (chatConnect == null)
        {
            Debug.LogError("ChatConnect component not assigned.");
        }

        // in order not to repeat
        room1.onClick.RemoveAllListeners();
        room2.onClick.RemoveAllListeners();
        finish_button.onClick.RemoveAllListeners();

        room1.onClick.AddListener(() =>
        {
            room_description.text = $"User ID:{chatConnect.Room_1.user.userId}, You have entered the Room:{chatConnect.Room_1.roomId}, topic: {chatConnect.Room_1.RoomContext}";
            chatConnect.SendMessageToServer(chatConnect.Room_1.RoomContext);
        });

        room2.onClick.AddListener(() =>
        {
            room_description.text = $"User ID:{chatConnect.Room_2.user.userId}, You have entered the Room:{chatConnect.Room_2.roomId}, topic: {chatConnect.Room_2.RoomContext}";
            chatConnect.SendMessageToServer(chatConnect.Room_2.RoomContext);
        });

        finish_button.onClick.AddListener(() =>
        {
            string final = $"Analyze the user's list of selected topics and, using Internet resources, select one topic that might be of interest to him: {chatConnect.person.list_interest}";
            lastResponse.text = $"Amount of prompts: {chatConnect.person.counter}, Topics: {chatConnect.person.list_interest}";
            chatConnect.SendMessageToServer(final, true);
        });
    }
}