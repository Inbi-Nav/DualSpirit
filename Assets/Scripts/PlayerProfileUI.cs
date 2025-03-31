using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class PlayerProfileUI : MonoBehaviour
{
    public TMP_Text usernameText;
    public TMP_Text rankText;

    void Start()
    {
        int userId = PlayerPrefs.GetInt("userId", -1);
        if (userId != -1)
        {
            StartCoroutine(CargarDatosJugador(userId));
        }
        else
        {
            usernameText.text = "Usuario no válido";
            rankText.text = "";
        }
    }

    IEnumerator CargarDatosJugador(int id)
    {
        string url = $"http://localhost:3000/users/{id}";
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            string json = www.downloadHandler.text;
            UserData jugador = JsonUtility.FromJson<UserData>(json);

            if (jugador != null)
            {
                usernameText.text = jugador.username;
                rankText.text = ObtenerRango(jugador.gamesPlayed);
            }
            else
            {
                usernameText.text = "Usuario eliminado";
                rankText.text = "";
            }
        }
        else
        {
            usernameText.text = "Error al cargar perfil";
            rankText.text = "";
        }
    }

    string ObtenerRango(int gamesPlayed)
    {
        if (gamesPlayed <= 2)
            return "Novato";
        else if (gamesPlayed <= 5)
            return "Jugador Activo";
        else
            return "Leyenda";
    }

    public void VolverAlLogin()
    {
        SceneManager.LoadScene("Login");
    }
}

[System.Serializable]
public class UserData
{
    public int id;
    public string username;
    public int gamesPlayed;
}
