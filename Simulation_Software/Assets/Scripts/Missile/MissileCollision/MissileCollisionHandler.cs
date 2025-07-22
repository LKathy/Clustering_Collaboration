using UnityEngine;

public class MissileCollisionHandler : MonoBehaviour
{
    public GameObject explosionPrefab;

    private void OnTriggerEnter(Collider other)
    {
        // 确保碰撞的是目标物体，例如设置为 "Target" 标签
        if (other.CompareTag("Target"))
        {
            // 实例化爆炸特效在目标物体位置
            if (explosionPrefab != null)
            {
                // Vector3 explosionPos = other.ClosestPoint(transform.position);
                Vector3 explosionPos = other.ClosestPoint(transform.position) + new Vector3(-1f, -3f, 2f);
                Instantiate(explosionPrefab, explosionPos, Quaternion.identity);
            }

            // 销毁导弹自身
            Destroy(gameObject);
        }
    }
}
