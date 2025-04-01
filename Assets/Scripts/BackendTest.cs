using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class BackendTest : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TestConnection());
    }

    IEnumerator TestConnection()
    {
        string url = "http://dam.inspedralbes.cat:27775/health";
        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Unity puede conectarse al backend: " + request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("❌ Unity NO puede conectarse al backend: " + request.error);
        }
    }
}
