using Unity.Jobs;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;
using InfinityTech.Core.Native;
using Unity.Collections.LowLevel.Unsafe;
using InfinityTech.Rendering.MeshPipeline;
using System.Runtime.InteropServices;
using System.Threading;
using System.Collections.Generic;
using System;
using static InfinityTech.Core.Native.FSortFactory;

[BurstCompile]
public struct SortJob : IJob
{
    public NativeArray<float> array;

    public void Execute()
    {
        array.Sort();
    }
}

[BurstCompile]
public struct TestJob : IJob
{
    public float value;
    public NativeArray<float> result;
    
    public void Execute()
    {
        for(int i = 0; i < result.Length; i++)
        {
            result[i] = value;
        }
    }
}

//[BurstCompile]
public struct OneJob : IJobParallelFor
{
    public void Execute(int i)
    {
        Debug.Log(i);
    }
}

[BurstCompile]
public struct ParallelTestJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<int> ParallelData;
    
    [NativeDisableParallelForRestriction]
    public NativeList<int> ParallelBuffer;

    public void Execute(int index)
    {
        if(ParallelData[index] % 512 == 0)
        {
            ParallelBuffer.Add(index);
        }
    }
}

[BurstCompile]
public struct ParallelCopy<T> : IJobParallelFor where T : struct
{
    [ReadOnly]
    public NativeArray<T> InputArray;

    [WriteOnly]
    public NativeArray<T> OutputArray;

    public void Execute(int index)
    {
        OutputArray[index] = InputArray[index];
    }
}

[BurstCompile]
public struct FParallelListAddJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<int> ParallelData;

    [NativeDisableParallelForRestriction]
    public NativeList<int>.ParallelWriter ParallelBuffer;

    public void Execute(int index)
    {
        if (ParallelData[index] % 64 == 0)
        {
            ParallelBuffer.AddNoResize(index);
        }
    }
}

//[BurstCompile]
public struct ParallelHashmapJob : IJobParallelFor
{
    [ReadOnly]
    public NativeArray<int> HashmapKey;
    
    [ReadOnly]
    public NativeHashMap<int, float> HashmapData;

    public void Execute(int index)
    {
        int Key = HashmapKey[index];
        Debug.Log(HashmapData[Key]);
    }
}

public unsafe struct PointerStruct
{
    public float Float;
}

public class PointerClass
{
    public float Float;
}

public struct PointerClassJob : IJob
{
    public GCHandle GCRef;

    public void Execute()
    {
        PointerClass PC = (PointerClass)GCRef.Target;
        PC.Float = 102.5f;
        Debug.Log(PC.Float);
    }
}

[BurstCompile]
public unsafe struct AtomicJob : IJob
{
    [NativeDisableUnsafePtrRestriction]
    public int* Count;
    //public NativeArray<float> Result;

    public void Execute()
    {
        Interlocked.Increment(ref Count[0]);
    }
}

public unsafe class JobTest : MonoBehaviour
{
    public int SortNum = 1024;
    public bool CustomSort = false;
    public bool ParallelSort = true;
    NativeArray<float> Result;

    void OnEnable()
    {
        Result = new NativeArray<float>(SortNum, Allocator.Persistent);

        for(int i = 0; i < Result.Length; i++)
        {
            Result[i] = UnityEngine.Random.value;
        }

        //GCJobTest();
        //NativeHashmapTest();
        //UnsafeStructTest();
        //UnsafeArrayTest();
        //UnsafeClassTest();
        //NativeMultiHashmapTest();
        //NativeHashmapToArrayTest();

        /*int* MyData = (int*)UnsafeUtility.Malloc(sizeof(int), 64, Allocator.TempJob);
        MyData[0] = 1;

        AtomicJob Atomic = new AtomicJob();
        Atomic.Count = MyData;
        Atomic.Run();

        print(MyData[0]);
        UnsafeUtility.Free(MyData, Allocator.TempJob);*/
        //NativeArray<int> Data = new NativeArray<int>(1, Allocator.TempJob);
    }

    void Update()
    {
        //RunNet();
        //RunNetNative();
        //RunJob();
        //RunParallelJob();
        //NativeListTest();
        //ParallelWrite();
        SortTest();

        /*OneJob Task = new OneJob();
        Task.Schedule(512, 64);*/
    }

    void RunNet()
    {
        float[] result = new float[1024];
        
        for(int i = 0; i < 32; i++)
        {
            for(int j = 0; j < result.Length; j++)
            {
                result[j] = 5;
            }
        }
    }

    void RunNetNative()
    {
        for(int i = 0; i < 32; i++)
        {
            for(int j = 0; j < Result.Length; j++)
            {
                Result[j] = 5;
            }
        }
    }
    
    void RunJob()
    {
        NativeArray<float> result = new NativeArray<float>(1024, Allocator.TempJob);
    
        TestJob jobData = new TestJob();
        jobData.value = 5;
        jobData.result = result;
        
        for(int i = 0; i < 32; i++)
        {
            JobHandle handle = jobData.Schedule();
            handle.Complete();
        }

        result.Dispose();
    }

    void RunParallelJob()
    {
        NativeArray<int> ParallelData = new NativeArray<int>(8192, Allocator.TempJob);
        for(int i = 0; i < 8192; i++) {
            ParallelData[i] = i;
        }

        NativeList<int> ParallelBuffer = new NativeList<int>(8192, Allocator.TempJob);

        ParallelTestJob ParallelJob = new ParallelTestJob();
        ParallelJob.ParallelData = ParallelData;
        ParallelJob.ParallelBuffer = ParallelBuffer;

        ParallelJob.Schedule(ParallelData.Length, 1).Complete();

        ParallelData.Dispose();
        ParallelBuffer.Dispose();
    }

