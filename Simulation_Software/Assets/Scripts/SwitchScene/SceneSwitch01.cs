using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  //����UI�����ռ���ʹ��UI���
using UnityEngine.SceneManagement;
using System;  //��������������ռ�

public class SceneSwitch01 : MonoBehaviour
{
    public Button switchButton;  // ����һ��Button���͵ı���
    public Animator sceneAnimator;  // ����һ��Animator���͵ı��������ڿ��Ƴ����л�����
    public GameObject loadingMask; // ����LoadingMask����
    // Start is called before the first frame update
    void Start()
    {
        if (this.transform.parent != null)
        {
            this.transform.SetParent(null);
        }
        GameObject.DontDestroyOnLoad(this.gameObject);  // ȷ���˶����ڳ����л�ʱ���ᱻ����iqiu
        switchButton.onClick.AddListener(SwitchScene);  // ��Ӱ�ť����¼�������
        loadingMask.SetActive(false); // ��ʼ����
    }

    private void SwitchScene()
    {
        StartCoroutine(LoadScene(1));  // ����Э�������س��������볡������1
    }

    IEnumerator LoadScene(int index)
    {
        loadingMask.SetActive(true); // ��ʾLoading����
        sceneAnimator.SetBool("FadeIn", true);
        sceneAnimator.SetBool("FadeOut", false);

        // �첽���س���
        AsyncOperation async = SceneManager.LoadSceneAsync(index);
        async.allowSceneActivation = false;

        // �ȴ����ص�90%
        while (async.progress < 0.9f)
        {
            yield return null;
        }

        // ��ѡ���ȴ�һС��ʱ�䣬���û�����
        yield return new WaitForSeconds(0.5f);

        // ���������󼤻��
        async.allowSceneActivation = true;
        async.completed += OnLoadedScene;
    }

    private void OnLoadedScene(AsyncOperation operation)
    {
        // �³���������ɺ󣬵���
        if (sceneAnimator == null)
        {
            GameObject animatorObj = GameObject.Find("SwitchBG");
            if (animatorObj != null)
                sceneAnimator = animatorObj.GetComponent<Animator>();
        }
        if (sceneAnimator != null)
        {
            sceneAnimator.SetBool("FadeIn", false);
            sceneAnimator.SetBool("FadeOut", true);
        }

        // ����Loading����
        if (loadingMask != null)
            loadingMask.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
