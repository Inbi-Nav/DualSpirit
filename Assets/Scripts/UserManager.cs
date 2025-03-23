using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.SceneManagement;

public class UserManager : MonoBehaviour
{
    public string backendURL = "http://192.168.1.135:3000"; // ✅ Dirección correcta

    public void Register(string username, string password)
    {
        StartCoroutine(RegisterUser(username, password));
    }

    public void Login(string username, string password)
    {
        StartCoroutine(LoginUser(username, password));
    }

    IEnumerator RegisterUser(string username, string password)
    {
        string jsonData = JsonUtility.ToJson(new UserData(username, password));
        UnityWebRequest www = new UnityWebRequest(backendURL + "/users/unity-register", "POST"); // 👈 Nueva ruta

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ User registered: " + www.downloadHandler.text);
        }
        else
        {
            Debug.Log("❌ Register Error: " + www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }

    IEnumerator LoginUser(string username, string password)
    {
        string jsonData = JsonUtility.ToJson(new UserData(username, password));
        UnityWebRequest www = new UnityWebRequest(backendURL + "/users/login", "POST");

        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("✅ Login success: " + www.downloadHandler.text);

            UserResponse response = JsonUtility.FromJson<UserResponse>(www.downloadHandler.text);
            PlayerPrefs.SetInt("userId", response.userId);
            PlayerPrefs.Save();

            Debug.Log("📦 userId saved: " + response.userId);
            SceneManager.LoadScene("SampleScene"); // ✅ Redirección tras login
        }
        else
        {
            Debug.Log("❌ Login failed: " + www.error);
            Debug.Log(www.downloadHandler.text);
        }
    }

    [System.Serializable]
    public class UserData
    {
        public string username;
        public string password;

        public UserData(string user, string pass)
        {
            username = user;
            password = pass;
        }
    }

    [System.Serializable]
    public class UserResponse
    {
        public string message;
        public int userId;
    }
}