    void NativeListTest()
    {
        /*NativeList<int> result = new NativeList<int>(5, Allocator.TempJob);
        result.Add(0);
        result.Add(1);
        result.Add(2);
        result.Add(3);
        result.Add(4);
        result.Add(5);
        result.Add(6);
        result.Add(7);

        result.RemoveAt(0);
        result.RemoveAt(2 - 1);
        result.RemoveAt(4 - 2);

        int data = result[4 - 3];

        result.Add(8);
        result.Add(9);*/

        NativeList<int> result = new NativeList<int>(5, Allocator.Temp);
        result.Resize(2, NativeArrayOptions.ClearMemory);

        result[0] = 0;
        result[1] = 1;

        result.Resize(3, NativeArrayOptions.ClearMemory);
        result[2] = 2;

        result.Dispose();
    }

    void NativeHashmapTest()
    {
        NativeHashMap<int, float> HashmapData = new NativeHashMap<int, float>(5, Allocator.TempJob);
        HashmapData.Add(0, 0.1f);
        HashmapData.Add(1, 0.2f);
        HashmapData.Add(2, 0.3f);
        HashmapData.Add(3, 0.4f);
        HashmapData.Add(4, 0.5f);
        HashmapData.Add(5, 0.6f);

        NativeArray<int> HashmapKey = HashmapData.GetKeyArray(Allocator.TempJob);

        /*for(int i = 0; i < HashmapKey.Length; i++)
        {
            int Key = HashmapKey[i];
            print(Hashmap[Key]);
        }*/

        ParallelHashmapJob ParallelJob = new ParallelHashmapJob();
        ParallelJob.HashmapKey = HashmapKey;
        ParallelJob.HashmapData = HashmapData;

        ParallelJob.Schedule(HashmapKey.Length, 1).Complete();

        HashmapData.Dispose();
        HashmapKey.Dispose();
    }

    void NativeMultiHashmapTest()
    {
        NativeMultiHashMap<int, float> MultiMap = new NativeMultiHashMap<int, float>(16, Allocator.TempJob);

        MultiMap.Add(0, 0.25f);
        MultiMap.Add(0, 0.25f);
        MultiMap.Add(0, 0.75f);
        MultiMap.Add(0, 0.99f);
        MultiMap.Add(1, 1.25f);
        MultiMap.Add(1, 1.5f);
        MultiMap.Add(1, 1.75f);
        MultiMap.Add(1, 1.99f);

        float OutData;
        int Count = MultiMap.Count();

        if (MultiMap.TryGetFirstValue(0, out OutData, out var iterator))
        {
            while (MultiMap.TryGetNextValue(out OutData, ref iterator))
            {
                print(OutData);
            }
        }

        MultiMap.Dispose();
    }

    unsafe void UnsafeStructTest()
    {
        PointerStruct* MyData = (PointerStruct*)UnsafeUtility.Malloc(sizeof(PointerStruct), 4, Allocator.Temp);
        MyData->Float = 150;

        print(MyData->Float);
        UnsafeUtility.Free(MyData, Allocator.Temp);
    }

    unsafe void UnsafeClassTest()
    {
        PointerStruct* MyData = (PointerStruct*)UnsafeUtility.Malloc(sizeof(PointerStruct), 64, Allocator.Temp);
        MyData->Float = 150;

        print(MyData->Float);
        UnsafeUtility.Free(MyData, Allocator.Temp);
    }

    unsafe void UnsafeArrayTest()
    {
        int* MyData = (int*)UnsafeUtility.Malloc(sizeof(int) * 5, 64, Allocator.Temp);
        MyData[0] = 1;
        MyData[1] = 2;
        MyData[2] = 3;
        MyData[3] = 4;
        MyData[4] = 5;

        UnsafeUtility.Free(MyData, Allocator.Temp);
    }

    void SortTest()
    {
        NativeArray<float> CopyData = new NativeArray<float>(Result.Length, Allocator.TempJob);

        ParallelCopy<float> CopyJob = new ParallelCopy<float>();
        CopyJob.InputArray = Result;
        CopyJob.OutputArray = CopyData;
        CopyJob.Schedule(Result.Length, 512).Complete();

        if (ParallelSort)
        {
            FSortFactory.ParallelSort(CopyData).Complete();
        } else {
            if (CustomSort)
            {
                new FSortFactory.FQuicksortJob<float>()
                {
                    array = CopyData,
                    left = 0,
                    right = CopyData.Length - 1
                }.Schedule().Complete();
            } else {
                SortJob sortJob;
                sortJob.array = CopyData;
                sortJob.Schedule().Complete();
            }
        }

        CopyData.Dispose();
    }

    void ParallelWrite()
    {
        NativeArray<int> ParallelData = new NativeArray<int>(512, Allocator.TempJob);
        for (int i = 0; i < 512; i++)
        {
            ParallelData[i] = i;
        }

        NativeList<int> ParallelBuffer = new NativeList<int>(512, Allocator.TempJob);

        FParallelListAddJob ParallelJob = new FParallelListAddJob();
        ParallelJob.ParallelData = ParallelData;
        ParallelJob.ParallelBuffer = ParallelBuffer.AsParallelWriter();

        ParallelJob.Schedule(ParallelData.Length, 64).Complete();

        ParallelData.Dispose();
        ParallelBuffer.Dispose();
    }

    void GCJobTest()
    {
        PointerClass PC = new PointerClass();
        GCHandle GCRef = GCHandle.Alloc(PC);

        PointerClassJob PCJob = new PointerClassJob();
        PCJob.GCRef = GCRef;
        PCJob.Schedule().Complete();

        GCRef.Free();
    }

    void OnDisable()
    {
        Result.Dispose();
    }
}
