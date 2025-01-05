using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    // �J�ڐ�̃V�[����
    [SerializeField]
    private string nextSceneName;

    void Update()
    {
        // �}�E�X�̍��N���b�N�����o
        if (Input.GetMouseButtonDown(0))
        {
            LoadNextScene();
        }
    }

    // �V�[���J�ڂ��s�����\�b�h
    public void LoadNextScene()
    {
        // �w�肵���V�[���ɑJ��
        SceneManager.LoadScene(nextSceneName);
    }
}