using UnityEngine;

[ExecuteAlways]
public class HashID : MonoBehaviour
{
    public int lODIndex;
    public int meshIndex;
    public int materialIndex;

    void LateUpdate()
    {
        Debug.Log((meshIndex | 32) >> (lODIndex << 32 | materialIndex));
    }
}
