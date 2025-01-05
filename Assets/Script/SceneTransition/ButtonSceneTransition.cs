using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonSceneTransition : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName;  // �C���X�y�N�^�[�Őݒ肷��J�ڐ�V�[����

    [SerializeField]
    private Button transitionButton;  // �{�^���ւ̎Q��

    void Start()
    {
        // �{�^���R���|�[�l���g�̎擾�i�A�^�b�`����Ă��Ȃ��ꍇ�j
        if (transitionButton == null)
        {
            transitionButton = GetComponent<Button>();
        }

        // �{�^���ɃN���b�N�C�x���g��ǉ�
        transitionButton.onClick.AddListener(LoadNextScene);
    }

    // �V�[���J�ڂ��s�����\�b�h
    public void LoadNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("�J�ڐ�̃V�[�������ݒ肳��Ă��܂���");
            return;
        }

        // �V�[���J��
        SceneManager.LoadScene(nextSceneName);
    }

    void OnDestroy()
    {
        // �N���[���A�b�v
        if (transitionButton != null)
        {
            transitionButton.onClick.RemoveListener(LoadNextScene);
        }
    }
}