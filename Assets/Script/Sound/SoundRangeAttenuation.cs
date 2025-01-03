using UnityEditor.SceneManagement;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundRangeAttenuation : MonoBehaviour
{
    [SerializeField, Tooltip("音が聞こえる距離")]
    private float maxAudibleDistance = 15f;
    private const float offscreenDistance = 10.0f;
    
    private AudioSource audioSource;
    private float masterVolume;

    private Vector2 cameraPosition;
    private bool shouldToPutSound;

    private bool IsRightSide()
    {
        if (cameraPosition.x < transform.position.x) return true;
        else return false;
    }
    // 右なら1、左なら-1
    private int sideSign;
    private float rangeWithCamera;
    private float screenEdgeXPosition;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        masterVolume = SoundManager.instance.GetSEVolume();

        cameraPosition = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        sideSign = IsRightSide() ? 1 : -1;
        rangeWithCamera = Vector2.Distance(cameraPosition, transform.position);
        
        AdjustVolume();
    }

    private void AdjustVolume()
    {
        // カメラの範囲外
        if (rangeWithCamera > maxAudibleDistance)
        {
            shouldToPutSound = false;
            audioSource.volume = 0;
        }
        else shouldToPutSound = true;
        
        if (!shouldToPutSound) return;
        
        // カメラ外かつ音が聞こえる範囲
        if (rangeWithCamera > offscreenDistance　&& rangeWithCamera < maxAudibleDistance)
        {
            audioSource.panStereo = sideSign;
            // 距離に応じてボリュームを設定
            audioSource.volume = Mathf.InverseLerp(maxAudibleDistance, offscreenDistance, rangeWithCamera) * masterVolume;
        }
        // カメラ内のとき
        else if (rangeWithCamera <= offscreenDistance)
        {
            audioSource.volume = masterVolume;
            if (IsRightSide())
            {
                audioSource.panStereo = Mathf.InverseLerp(cameraPosition.x, cameraPosition.x + offscreenDistance, transform.position.x);
            }
            else
            {
                audioSource.panStereo = -Mathf.InverseLerp(cameraPosition.x, cameraPosition.x - offscreenDistance, transform.position.x);
            }
        }
    }
}
