using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Rotate : MonoBehaviour
{
    public Vector3 axil = Vector3.up;
    public float speed = 0.1f;

    void OnEnable()
    {
        //Debug.Log("One : " + (548 << 16 | 21720));
        //Debug.Log("Two : " + (544 << 16 | 21724));
    }

    void Update()
    {
        transform.Rotate(axil, speed, Space.World);
    }

#if UNITY_EDITOR
    [MenuItem("GameObject/EntityAction/RandomRotateValue", false, 9)]
    public static void SetEntityRandomRotateValue(MenuCommand menuCommand)
    {
        GameObject[] EntityList = Selection.gameObjects;
        for (int i = 0; i < EntityList.Length; i++)
        {
            GameObject Entity = EntityList[i];
            Rotate Rotater = Entity.GetComponent<Rotate>();

            float X = Random.Range(-1.1f, 1.1f);
            float Y = Random.Range(-1.1f, 1.1f);
            float Z = Random.Range(-1.1f, 1.1f);

            Rotater.speed = Random.Range(-1.1f, 1.1f);
            Rotater.axil = new Vector3(X, Y, Z);
        }
    }
#endif
}
