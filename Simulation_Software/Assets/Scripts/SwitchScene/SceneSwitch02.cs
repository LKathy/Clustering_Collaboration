using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch02 : MonoBehaviour
{
    public Image imageEffect;
    public float speed = 1; // ͸���ȱ仯���ٶ�
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(FadeIn()); //����Э��
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Onclick(string sceneName)
    {
        //͸���ȱ仯�Ĺ���
        StartCoroutine(FadeOut(sceneName)); //����Э��
        //SceneManager.LoadScene("MainScene06");
        //SceneManager.LoadScene(1);
    }

    //����
    IEnumerator FadeOut(string sceneName)
    {
        Color tempColor = imageEffect.color;
        tempColor.a = 0; // ���ó�ʼ͸����Ϊ0
        imageEffect.color = tempColor;
        while (imageEffect.color.a < 1) 
        {
            imageEffect.color += new Color(0, 0, 0, speed * Time.deltaTime);
            yield return null; // �ȴ�һ֡
        }
        SceneManager.LoadScene(sceneName);
    }

    //����
    IEnumerator FadeIn()
    {
        Color tempColor = imageEffect.color;
        tempColor.a = 1; // ���ó�ʼ͸����Ϊ1
        imageEffect.color = tempColor;
        while (imageEffect.color.a > 0)
        {
            imageEffect.color -= new Color(0, 0, 0, speed * Time.deltaTime);
            yield return null; // �ȴ�һ֡
        }
    }
}
