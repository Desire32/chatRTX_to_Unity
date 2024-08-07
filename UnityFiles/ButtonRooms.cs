using UnityEngine;
using UnityEngine.UI;

public class ButtonRooms : MonoBehaviour
{
    public ChatConnect chatConnect;
    public Button room1;
    public Button room2;

    void Start()
    {
        if (room1 == null || room2 == null)
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

        room1.onClick.AddListener(() =>
        {
            Debug.Log($"User ID:{chatConnect.Room_1.userObject.userId}, You have entered the Room:{chatConnect.Room_1.roomId}, topic: {chatConnect.Room_1.RoomContext}");
            chatConnect.SendMessageToServer(chatConnect.Room_1.RoomContext);
        });

        room2.onClick.AddListener(() =>
        {
            Debug.Log($"User ID:{chatConnect.Room_2.userObject.userId}, You have entered the Room:{chatConnect.Room_2.roomId}, topic: {chatConnect.Room_2.RoomContext}");
            chatConnect.SendMessageToServer(chatConnect.Room_2.RoomContext);
        });
    }
}
