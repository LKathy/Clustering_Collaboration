using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SwitchCamera01 : MonoBehaviour
{
    public GameObject mainCamera;
    public GameObject destroyer01Camera;
    public GameObject destroyer02Camera;
    public GameObject destroyer03Camera;
    public GameObject destroyer04Camera;
    public GameObject depotShipCamera;
    public GameObject aircraftCarrierCamera;
    public GameObject submarineCamera;
    public GameObject missileCamera;

    public Dropdown chooseBoatDropdown;
    public Button mainSceneCamera;
    public Button missileSceneCamera;

    private void Start()
    {
        // 绑定下拉菜单的值变化事件
        if (chooseBoatDropdown != null)
        {
            chooseBoatDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
        }
        // 绑定按钮的点击事件
        if (mainSceneCamera != null)
        {
            mainSceneCamera.onClick.AddListener(MainCamera);
        }
        if (missileSceneCamera != null)
        {
            missileSceneCamera.onClick.AddListener(MissileCamera);
        }

    }

    private void OnDropdownValueChanged(int index)
    {
        // 根据下拉菜单的选项索引切换摄像头
        switch (index)
        {
            case 0:
                AircraftCarrierCamera();
                break;
            case 1:
                DepotShipCamera();
                break;
            case 2:
                Destroyer01Camera();
                break;
            case 3:
                Destroyer02Camera();
                break;
            case 4:
                Destroyer03Camera();
                break;
            case 5:
                Destroyer04Camera();
                break;
            case 6:
                SubmarineCamera();
                break;
            default:
                MainCamera();
                break;
        }
    }
    public void MainCamera()
    {
        mainCamera.SetActive(true);
        destroyer01Camera.SetActive(false);
        destroyer02Camera.SetActive(false);
        destroyer03Camera.SetActive(false);
        destroyer04Camera.SetActive(false);
        depotShipCamera.SetActive(false);
        aircraftCarrierCamera.SetActive(false);
        submarineCamera.SetActive(false);
        missileCamera.SetActive(false);
    }
    public void Destroyer01Camera()
    {
        mainCamera.SetActive(false);
        destroyer01Camera.SetActive(true);
        destroyer02Camera.SetActive(false);
        destroyer03Camera.SetActive(false);
        destroyer04Camera.SetActive(false);
        depotShipCamera.SetActive(false);
        aircraftCarrierCamera.SetActive(false);
        submarineCamera.SetActive(false);
        missileCamera.SetActive(false);
    }
    public void Destroyer02Camera()
    {
        mainCamera.SetActive(false);
        destroyer01Camera.SetActive(false);
        destroyer02Camera.SetActive(true);
        destroyer03Camera.SetActive(false);
        destroyer04Camera.SetActive(false);
        depotShipCamera.SetActive(false);
        aircraftCarrierCamera.SetActive(false);
        submarineCamera.SetActive(false);
        missileCamera.SetActive(false);
    }
    public void Destroyer03Camera()
    {
        mainCamera.SetActive(false);
        destroyer01Camera.SetActive(false);
        destroyer02Camera.SetActive(false);
        destroyer03Camera.SetActive(true);
        destroyer04Camera.SetActive(false);
        depotShipCamera.SetActive(false);
        aircraftCarrierCamera.SetActive(false);
        submarineCamera.SetActive(false);
        missileCamera.SetActive(false);
    }
    public void Destroyer04Camera()
    {
        mainCamera.SetActive(false);
        destroyer01Camera.SetActive(false);
        destroyer02Camera.SetActive(false);
        destroyer03Camera.SetActive(false);
        destroyer04Camera.SetActive(true);
        depotShipCamera.SetActive(false);
        aircraftCarrierCamera.SetActive(false);
        submarineCamera.SetActive(false);
        missileCamera.SetActive(false);
    }
    public void DepotShipCamera()
    {
        mainCamera.SetActive(false);
        destroyer01Camera.SetActive(false);
        destroyer02Camera.SetActive(false);
        destroyer03Camera.SetActive(false);
        destroyer04Camera.SetActive(false);
        depotShipCamera.SetActive(true);
        aircraftCarrierCamera.SetActive(false);
        submarineCamera.SetActive(false);
        missileCamera.SetActive(false);
    }
    public void AircraftCarrierCamera()
    {
        mainCamera.SetActive(false);
        destroyer01Camera.SetActive(false);
        destroyer02Camera.SetActive(false);
        destroyer03Camera.SetActive(false);
        destroyer04Camera.SetActive(false);
        depotShipCamera.SetActive(false);
        aircraftCarrierCamera.SetActive(true);
        submarineCamera.SetActive(false);
        missileCamera.SetActive(false);
    }

    public void SubmarineCamera()
    {
        mainCamera.SetActive(false);
        destroyer01Camera.SetActive(false);
        destroyer02Camera.SetActive(false);
        destroyer03Camera.SetActive(false);
        destroyer04Camera.SetActive(false);
        depotShipCamera.SetActive(false);
        aircraftCarrierCamera.SetActive(false);
        submarineCamera.SetActive(true);
        missileCamera.SetActive(false);
    }
    public void MissileCamera()
    {
        mainCamera.SetActive(false);
        destroyer01Camera.SetActive(false);
        destroyer02Camera.SetActive(false);
        destroyer03Camera.SetActive(false);
        destroyer04Camera.SetActive(false);
        depotShipCamera.SetActive(false);
        aircraftCarrierCamera.SetActive(false);
        submarineCamera.SetActive(false);
        missileCamera.SetActive(true);
    }
}
