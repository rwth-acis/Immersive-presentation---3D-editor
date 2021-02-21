using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class SaveProfilerData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Profiler.logFile = @"C:\Users\Lukas\Downloads\Unity_Trash\ImPres-3D-Editor\mylog2.raw"; //Also supports passing "myLog.raw"
        Profiler.enableBinaryLog = true;
        Profiler.enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
