using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileControlDroft : MonoBehaviour
{
    public float timeToTarget = 5f; // 固定的飞行时间
    public GameObject explosionEffect; // 爆炸效果的预制体

    private Vector3 targetPosition; // 目标位置
    private Quaternion targetRotation; // 目标角度
    private bool missile01IsMoving = false; // 是否正在移动
    private bool missile02IsMoving = false;
    private bool missile03IsMoving = false;
    private float missile01Speed; // 计算出的飞行速度
    private float missile02Speed; 
    private float missile03Speed; 
    private float missile01AngularSpeed; // 计算出的角速度
    private float missile02AngularSpeed; 
    private float missile03AngularSpeed; 

    void Start()
    {
        CalculateOfMissile01();
        CalculateOfMissile02();
        CalculateOfMissile03();
    }

    void Update()
    {
        if (missile01IsMoving)
        {
            // 获取当前导弹位置
            Vector3 currentPosition = transform.position;

            // 计算方向向量
            Vector3 direction = (targetPosition - currentPosition).normalized;

            // 匀速移动导弹
            transform.position += direction * missile01Speed * Time.deltaTime;

            // 匀角速度旋转导弹
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, missile01AngularSpeed * Time.deltaTime);

            // 检测是否到达目标位置
            if (Vector3.Distance(currentPosition, targetPosition) < 0.5f)
            {
                Explode();
            }
        }

        if (missile02IsMoving)
        {
            // 获取当前导弹位置
            Vector3 currentPosition = transform.position;

            // 计算方向向量
            Vector3 direction = (targetPosition - currentPosition).normalized;

            // 匀速移动导弹
            transform.position += direction * missile02Speed * Time.deltaTime;

            // 匀角速度旋转导弹
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, missile02AngularSpeed * Time.deltaTime);

            // 检测是否到达目标位置
            if (Vector3.Distance(currentPosition, targetPosition) < 0.5f)
            {
                Explode();
            }
        }

        if (missile03IsMoving)
        {
            // 获取当前导弹位置
            Vector3 currentPosition = transform.position;

            // 计算方向向量
            Vector3 direction = (targetPosition - currentPosition).normalized;

            // 匀速移动导弹
            transform.position += direction * missile03Speed * Time.deltaTime;

            // 匀角速度旋转导弹
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, missile03AngularSpeed * Time.deltaTime);

            // 检测是否到达目标位置
            if (Vector3.Distance(currentPosition, targetPosition) < 0.5f)
            {
                Explode();
            }
        }
    }

    private void CalculateOfMissile01() 
    {
        // 获取名为 "missile01" 的游戏对象
        GameObject missile01 = GameObject.Find("missile01");
        if (missile01 != null)
        {
            // 获取初始位置
            Vector3 initialPosition = missile01.transform.position;

            // 获取初始角度（欧拉角）
            Quaternion initialRotation = missile01.transform.rotation;

            // 设置导弹的初始位置和角度
            transform.position = initialPosition;
            transform.rotation = initialRotation;

            Debug.Log($"Missile01 初始位置: {initialPosition}");
            Debug.Log($"Missile01 初始角度: {initialRotation.eulerAngles}");
        }
        else
        {
            Debug.LogError("未找到名为 'missile01' 的游戏对象！");
            return;
        }

        // 获取名为 "Aircraft Carrier" 的游戏对象
        GameObject aircraftCarrier = GameObject.Find("Aircraft Carrier");
        if (aircraftCarrier != null)
        {
            // 获取目标位置
            targetPosition = aircraftCarrier.transform.position;

            // 增加目标位置的 x 坐标
            targetPosition.x += 15;

            // 设置目标角度为30度
            targetRotation = Quaternion.Euler(30f, transform.eulerAngles.y, transform.eulerAngles.z);

            Debug.Log($"Aircraft Carrier 初始位置: {targetPosition}");
            Debug.Log($"目标角度: {targetRotation.eulerAngles}");

            // 计算飞行速度
            float distance = Vector3.Distance(transform.position, targetPosition);
            missile01Speed = distance / timeToTarget;

            // 计算角速度
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            missile01AngularSpeed = angle / timeToTarget;

            Debug.Log($"计算出的飞行速度: {missile01Speed}");
            Debug.Log($"计算出的角速度: {missile01AngularSpeed}");

            // 开始移动导弹
            missile01IsMoving = true;
        }
        else
        {
            Debug.LogError("未找到名为 'Aircraft Carrier' 的游戏对象！");
        }
    }

    private void CalculateOfMissile02()
    {
        // 获取名为 "missile02" 的游戏对象
        GameObject missile02 = GameObject.Find("missile02");
        if (missile02 != null)
        {
            // 获取初始位置
            Vector3 initialPosition = missile02.transform.position;

            // 获取初始角度（欧拉角）
            Quaternion initialRotation = missile02.transform.rotation;

            // 设置导弹的初始位置和角度
            transform.position = initialPosition;
            transform.rotation = initialRotation;

            Debug.Log($"Missile02 初始位置: {initialPosition}");
            Debug.Log($"Missile02 初始角度: {initialRotation.eulerAngles}");
        }
        else
        {
            Debug.LogError("未找到名为 'missile02' 的游戏对象！");
            return;
        }

        // 获取名为 "Aircraft Carrier" 的游戏对象
        GameObject aircraftCarrier = GameObject.Find("Aircraft Carrier");
        if (aircraftCarrier != null)
        {
            // 获取目标位置
            targetPosition = aircraftCarrier.transform.position;

            // 增加目标位置的 x 坐标
            targetPosition.x += 5;

            // 设置目标角度为30度
            targetRotation = Quaternion.Euler(30f, transform.eulerAngles.y, transform.eulerAngles.z);

            Debug.Log($"Aircraft Carrier 初始位置: {targetPosition}");
            Debug.Log($"目标角度: {targetRotation.eulerAngles}");

            // 计算飞行速度
            float distance = Vector3.Distance(transform.position, targetPosition);
            missile02Speed = distance / timeToTarget;

            // 计算角速度
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            missile02AngularSpeed = angle / timeToTarget;

            Debug.Log($"计算出的飞行速度: {missile02Speed}");
            Debug.Log($"计算出的角速度: {missile02AngularSpeed}");

            // 开始移动导弹
            missile02IsMoving = true;
        }
        else
        {
            Debug.LogError("未找到名为 'Aircraft Carrier' 的游戏对象！");
        }
    }

    private void CalculateOfMissile03()
    {
        // 获取名为 "missile03" 的游戏对象
        GameObject missile03 = GameObject.Find("missile03");
        if (missile03 != null)
        {
            // 获取初始位置
            Vector3 initialPosition = missile03.transform.position;

            // 获取初始角度（欧拉角）
            Quaternion initialRotation = missile03.transform.rotation;

            // 设置导弹的初始位置和角度
            transform.position = initialPosition;
            transform.rotation = initialRotation;

            Debug.Log($"Missile03 初始位置: {initialPosition}");
            Debug.Log($"Missile03 初始角度: {initialRotation.eulerAngles}");
        }
        else
        {
            Debug.LogError("未找到名为 'missile03' 的游戏对象！");
            return;
        }

        // 获取名为 "Aircraft Carrier" 的游戏对象
        GameObject aircraftCarrier = GameObject.Find("Aircraft Carrier");
        if (aircraftCarrier != null)
        {
            // 获取目标位置
            targetPosition = aircraftCarrier.transform.position;

            // 增加目标位置的 x 坐标
            targetPosition.x += 25;

            // 设置目标角度为30度
            targetRotation = Quaternion.Euler(30f, transform.eulerAngles.y, transform.eulerAngles.z);

            Debug.Log($"Aircraft Carrier 初始位置: {targetPosition}");
            Debug.Log($"目标角度: {targetRotation.eulerAngles}");

            // 计算飞行速度
            float distance = Vector3.Distance(transform.position, targetPosition);
            missile03Speed = distance / timeToTarget;

            // 计算角速度
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            missile03AngularSpeed = angle / timeToTarget;

            Debug.Log($"计算出的飞行速度: {missile03Speed}");
            Debug.Log($"计算出的角速度: {missile03AngularSpeed}");

            // 开始移动导弹
            missile03IsMoving = true;
        }
        else
        {
            Debug.LogError("未找到名为 'Aircraft Carrier' 的游戏对象！");
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        // 碰撞时触发爆炸
        Explode();
    }

    private void Explode()
    {
        // 停止移动
        missile01IsMoving = false;
        missile02IsMoving = false;
        missile03IsMoving = false;

        // 固定导弹角度为目标角度30度
        transform.rotation = targetRotation;

        // 生成爆炸效果
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect);
        }

        // 销毁导弹
        Destroy(gameObject);

        //销毁航空母舰
        Destroy(GameObject.Find("Aircraft Carrier"));
    }
}
