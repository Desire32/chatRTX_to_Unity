using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonInput : MonoBehaviour
{
    private Button button;
    private UserInput inputField;

    void Start()
    {
        button = GetComponent<Button>();

        inputField = FindObjectOfType<UserInput>();

        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        inputField.OnSubmit();
    }
}
