using System.Collections;
using UnityEngine;

public class ObjectFlicker : MonoBehaviour
{
    public Renderer objectRenderer; // �������Ⱦ��
    public float flickerInterval = 0.5f; // ��˸���ʱ�䣨�룩
    public Transform target; // Ŀ����Ϸ����� Transform

    private bool isFlickering = false;

    void Start()
    {
        if (objectRenderer == null)
        {
            objectRenderer = GetComponent<Renderer>();
        }

        StartFlicker(); // ��ʼ��˸
    }

    void Update()
    {
        if (target == null)
        {
            Destroy(gameObject); // ���ٵ�ǰ��Ϸ����
            return;
        }

        FollowTarget(); // ÿ֡���¸����߼�
    }

    public void StartFlicker()
    {
        if (!isFlickering)
        {
            StartCoroutine(Flicker());
        }
    }

    private IEnumerator Flicker()
    {
        isFlickering = true;

        while (true) // ����ѭ����ֱ�����屻����
        {
            objectRenderer.enabled = !objectRenderer.enabled; // �л���Ⱦ��������״̬
            yield return new WaitForSeconds(flickerInterval); // �ȴ�ָ���ļ��ʱ��
        }
    }

    private void FollowTarget()
    {
        if (target != null)
        {
            // ֱ�ӽ������λ������ΪĿ���λ��
            transform.position = target.position + new Vector3(0f, 5f, 0f);
        }
    }

    void OnDestroy()
    {
        // ȷ��������ʱֹͣ��˸��Э�̻��Զ�������
        objectRenderer.enabled = true; // ȷ������ǰ��Ⱦ������
    }
}
