using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ChatDisplay : MonoBehaviour
{
    public Text consoleDisplay;
    private string filePath = "C:/Visual_Studio_ChatRTX/Chat-With-RTX-python-api/chat_history.txt";

    void Start()
    {
        StartCoroutine(ReadChatFile());
    }

    IEnumerator ReadChatFile()
    {
        while (true)
        {
            yield return new WaitForSeconds(1); // Read file every second
            if (File.Exists(filePath))
            {
                string lastLine = GetLastLine(filePath);
                if (lastLine != null)
                {
                    consoleDisplay.text = lastLine;
                }
            }
        }
    }

    string GetLastLine(string filePath)
    {
        string lastLine = null;
        try
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs))
            {
                while (!sr.EndOfStream)
                {
                    lastLine = sr.ReadLine();
                }
            }
        }
        catch (IOException ex)
        {
            Debug.LogError("Error reading file: " + ex.Message);
        }
        return lastLine;
    }
}
