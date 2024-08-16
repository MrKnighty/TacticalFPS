using System;
using UnityEngine;

public static class Wobble
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // Camera cam;
    // void Start()
    // {
    //     cam = Camera.main;
    // }
    // // public float wobbleSpeed;
    // // public float wobbleAmount = 1;
    // // [SerializeField, Range(0,1)]float x = 1;
    // // public bool wob = false;

    // void Update()
    // {
    //     // if(Input.GetMouseButtonDown(0))
    //     //     x = 0;
    //     // if(x != 1)
    //     // {
    //     //     if(wob)x += Time.deltaTime * wobbleSpeed;
    //     //     x = Mathf.Clamp01(x);
    //     //     Quaternion rot = cam.transform.rotation;
    //     //     Vector3 camEuler = rot.eulerAngles;
    //     //     cam.transform.rotation = Quaternion.Euler(camEuler.x,camEuler.y, WobbleEase(x));

    //     // }
    // }
    public static float WobbleEase(float x, float power)
    {
        // Quaternion rot = cam.transform.rotation;
        // Vector3 camEuler = rot.eulerAngles;
        //cam.transform.rotation = Quaternion.Euler(camEuler.x,camEuler.y, WobbleEase(x)); Use this for rotation <-

        return Mathf.Sin(Time.time * 2 * MathF.PI) * easeOutSine(x) * power;
    }
    static float  easeOutSine(float x)
    {
        return Mathf.Sin(x * Mathf.PI / 2);
    }
    
    
    
}
