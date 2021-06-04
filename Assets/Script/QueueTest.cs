using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueueTest : MonoBehaviour
{
    public Queue<int> QueueA;
    public Queue<int> QueueB;

    // Start is called before the first frame update
    void Start()
    {
        QueueA = new Queue<int>();
        QueueA.Enqueue(1);
        QueueA.Enqueue(2);
        QueueA.Enqueue(3);

        foreach(int QueueAValue in QueueA)
        {
            Debug.Log("QueueA + : " + QueueAValue);
        }

        Debug.Log("XXXXXXX");

        QueueB = new Queue<int>(QueueA);
        QueueB.Enqueue(4);

        foreach(int QueueBValue in QueueB)
        {
            Debug.Log("QueueB + : " + QueueBValue);
        }

        Debug.Log("XXXXXXX");

        foreach(int QueueAValue in QueueA)
        {
            Debug.Log("QueueA + : " + QueueAValue);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
