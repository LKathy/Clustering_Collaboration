using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SwitchCamera02 : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject destroyer01Camera;
    public GameObject destroyer02Camera;
    public GameObject destroyer03Camera;
    public GameObject destroyer04Camera;
    public GameObject depotShipCamera;
    public GameObject aircraftCarrierCamera;
    public GameObject submarineCamera;

    public Dropdown chooseBoatDropdown;
    public Button mainSceneCamera;

    private Camera activeCamera;
    private float transitionDuration = 1.0f; // 平滑过渡的持续时间

    private void Start()
    {
        if (chooseBoatDropdown != null)
        {
            chooseBoatDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
        if (mainSceneCamera != null)
        {
            mainSceneCamera.onClick.AddListener(MainCamera);
        }
    }

    private void OnDropdownValueChanged(int index)
    {
        Debug.Log("Dropdown value changed: " + index);

        switch (index)
        {
            case 0:
                StartCoroutine(SmoothTransition(aircraftCarrierCamera));
                break;
            case 1:
                StartCoroutine(SmoothTransition(depotShipCamera));
                break;
            case 2:
                StartCoroutine(SmoothTransition(destroyer01Camera));
                break;
            case 3:
                StartCoroutine(SmoothTransition(destroyer02Camera));
                break;
            case 4:
                StartCoroutine(SmoothTransition(destroyer03Camera));
                break;
            case 5:
                StartCoroutine(SmoothTransition(destroyer04Camera));
                break;
            case 6:
                StartCoroutine(SmoothTransition(submarineCamera));
                break;
            default:
                StartCoroutine(SmoothTransition(mainCamera));
                break;
        }
    }

    public void MainCamera()
    {
        StartCoroutine(SmoothTransition(mainCamera));
    }

    private IEnumerator SmoothTransition(GameObject targetCamera)
    {
        // 初始化激活的摄像机
        activeCamera = mainCamera.GetComponent<Camera>();

        if (activeCamera == null)
        {
            Debug.LogError("Main camera does not have a Camera component!");
            yield break;
        }

        Debug.Log("Starting smooth transition to: " + targetCamera.name+ "+"+mainCamera.name);
        
        //if (targetCamera == null)
        //{
        //    Debug.LogError("Target camera was destroyed after logging!");
        //    yield break;
        //}

        if (activeCamera == null || targetCamera == null)
        {
            Debug.LogError("Target camera is null!");
            yield break;
        }

        Camera targetCamComponent = targetCamera.GetComponent<Camera>();
        if (targetCamComponent == null)
        {
            Debug.LogError("Target camera does not have a Camera component!");
            yield break;
        }

        Vector3 startPosition = activeCamera.transform.position;
        Quaternion startRotation = activeCamera.transform.rotation;

        Vector3 endPosition = targetCamera.transform.position;
        Quaternion endRotation = targetCamera.transform.rotation;

        float elapsedTime = 0.0f;

        // 平滑过渡
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / transitionDuration;

            activeCamera.transform.position = Vector3.Lerp(startPosition, endPosition, t);
            activeCamera.transform.rotation = Quaternion.Lerp(startRotation, endRotation, t);

            yield return null;
        }

        // 确保最终位置和旋转与目标一致
        activeCamera.transform.position = endPosition;
        activeCamera.transform.rotation = endRotation;

        // 激活目标摄像机
        SetActiveCamera(targetCamera);
    }

    private void SetActiveCamera(GameObject targetCamera)
    {
        Debug.Log("Switching to camera: " + targetCamera.name);

        // 禁用所有摄像机
        mainCamera.SetActive(false);
        destroyer01Camera.SetActive(false);
        destroyer02Camera.SetActive(false);
        destroyer03Camera.SetActive(false);
        destroyer04Camera.SetActive(false);
        depotShipCamera.SetActive(false);
        aircraftCarrierCamera.SetActive(false);
        submarineCamera.SetActive(false);

        // 启用目标摄像机
        targetCamera.SetActive(true);
        activeCamera = targetCamera.GetComponent<Camera>();
    }
}