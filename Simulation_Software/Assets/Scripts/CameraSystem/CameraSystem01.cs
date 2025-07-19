using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraSystem01 : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera cinemechineVirtualCamera; //虚拟相机
    [SerializeField] private bool useEdgeScrolling = false; //是否使用边缘滚动  
    [SerializeField] private bool useDragPan = false; //是否使用拖拽平移
    [SerializeField] private float fieldOfViewMax = 50;
    [SerializeField] private float fieldOfViewMin = 10;
    [SerializeField] private float followOffsetMax = 50f;
    [SerializeField] private float followOffsetMin = 5f;
    [SerializeField] private float followOffsetMaxY = 50f;
    [SerializeField] private float followOffsetMinY = 5f;

    private bool dragPanMoveActive; //是否拖拽平移
    private Vector2 lastMousePosition; //上次鼠标位置
    private float targetFieldOfView = 50;
    private Vector3 followOffset;

    private void Awake()
    {
        followOffset = cinemechineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        //Debug.Log($"初始 followOffset.y: {followOffset.y}, 限制范围: [{followOffsetMinY}, {followOffsetMaxY}]");
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
        //Debug.Log("当前 m_FollowOffset: " + currentOffset);
    }

    //实现移动 
    private void HandleCameraMovement()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) inputDir.z = +0.5f;
        if (Input.GetKey(KeyCode.S)) inputDir.z = -0.5f;
        if (Input.GetKey(KeyCode.A)) inputDir.x = -0.5f;
        if (Input.GetKey(KeyCode.D)) inputDir.x = +0.5f;
        
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 10f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraMovementEdgeScrolling()
    {
        Vector3 inputDir = new Vector3(0, 0, 0);

        if (useEdgeScrolling)
        {
            int edgeScrollSize = 20; //边缘滚动的大小  

            if (Input.mousePosition.x < edgeScrollSize)
            {
                inputDir.x = -1f;
            }
            if (Input.mousePosition.x > Screen.width - edgeScrollSize)
            {
                inputDir.x = +1f;
            }
            if (Input.mousePosition.y < edgeScrollSize)
            {
                inputDir.z = -1f;
            }
            if (Input.mousePosition.y > Screen.height - edgeScrollSize)
            {
                inputDir.z = +1f;
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

            float dragPanSpeed = 0.4f; //拖拽平移速度
            inputDir.x = mouseMovementDelta.x * dragPanSpeed;
            inputDir.z = mouseMovementDelta.y * dragPanSpeed;

            lastMousePosition = Input.mousePosition;
        }
        Vector3 moveDir = transform.forward * inputDir.z + transform.right * inputDir.x;
        float moveSpeed = 15f;
        transform.position += moveDir * moveSpeed * Time.deltaTime;
    }

    //实现旋转
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
        float zoomAmount = 1f;
        if (Input.mouseScrollDelta.y > 0)
        {
            followOffset.y -= zoomAmount;
        }

        if (Input.mouseScrollDelta.y < 0)
        {
            followOffset.y += zoomAmount;
        }
        followOffset.y = Mathf.Clamp(followOffset.y, followOffsetMinY, followOffsetMaxY);      

        float zoomSpeed = 5f;
        cinemechineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
            Vector3.Lerp(cinemechineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
    }

} 