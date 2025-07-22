using UnityEngine;

public class MissileCollisionHandler : MonoBehaviour
{
    public GameObject explosionPrefab;

    private void OnTriggerEnter(Collider other)
    {
        // ȷ����ײ����Ŀ�����壬��������Ϊ "Target" ��ǩ
        if (other.CompareTag("Target"))
        {
            // ʵ������ը��Ч��Ŀ������λ��
            if (explosionPrefab != null)
            {
                // Vector3 explosionPos = other.ClosestPoint(transform.position);
                Vector3 explosionPos = other.ClosestPoint(transform.position) + new Vector3(-1f, -3f, 2f);
                Instantiate(explosionPrefab, explosionPos, Quaternion.identity);
            }

            // ���ٵ�������
            Destroy(gameObject);
        }
    }
}
