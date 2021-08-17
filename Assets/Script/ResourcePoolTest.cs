using InfinityTech.Rendering.GPUResource;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcePoolTest : MonoBehaviour
{
    BufferDescription bufferADescription;
    BufferDescription bufferBDescription;
    FResourcePool m_ResourcePool;

    void OnEnable()
    {
        m_ResourcePool = new FResourcePool();

        bufferADescription = new BufferDescription(16, 4);
        bufferADescription.name = "BufferA";

        bufferBDescription = new BufferDescription(16, 4);
        bufferBDescription.name = "BufferB";
    }

    void Update()
    {
        BufferRef bufferA = m_ResourcePool.AllocateBuffer(bufferADescription);
        BufferRef bufferB = m_ResourcePool.AllocateBuffer(bufferBDescription);

        m_ResourcePool.ReleaseBuffer(bufferA);
        m_ResourcePool.ReleaseBuffer(bufferB);
    }

    void OnDisable()
    {
        m_ResourcePool.Disposed();
    }
}
