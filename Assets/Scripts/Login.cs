using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Login : MonoBehaviour
{
    public InputField usernameInput;
    public InputField passwordInput;
    public Button loginButton;
    public Button goToRegisterButton;

    public UserManager userManager;
    void Start()
    {
        loginButton.onClick.AddListener(LoginUser);
        goToRegisterButton.onClick.AddListener(() => SceneManager.LoadScene("Register"));
    }

    void LoginUser()
    {
        userManager.Login(usernameInput.text, passwordInput.text);
    }
}
