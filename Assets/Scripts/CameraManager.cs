using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.Mathematics;
using UnityEngine;


public class CameraManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera[] allVirtualCamera;

    private CinemachineVirtualCamera currentCamera;
    private CinemachineFramingTransposer framingTransposer;

    [Header("Y damping setting for player jump/fall")]
    [SerializeField] private float panAmount = 0.1f;
    [SerializeField] private float panTime = 0.2f;
    public float playerFallSpeedTheshold = -10;
    public bool isLerpingYDamping;
    public bool hasLerpedYDamping;
    private float normalYDamp;


    public static CameraManager Instance { get; private set; }
    private void Awake()
    {
        if(Instance == null)
        {
            Instance =this;
        }

        for(int i = 0;i< allVirtualCamera.Length; i++)
        {
            if (allVirtualCamera[i].enabled)
            {
                currentCamera = allVirtualCamera[i];
                framingTransposer = currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }
        normalYDamp = framingTransposer.m_YDamping;
    }
    private void Start()
    {
        for(int i = 0;i< allVirtualCamera.Length; i++)
        {
            allVirtualCamera[i].Follow = Player.Instance.transform;
        }
    }
    public void SwapCamera(CinemachineVirtualCamera _camera)
    {
        currentCamera.enabled = false;
        currentCamera = _camera;
        currentCamera.enabled = true;
    }
    public IEnumerator LerpYDamping( bool _isPlayerFalling)
    {
        isLerpingYDamping = true;

        float _startYDamp = framingTransposer.m_YDamping;
        float _endYDamp = 0;
        if (_isPlayerFalling)
        {
            _endYDamp = panAmount;
            hasLerpedYDamping = true;
        }
        else
        {
            _endYDamp = normalYDamp;
        }

        float _timer = 0;
        while(_timer< panTime)
        {
            _timer += Time.deltaTime;
            float _lerpedPanAmount = math.lerp(_startYDamp, _endYDamp, (_timer / panTime));
            framingTransposer.m_YDamping = _lerpedPanAmount;
            yield return null;
        }
        isLerpingYDamping= false;
    }
}
