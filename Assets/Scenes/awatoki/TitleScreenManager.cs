using UnityEngine;
using UnityEngine.EventSystems;

public class TitleScreenManager : MonoBehaviour
{
    public GameObject startButton;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(startButton);
    }
}
