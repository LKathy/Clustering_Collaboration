using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileControl01 : MonoBehaviour
{
    public float timeToTarget = 5f; // �̶��ķ���ʱ��
    public GameObject explosionEffect; // ��ըЧ����Ԥ����

    private Vector3 targetPosition; // Ŀ��λ��
    private Quaternion targetRotation; // Ŀ��Ƕ�
    private bool isMoving = false; // �Ƿ������ƶ�
    private float speed; // ������ķ����ٶ�
    private float angularSpeed; // ������Ľ��ٶ�

    void Start()
    {
        // ��ȡ��Ϊ "missile01" ����Ϸ����
        GameObject missile01 = GameObject.Find("missile01");
        if (missile01 != null)
        {
            // ��ȡ��ʼλ��
            Vector3 initialPosition = missile01.transform.position;

            // ��ȡ��ʼ�Ƕȣ�ŷ���ǣ�
            Quaternion initialRotation = missile01.transform.rotation;

            // ���õ����ĳ�ʼλ�úͽǶ�
            transform.position = initialPosition;
            transform.rotation = initialRotation;

            Debug.Log($"Missile01 ��ʼλ��: {initialPosition}");
            Debug.Log($"Missile01 ��ʼ�Ƕ�: {initialRotation.eulerAngles}");
        }
        else
        {
            //Debug.LogError("δ�ҵ���Ϊ 'missile01' ����Ϸ����");
            return;
        }

        // ��ȡ��Ϊ "Aircraft Carrier" ����Ϸ����
        GameObject aircraftCarrier = GameObject.Find("Aircraft Carrier");
        if (aircraftCarrier != null)
        {
            // ��ȡĿ��λ��
            targetPosition = aircraftCarrier.transform.position;

            // ����Ŀ��λ�õ� x ����
            targetPosition.x += 15;

            // ����Ŀ��Ƕ�Ϊ30��
            targetRotation = Quaternion.Euler(30f, transform.eulerAngles.y, transform.eulerAngles.z);

            Debug.Log($"Aircraft Carrier ��ʼλ��: {targetPosition}");
            Debug.Log($"Ŀ��Ƕ�: {targetRotation.eulerAngles}");

            // ��������ٶ�
            float distance = Vector3.Distance(transform.position, targetPosition);
            speed = distance / timeToTarget;

            // ������ٶ�
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            angularSpeed = angle / timeToTarget;

            Debug.Log($"������ķ����ٶ�: {speed}");
            Debug.Log($"������Ľ��ٶ�: {angularSpeed}");

            // ��ʼ�ƶ�����
            isMoving = true;
        }
        else
        {
            //Debug.LogError("δ�ҵ���Ϊ 'Aircraft Carrier' ����Ϸ����");
        }
    }

    void Update()
    {
        if (isMoving)
        {
            // ��ȡ��ǰ����λ��
            Vector3 currentPosition = transform.position;

            // ���㷽������
            Vector3 direction = (targetPosition - currentPosition).normalized;

            // �����ƶ�����
            transform.position += direction * speed * Time.deltaTime;

            // �Ƚ��ٶ���ת����
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, angularSpeed * Time.deltaTime);

            // ����Ƿ񵽴�Ŀ��λ��
            if (Vector3.Distance(currentPosition, targetPosition) < 0.5f)
            {
                Explode();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ��ײʱ������ը
        Explode();
    }

    private void Explode()
    {
        // ֹͣ�ƶ�
        isMoving = false;

        // �̶������Ƕ�ΪĿ��Ƕ�30��
        transform.rotation = targetRotation;

        // ���ɱ�ըЧ��
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect);
        }

        // ���ٵ���
        Destroy(gameObject);
        Debug.Log("������missile01");

        //���ٺ���ĸ��        
        StartCoroutine(DestroyAircraftCarrierWithDelay(1f)); // �ӳ�1������
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
