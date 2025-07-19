using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSystem02 : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemechineVirtualCamera; //�������
    [SerializeField] private bool useEdgeScrolling = false; //�Ƿ�ʹ�ñ�Ե����  
    [SerializeField] private bool useDragPan = false; //�Ƿ�ʹ����קƽ��
    [SerializeField] private float fieldOfViewMax = 50;
    [SerializeField] private float fieldOfViewMin = 10;
    [SerializeField] private float followOffsetMax = 50f;
    [SerializeField] private float followOffsetMin = 5f;
    [SerializeField] private float followOffsetMaxY = 50f;
    [SerializeField] private float followOffsetMinY = 5f;

    private bool dragPanMoveActive; //�Ƿ���קƽ��
    private Vector2 lastMousePosition; //�ϴ����λ��
    private float targetFieldOfView = 50;
    private Vector3 followOffset;
    //private float lastClickTime = 0f; // �ϴε��ʱ��
    //private float doubleClickThreshold = 0.3f; // ˫����ʱ������ֵ   

    private void Awake()
    {
        followOffset = cinemechineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        //Debug.Log($"��ʼ followOffset.y: {followOffset.y}, ���Ʒ�Χ: [{followOffsetMinY}, {followOffsetMaxY}]");
    }

    private void Update()
    {
        HandleCameraMovement();

        if (useEdgeScrolling) { 
            HandleCameraMovementEdgeScrolling(); 
        }

        if (useDragPan)
        {
            HandleCameraMovementDragPan();
        }
        
        HandleCameraRotation();

        //HandleCameraZoom_FeildOfView();
        //HandleCameraZoom_MoveForward();
        HandleCameraZoom_LowerY();

        var currentOffset = cinemechineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        //Debug.Log("��ǰ m_FollowOffset: " + currentOffset);

        //HandleDoubleClickFocus(); // ���˫���۽�����
    }

    //ʵ���ƶ� 
    private void HandleCameraMovement()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;
        
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 30f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraMovementEdgeScrolling()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

        if (useEdgeScrolling)
        {
            int edgeScrollSize = 20; //��Ե�����Ĵ�С  

            if (Input.mousePosition.x < edgeScrollSize)
            {
                inputDir.x = -5f;
            }
            if (Input.mousePosition.x > Screen.width - edgeScrollSize)
            {
                inputDir.x = +5f;
            }
            if (Input.mousePosition.y < edgeScrollSize)
            {
                inputDir.z = -5f;
            }
            if (Input.mousePosition.y > Screen.height - edgeScrollSize)
            {
                inputDir.z = +5f;
            }
        }
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 15f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraMovementDragPan()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

        if (Input.GetMouseButtonDown(1))
        {
            dragPanMoveActive = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            dragPanMoveActive = false;
        }

        if (dragPanMoveActive)
        {
            Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition;

            float dragPanSpeed = 1f; //��קƽ���ٶ�
            inputDir.x = mouseMovementDelta.x * dragPanSpeed;
            inputDir.z = mouseMovementDelta.y * dragPanSpeed;

            lastMousePosition = Input.mousePosition;
        }
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 15f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    //ʵ����ת
    private void HandleCameraRotation()
    {
        float rotateDir = 0f;
        if (Input.GetKey(KeyCode.Q)) rotateDir = +0.5f;
        if (Input.GetKey(KeyCode.E)) rotateDir = -0.5f;

        float rotateSpeed = 80f;
        transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
    }

    private void HandleCameraZoom_FeildOfView()
    {
        //Debug.Log(Input.mouseScrollDelta.y);
        //if (Input.mouseScrollDelta.y > 0)
        //{
        //    cinemechineVirtualCamera.m_Lens.FieldOfView = 10;
        //}
        if (Input.mouseScrollDelta.y > 0)
        {
            targetFieldOfView -= 5;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFieldOfView += 5;
        }

        targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMin, fieldOfViewMax);

        float zoomSpeed = 10f;
        cinemechineVirtualCamera.m_Lens.FieldOfView = 
            Mathf.Lerp(cinemechineVirtualCamera.m_Lens.FieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);
        
        //Debug.Log("Current FOV: " + cinemechineVirtualCamera.m_Lens.FieldOfView);
    }

    private void HandleCameraZoom_MoveForward()
    {
        Vector3 zoomDir = followOffset.normalized;

        float zoomAmount = 3f;
        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset -= zoomDir * zoomAmount;
        }

        if(Input.mouseScrollDelta.y < 0)
        {
            followOffset += zoomDir * zoomAmount;
        }

        if (followOffset.magnitude < followOffsetMin)
        {
            followOffset = followOffset.normalized * followOffsetMin;
        }

        if (followOffset.magnitude > followOffsetMax)
        {
            followOffset = followOffset.normalized * followOffsetMax;
        }

        float zoomSpeed = 10f;
        cinemechineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset=
            Vector3.Lerp(cinemechineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
    }

    private void HandleCameraZoom_LowerY()
    {
        //Debug.Log("mouseScrollDelta.y: " + Input.mouseScrollDelta.y);
        float zoomAmount = 10f;
        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset.y -= zoomAmount;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset.y += zoomAmount;
        }
        followOffset.y = Mathf.Clamp(followOffset.y, followOffsetMinY, followOffsetMaxY);      

        float zoomSpeed = 10f;
        cinemechineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            Vector3.Lerp(cinemechineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
    }

    //// �������˫���۽�
    //private void HandleDoubleClickFocus()
    //{
    //    if (Input.GetMouseButtonDown(0)) // ������������
    //    {
    //        float timeSinceLastClick = Time.time - lastClickTime;
    //        if (timeSinceLastClick <= doubleClickThreshold) // �ж��Ƿ�Ϊ˫��
    //        {
    //            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); // �����λ�÷�������
    //            if (Physics.Raycast(ray, out RaycastHit hitInfo)) // ��������Ƿ���ײ������
    //            {
    //                Transform target = hitInfo.transform; // ��ȡ�����������
    //                FocusOnObject(target); // �۽���Ŀ������
    //            }
    //        }
    //        lastClickTime = Time.time; // �����ϴε��ʱ��
    //    }
    //}

    //// ��дĿ������
    //private void FocusOnObject(Transform target)
    //{
    //    // ����Ŀ��λ�úͽǶ�
    //    Vector3 targetPosition = target.position + target.TransformDirection(new Vector3(-5.5f, 2.5f, 4f));
    //    Quaternion targetRotation = Quaternion.Euler(20f, 120f, 0f);

    //    // ƽ���ƶ�����ͷ��Ŀ��λ��
    //    StartCoroutine(SmoothFocus(targetPosition, targetRotation));
    //}

    //// ƽ���ƶ�����ͷ��Э��
    //private IEnumerator SmoothFocus(Vector3 targetPosition, Quaternion targetRotation)
    //{
    //    float duration = 0.5f; // ƽ���ƶ��ĳ���ʱ��
    //    float elapsedTime = 0f;

    //    Vector3 startPosition = transform.position;
    //    Quaternion startRotation = transform.rotation;

    //    while (elapsedTime < duration)
    //    {
    //        transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / duration);
    //        transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
    //        elapsedTime += Time.deltaTime;
    //        yield return null;
    //    }

    //    // ȷ������λ�úͽǶȾ�ȷ
    //    transform.position = targetPosition;
    //    transform.rotation = targetRotation;
    //}

} 