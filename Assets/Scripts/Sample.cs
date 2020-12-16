using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEditor.VFX;
 
public class Sample : MonoBehaviour
{
    [SerializeField] 
    public Mesh mesh;

    [SerializeField]
    public Material material;
 
    private BatchRendererGroup _batchRendererGroup;
 
    // 毎フレーム、確実に前回のJobを終わらせるためのJobHandle
    private JobHandle _jobDependency;
 
    // XYZ軸それぞれの分割数
    private const int Split = 10;
 
    // インスタンスの総数
    private const int InstanceCount = Split * Split * Split;
 
    private int _batchIndex;
 
    private void OnEnable()
    {
        _batchRendererGroup = new BatchRendererGroup(CullingCallback);
        _batchIndex = _batchRendererGroup.AddBatch(
            mesh,
            0,
            material,
            0,
            ShadowCastingMode.Off,
            false,
            false,
            new Bounds(Vector3.zero, Vector3.one * float.MaxValue), // ここではすごく大きいBoundsを渡しておく
            InstanceCount,
            null,
            gameObject);
    }
 
    private void OnDisable()
    {
        _batchRendererGroup.Dispose();
    }
    
    private void Update()
    {
        // Matrixをいじる前に前回のJobを終了しておく
        _jobDependency.Complete();
 
        // Matrixを更新
        _jobDependency = new UpdateMatrixJob
        {
            Matrices = _batchRendererGroup.GetBatchMatrices(_batchIndex),
            Time = Time.time
        }.Schedule(InstanceCount, 16);
    }
 
    private JobHandle CullingCallback(BatchRendererGroup rendererGroup, BatchCullingContext cullingContext)
    {
        var inputDependency = _jobDependency;
        for (var i = 0; i < cullingContext.batchVisibility.Length; ++i)
        {
            var job = new CullingJob
            {
                CullingContext = cullingContext,
                Matrices = rendererGroup.GetBatchMatrices(i),
                BatchIndex = i
            }.Schedule(inputDependency);
 
            // Jobの依存関係を更新
            _jobDependency = JobHandle.CombineDependencies(job, _jobDependency);
        }
            
        return _jobDependency;
    }
 
    // Culling Job
    private struct CullingJob : IJob
    {
        public BatchCullingContext CullingContext;
        public NativeArray<Matrix4x4> Matrices;
        public int BatchIndex;
 
        public void Execute()
        {
            var batchVisibility = CullingContext.batchVisibility[BatchIndex];
 
            var visibleCount = 0;
            for (var i = 0; i < batchVisibility.instancesCount; ++i)
            {
                if (!Contains(CullingContext.cullingPlanes, Matrices[i])) continue;
                CullingContext.visibleIndices[visibleCount] = batchVisibility.offset + i;
                ++visibleCount;
            }
 
            batchVisibility.visibleCount = visibleCount;
            CullingContext.batchVisibility[BatchIndex] = batchVisibility;
        }
        
        // 全てのPlaneの内側に各Meshの原点があったらtrueを返す
        // 実際は原点ではなく、メッシュを覆うAABBなどでカリングを行いたい
        private bool Contains(NativeArray<Plane> planes, Matrix4x4 matrix)
        {
            for (var i = 0; i < planes.Length; ++i)
            {
                if (!planes[i].GetSide(matrix.MultiplyPoint(Vector3.zero)))
                {
                    return false;
                }
            }
 
            return true;
        }
    }
 
    // Matrixを更新するJob
    [BurstCompile]
    private struct UpdateMatrixJob : IJobParallelFor
    {
        public NativeArray<Matrix4x4> Matrices;
        public float Time;
 
        public void Execute(int index)
        {
            var id = new Vector3(index / Split / Split, index / Split % Split, index % Split);
            Matrices[index] = Matrix4x4.TRS(id * 5,
                quaternion.EulerXYZ(id + Vector3.one * Time), 
                Vector3.one);
        }
    }
}
