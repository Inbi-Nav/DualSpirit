using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button registerButton;
    public Button goToLoginButton;

    public UserManager userManager; 

    void Start()
    {
        registerButton.onClick.AddListener(RegisterUser);
        goToLoginButton.onClick.AddListener(() => SceneManager.LoadScene("Login"));
    }

    void RegisterUser()
    {
        userManager.Register(usernameInput.text, passwordInput.text);
    }
}
