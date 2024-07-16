using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;

public class ChatConnect : MonoBehaviour
{
    IEnumerator Start()
    {
        string url = "http://127.0.0.1:5000/send_message"; 
        string messageJson = "{\"message\": \"Hello, World!\"}"; 
        UnityWebRequest request = new UnityWebRequest(url, "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(messageJson);
        request.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(request.error);
        }
        else
        {
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }
}