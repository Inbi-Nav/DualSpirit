using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Text;
using UnityEngine.SceneManagement;

public class UserManager : MonoBehaviour
{
    public string backendURL = "http://dam.inspedralbes.cat:27775";

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
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest www = new UnityWebRequest(backendURL + "/users/unity-register", "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        Debug.Log($"Intentando registrar usuario: {username}");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(" Usuario registrado correctamente.");
        }
        else
        {
            Debug.LogError($"Error al registrar: {www.error} | Respuesta: {www.downloadHandler.text}");
        }
    }

    IEnumerator LoginUser(string username, string password)
    {
        string jsonData = JsonUtility.ToJson(new UserData(username, password));
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);

        UnityWebRequest www = new UnityWebRequest(backendURL + "/users/login", "POST");
        www.uploadHandler = new UploadHandlerRaw(bodyRaw);
        www.downloadHandler = new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");

        Debug.Log($"Intentando iniciar sesión: {username}");
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            try
            {
                UserResponse response = JsonUtility.FromJson<UserResponse>(www.downloadHandler.text);
                PlayerPrefs.SetInt("userId", response.userId);
                PlayerPrefs.Save();
                Debug.Log($" Login correcto. userId = {response.userId}");
                SceneManager.LoadScene("SampleScene");
            }
            catch
            {
                Debug.LogError(" Error al procesar respuesta del servidor.");
            }
        }
        else
        {
            Debug.LogError($" Error en login: {www.error} | Respuesta: {www.downloadHandler.text}");
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
