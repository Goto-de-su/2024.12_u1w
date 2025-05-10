using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string nextSceneName;
    [SerializeField] private InputActionAsset inputActions; // InputActionsをアタッチ

    private InputAction submitAction;
    private InputAction clickAction;

    private void Awake()
    {
        var uiMap = inputActions.FindActionMap("UI");
        submitAction = uiMap.FindAction("Submit"); // "UI" の "Submit"
        clickAction = uiMap.FindAction("Click");   // "UI" の "Click"
    }

    private void OnEnable()
    {
        submitAction.performed += OnActionPerformed;
        clickAction.performed += OnActionPerformed;
        submitAction.Enable();
        clickAction.Enable();
    }

    private void OnDisable()
    {
        submitAction.performed -= OnActionPerformed;
        clickAction.performed -= OnActionPerformed;
        submitAction.Disable();
        clickAction.Disable();
    }

    private void OnActionPerformed(InputAction.CallbackContext context)
    {
        LoadNextScene();
    }

    public void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
