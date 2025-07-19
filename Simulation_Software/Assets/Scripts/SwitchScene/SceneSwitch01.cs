using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  //引入UI命名空间以使用UI组件
using UnityEngine.SceneManagement;
using System;  //首先引入此命名空间

public class SceneSwitch01 : MonoBehaviour
{
    public Button switchButton;  // 声明一个Button类型的变量
    public Animator sceneAnimator;  // 声明一个Animator类型的变量，用于控制场景切换动画
    public GameObject loadingMask; // 拖入LoadingMask对象
    // Start is called before the first frame update
    void Start()
    {
        if (this.transform.parent != null)
        {
            this.transform.SetParent(null);
        }
        GameObject.DontDestroyOnLoad(this.gameObject);  // 确保此对象在场景切换时不会被销毁iqiu
        switchButton.onClick.AddListener(SwitchScene);  // 添加按钮点击事件监听器
        loadingMask.SetActive(false); // 初始隐藏
    }

    private void SwitchScene()
    {
        StartCoroutine(LoadScene(1));  // 调用协程来加载场景，传入场景索引1
    }

    IEnumerator LoadScene(int index)
    {
        loadingMask.SetActive(true); // 显示Loading界面
        sceneAnimator.SetBool("FadeIn", true);
        sceneAnimator.SetBool("FadeOut", false);

        // 异步加载场景
        AsyncOperation async = SceneManager.LoadSceneAsync(index);
        async.allowSceneActivation = false;

        // 等待加载到90%
        while (async.progress < 0.9f)
        {
            yield return null;
        }

        // 可选：等待一小段时间，给用户缓冲
        yield return new WaitForSeconds(0.5f);

        // 动画结束后激活场景
        async.allowSceneActivation = true;
        async.completed += OnLoadedScene;
    }

    private void OnLoadedScene(AsyncOperation operation)
    {
        // 新场景加载完成后，淡入
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

        // 隐藏Loading界面
        if (loadingMask != null)
            loadingMask.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
