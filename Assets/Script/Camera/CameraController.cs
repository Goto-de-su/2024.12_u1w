using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform player;  // �v���C���[��Transform
    private Vector3 offset;                     // �J�����̏����I�t�Z�b�g

    private void Start()
    {
        // �J�����̏����ʒu����v���C���[�܂ł̋�����ۑ�
        offset = transform.position - player.position;
    }

    private void LateUpdate()
    {
        // �v���C���[�̈ʒu�ɃI�t�Z�b�g���������ʒu�ɃJ�������ړ�
        transform.position = new Vector3(
            player.position.x + offset.x,
            player.position.y + offset.y,
            offset.z  // Z���͌Œ�
        );
    }
}
