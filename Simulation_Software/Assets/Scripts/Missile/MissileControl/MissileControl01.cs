using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileControl01 : MonoBehaviour
{
    public float timeToTarget = 5f; // 固定的飞行时间
    public GameObject explosionEffect; // 爆炸效果的预制体

    private Vector3 targetPosition; // 目标位置
    private Quaternion targetRotation; // 目标角度
    private bool isMoving = false; // 是否正在移动
    private float speed; // 计算出的飞行速度
    private float angularSpeed; // 计算出的角速度

    void Start()
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
            //Debug.LogError("未找到名为 'missile01' 的游戏对象！");
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
            speed = distance / timeToTarget;

            // 计算角速度
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            angularSpeed = angle / timeToTarget;

            Debug.Log($"计算出的飞行速度: {speed}");
            Debug.Log($"计算出的角速度: {angularSpeed}");

            // 开始移动导弹
            isMoving = true;
        }
        else
        {
            //Debug.LogError("未找到名为 'Aircraft Carrier' 的游戏对象！");
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // 获取当前导弹位置
            Vector3 currentPosition = transform.position;

            // 计算方向向量
            Vector3 direction = (targetPosition - currentPosition).normalized;

            // 匀速移动导弹
            transform.position += direction * speed * Time.deltaTime;

            // 匀角速度旋转导弹
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);

            // 检测是否到达目标位置
            if (Vector3.Distance(currentPosition, targetPosition) < 0.5f)
            {
                Explode();
            }
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
        isMoving = false;

        // 固定导弹角度为目标角度30度
        transform.rotation = targetRotation;

        // 生成爆炸效果
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect);
        }

        // 销毁导弹
        Destroy(gameObject);
        Debug.Log("已销毁missile01");

        //销毁航空母舰        
        StartCoroutine(DestroyAircraftCarrierWithDelay(1f)); // 延迟1秒销毁
    }

    private IEnumerator DestroyAircraftCarrierWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject aircraftCarrier = GameObject.Find("Aircraft Carrier");
        if (aircraftCarrier != null)
        {
            Destroy(aircraftCarrier);
        }
    }
}
