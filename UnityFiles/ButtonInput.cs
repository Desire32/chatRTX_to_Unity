using UnityEngine;
using UnityEngine.UI;

public class ButtonInput : MonoBehaviour
{
    public Button button;
    public UserInput inputField;

    void Start()
    {
        button = GetComponent<Button>();
        inputField = FindObjectOfType<UserInput>();

        if (inputField == null)
        {
            Debug.LogError("UserInput component not found.");
        }

        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        if (inputField != null)
        {
            inputField.OnSubmit();
        }
        else
        {
            Debug.LogError("UserInput component is null in OnButtonClick.");
        }
    }
}
