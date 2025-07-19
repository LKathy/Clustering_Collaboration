using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch02 : MonoBehaviour
{
    public Image imageEffect;
    public float speed = 1; // 透明度变化的速度
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeIn()); //开启协程
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Onclick(string sceneName)
    {
        //透明度变化的过程
        StartCoroutine(FadeOut(sceneName)); //开启协程
        //SceneManager.LoadScene("MainScene06");
        //SceneManager.LoadScene(1);
    }

    //淡出
    IEnumerator FadeOut(string sceneName)
    {
        Color tempColor = imageEffect.color;
        tempColor.a = 0; // 设置初始透明度为0
        imageEffect.color = tempColor;
        while (imageEffect.color.a < 1) 
        {
            imageEffect.color += new Color(0, 0, 0, speed * Time.deltaTime);
            yield return null; // 等待一帧
        }
        SceneManager.LoadScene(sceneName);
    }

    //淡入
    IEnumerator FadeIn()
    {
        Color tempColor = imageEffect.color;
        tempColor.a = 1; // 设置初始透明度为1
        imageEffect.color = tempColor;
        while (imageEffect.color.a > 0)
        {
            imageEffect.color -= new Color(0, 0, 0, speed * Time.deltaTime);
            yield return null; // 等待一帧
        }
    }
}
