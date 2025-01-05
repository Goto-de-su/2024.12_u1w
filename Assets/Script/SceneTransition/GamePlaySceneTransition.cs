using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlaySceneTransition : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName = "NextScene"; // �C���X�y�N�^�[�Őݒ肷�鎟�̃V�[����

    [SerializeField]
    private string playerTag = "Player"; // �v���C���[�̃^�O

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // �Փ˂����I�u�W�F�N�g���v���C���[���ǂ������m�F
        if (collision.gameObject.CompareTag(playerTag))
        {
            LoadNextScene();
        }
    }

    private void LoadNextScene()
    {
        // ���̃V�[�������[�h
        SceneManager.LoadScene(nextSceneName);
    }
}