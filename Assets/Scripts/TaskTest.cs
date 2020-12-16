using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class TaskTest : MonoBehaviour
{
    public ComputeBuffer GPUBuffer;
    int[] IntArray;

    void OnEnable()
    {
        IntArray = new int[]{100, 200};
        GPUBuffer = new ComputeBuffer(2, 4);
    }

    void ExecuteTaskA()
    {
        print("InTaskA");
    }

    void ExecuteTaskB(Task ParentTask)
    {
        print("InTaskB");
        //GPUBuffer.SetData(IntArray);
    }

    void ExecuteTask()
    {
        print("BeforeTask");

        Task TaskA = new Task(ExecuteTaskA);
        TaskA.Start();

        Task TaskB = TaskA.ContinueWith(ExecuteTaskB);

        //Sync Task
        //TaskA.Wait();
        TaskB.Wait();
        print("AfterTask");
    }

    void Update()
    {
        //ExecuteTask();
        print("Task");
        //Shader.SetGlobalBuffer("_ASyncBuffer", GPUBuffer);
    }

    void OnDisable()
    {
        GPUBuffer.Dispose();
    }
}
