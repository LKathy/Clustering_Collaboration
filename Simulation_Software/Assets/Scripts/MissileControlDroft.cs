using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissileControlDroft : MonoBehaviour
{
    public float timeToTarget = 5f; // �̶��ķ���ʱ��
    public GameObject explosionEffect; // ��ըЧ����Ԥ����

    private Vector3 targetPosition; // Ŀ��λ��
    private Quaternion targetRotation; // Ŀ��Ƕ�
    private bool missile01IsMoving = false; // �Ƿ������ƶ�
    private bool missile02IsMoving = false;
    private bool missile03IsMoving = false;
    private float missile01Speed; // ������ķ����ٶ�
    private float missile02Speed; 
    private float missile03Speed; 
    private float missile01AngularSpeed; // ������Ľ��ٶ�
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
            // ��ȡ��ǰ����λ��
            Vector3 currentPosition = transform.position;

            // ���㷽������
            Vector3 direction = (targetPosition - currentPosition).normalized;

            // �����ƶ�����
            transform.position += direction * missile01Speed * Time.deltaTime;

            // �Ƚ��ٶ���ת����
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, missile01AngularSpeed * Time.deltaTime);

            // ����Ƿ񵽴�Ŀ��λ��
            if (Vector3.Distance(currentPosition, targetPosition) < 0.5f)
            {
                Explode();
            }
        }

        if (missile02IsMoving)
        {
            // ��ȡ��ǰ����λ��
            Vector3 currentPosition = transform.position;

            // ���㷽������
            Vector3 direction = (targetPosition - currentPosition).normalized;

            // �����ƶ�����
            transform.position += direction * missile02Speed * Time.deltaTime;

            // �Ƚ��ٶ���ת����
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, missile02AngularSpeed * Time.deltaTime);

            // ����Ƿ񵽴�Ŀ��λ��
            if (Vector3.Distance(currentPosition, targetPosition) < 0.5f)
            {
                Explode();
            }
        }

        if (missile03IsMoving)
        {
            // ��ȡ��ǰ����λ��
            Vector3 currentPosition = transform.position;

            // ���㷽������
            Vector3 direction = (targetPosition - currentPosition).normalized;

            // �����ƶ�����
            transform.position += direction * missile03Speed * Time.deltaTime;

            // �Ƚ��ٶ���ת����
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, missile03AngularSpeed * Time.deltaTime);

            // ����Ƿ񵽴�Ŀ��λ��
            if (Vector3.Distance(currentPosition, targetPosition) < 0.5f)
            {
                Explode();
            }
        }
    }

    private void CalculateOfMissile01() 
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
            Debug.LogError("δ�ҵ���Ϊ 'missile01' ����Ϸ����");
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
            missile01Speed = distance / timeToTarget;

            // ������ٶ�
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            missile01AngularSpeed = angle / timeToTarget;

            Debug.Log($"������ķ����ٶ�: {missile01Speed}");
            Debug.Log($"������Ľ��ٶ�: {missile01AngularSpeed}");

            // ��ʼ�ƶ�����
            missile01IsMoving = true;
        }
        else
        {
            Debug.LogError("δ�ҵ���Ϊ 'Aircraft Carrier' ����Ϸ����");
        }
    }

    private void CalculateOfMissile02()
    {
        // ��ȡ��Ϊ "missile02" ����Ϸ����
        GameObject missile02 = GameObject.Find("missile02");
        if (missile02 != null)
        {
            // ��ȡ��ʼλ��
            Vector3 initialPosition = missile02.transform.position;

            // ��ȡ��ʼ�Ƕȣ�ŷ���ǣ�
            Quaternion initialRotation = missile02.transform.rotation;

            // ���õ����ĳ�ʼλ�úͽǶ�
            transform.position = initialPosition;
            transform.rotation = initialRotation;

            Debug.Log($"Missile02 ��ʼλ��: {initialPosition}");
            Debug.Log($"Missile02 ��ʼ�Ƕ�: {initialRotation.eulerAngles}");
        }
        else
        {
            Debug.LogError("δ�ҵ���Ϊ 'missile02' ����Ϸ����");
            return;
        }

        // ��ȡ��Ϊ "Aircraft Carrier" ����Ϸ����
        GameObject aircraftCarrier = GameObject.Find("Aircraft Carrier");
        if (aircraftCarrier != null)
        {
            // ��ȡĿ��λ��
            targetPosition = aircraftCarrier.transform.position;

            // ����Ŀ��λ�õ� x ����
            targetPosition.x += 5;

            // ����Ŀ��Ƕ�Ϊ30��
            targetRotation = Quaternion.Euler(30f, transform.eulerAngles.y, transform.eulerAngles.z);

            Debug.Log($"Aircraft Carrier ��ʼλ��: {targetPosition}");
            Debug.Log($"Ŀ��Ƕ�: {targetRotation.eulerAngles}");

            // ��������ٶ�
            float distance = Vector3.Distance(transform.position, targetPosition);
            missile02Speed = distance / timeToTarget;

            // ������ٶ�
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            missile02AngularSpeed = angle / timeToTarget;

            Debug.Log($"������ķ����ٶ�: {missile02Speed}");
            Debug.Log($"������Ľ��ٶ�: {missile02AngularSpeed}");

            // ��ʼ�ƶ�����
            missile02IsMoving = true;
        }
        else
        {
            Debug.LogError("δ�ҵ���Ϊ 'Aircraft Carrier' ����Ϸ����");
        }
    }

    private void CalculateOfMissile03()
    {
        // ��ȡ��Ϊ "missile03" ����Ϸ����
        GameObject missile03 = GameObject.Find("missile03");
        if (missile03 != null)
        {
            // ��ȡ��ʼλ��
            Vector3 initialPosition = missile03.transform.position;

            // ��ȡ��ʼ�Ƕȣ�ŷ���ǣ�
            Quaternion initialRotation = missile03.transform.rotation;

            // ���õ����ĳ�ʼλ�úͽǶ�
            transform.position = initialPosition;
            transform.rotation = initialRotation;

            Debug.Log($"Missile03 ��ʼλ��: {initialPosition}");
            Debug.Log($"Missile03 ��ʼ�Ƕ�: {initialRotation.eulerAngles}");
        }
        else
        {
            Debug.LogError("δ�ҵ���Ϊ 'missile03' ����Ϸ����");
            return;
        }

        // ��ȡ��Ϊ "Aircraft Carrier" ����Ϸ����
        GameObject aircraftCarrier = GameObject.Find("Aircraft Carrier");
        if (aircraftCarrier != null)
        {
            // ��ȡĿ��λ��
            targetPosition = aircraftCarrier.transform.position;

            // ����Ŀ��λ�õ� x ����
            targetPosition.x += 25;

            // ����Ŀ��Ƕ�Ϊ30��
            targetRotation = Quaternion.Euler(30f, transform.eulerAngles.y, transform.eulerAngles.z);

            Debug.Log($"Aircraft Carrier ��ʼλ��: {targetPosition}");
            Debug.Log($"Ŀ��Ƕ�: {targetRotation.eulerAngles}");

            // ��������ٶ�
            float distance = Vector3.Distance(transform.position, targetPosition);
            missile03Speed = distance / timeToTarget;

            // ������ٶ�
            float angle = Quaternion.Angle(transform.rotation, targetRotation);
            missile03AngularSpeed = angle / timeToTarget;

            Debug.Log($"������ķ����ٶ�: {missile03Speed}");
            Debug.Log($"������Ľ��ٶ�: {missile03AngularSpeed}");

            // ��ʼ�ƶ�����
            missile03IsMoving = true;
        }
        else
        {
            Debug.LogError("δ�ҵ���Ϊ 'Aircraft Carrier' ����Ϸ����");
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
        missile01IsMoving = false;
        missile02IsMoving = false;
        missile03IsMoving = false;

        // �̶������Ƕ�ΪĿ��Ƕ�30��
        transform.rotation = targetRotation;

        // ���ɱ�ըЧ��
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect);
        }

        // ���ٵ���
        Destroy(gameObject);

        //���ٺ���ĸ��
        Destroy(GameObject.Find("Aircraft Carrier"));
    }
}
