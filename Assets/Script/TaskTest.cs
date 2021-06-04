using UnityEngine;
using System.Threading;
using System.Collections;
using UnityEngine.Rendering;
using System.Threading.Tasks;
using System.Collections.Generic;

public class TaskTest : MonoBehaviour
{
    Thread MyThread;

    void OnEnable()
    {
        MyThread = new Thread(ThreadFunc);
        MyThread.Start();
    }

    void Update()
    {

    }

    void ThreadFunc()
    {

    }

    void OnDisable()
    {

    }
}
