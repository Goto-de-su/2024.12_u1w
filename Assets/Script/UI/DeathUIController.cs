using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class DeathUIController : MonoBehaviour
{
    [SerializeField] private GameObject deathButtonsContainer;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button titleButton;

    public UnityEvent onRetrySelected = new UnityEvent();
    public UnityEvent onTitleSelected = new UnityEvent();

    private PlayerStateManager stateManager;

    private void Awake()
    {
        stateManager = FindObjectOfType<PlayerStateManager>();

        // ������Ԃł͔�\��
        deathButtonsContainer.SetActive(false);

        // �{�^���C�x���g�̐ݒ�
        retryButton.onClick.AddListener(OnRetryButtonClicked);
        titleButton.onClick.AddListener(OnTitleButtonClicked);

        // ��ԕύX�C�x���g�̍w��
        if (stateManager != null)
        {
            stateManager.onStateChanged.AddListener(OnPlayerStateChanged);
        }
    }

    private void OnPlayerStateChanged(PlayerStateManager.PlayerState oldState,
                                    PlayerStateManager.PlayerState newState,
                                    PlayerStateManager.StateChangeContext context)
    {
        if (newState == PlayerStateManager.PlayerState.Dead)
        {
            ShowDeathUI();
        }
    }

    private void ShowDeathUI()
    {
        deathButtonsContainer.SetActive(true);
        retryButton.interactable = true;
        titleButton.interactable = true;
    }

    private void OnRetryButtonClicked()
    {
        onRetrySelected.Invoke();
    }

    private void OnTitleButtonClicked()
    {
        onTitleSelected.Invoke();
    }

    private void OnDestroy()
    {
        if (stateManager != null)
        {
            stateManager.onStateChanged.RemoveListener(OnPlayerStateChanged);
        }
    }
}