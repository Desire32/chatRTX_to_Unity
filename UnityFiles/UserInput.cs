using UnityEngine;
using TMPro;

public class UserInput : MonoBehaviour
{
    public TMP_InputField inputField;

    public string userInput = "";
    public ChatConnect chatConnect;

    void Start()
    {
        if (inputField == null)
        {
            Debug.LogError("InputField is not assigned.");
        }

        if (chatConnect == null)
        {
            chatConnect = FindObjectOfType<ChatConnect>();
            if (chatConnect == null)
            {
                Debug.LogError("ChatConnect component not found.");
            }
        }
    }

    public void OnSubmit()
    {
        if (inputField == null)
        {
            Debug.LogError("InputField is null in OnSubmit.");
            return;
        }

        userInput = inputField.text;
        Debug.Log($"User: {userInput}");

        if (chatConnect != null)
        {
            chatConnect.SendMessageToServer(userInput);
        }
        else
        {
            Debug.LogError("ChatConnect is not assigned in OnSubmit.");
        }
    }

    public void ClearInputField()
    {
        if (inputField != null)
        {
            inputField.text = "";
            userInput = "";
        }
        else
        {
            Debug.LogError("InputField is null in ClearInputField.");
        }
    }
}
