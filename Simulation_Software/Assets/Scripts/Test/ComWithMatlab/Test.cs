using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathWorks.MATLAB.NET.Arrays;
using MathWorks.MATLAB.NET.Utility;
using test;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        MWArray a = 1;
        Class1 d = new Class1();
        MWArray e = d.test(a);
        Debug.Log(e);
        Debug.Log("b");
    }

    // Update is called once per frame
    void Update()
    {

    }
}