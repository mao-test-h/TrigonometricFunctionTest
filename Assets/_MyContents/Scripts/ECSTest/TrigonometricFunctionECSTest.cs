namespace MainContents
{
    using UnityEngine;

    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Rendering;
    using Unity.Transforms;
    using Unity.Burst;
    using Unity.Jobs;
    using Unity.Collections;

    /// <summary>
    /// PureECSで回すテスト
    /// </summary>
    public sealed class TrigonometricFunctionECSTest : MonoBehaviour
    {
        const int Count = 100000;

        void Start()
        {
            var entityManager = World.Active.GetOrCreateManager<EntityManager>();

            var archetype = entityManager.CreateArchetype(
                //typeof(MathFunctionTestData),
                //typeof(LUTTestData),
                typeof(MaclaurinTestData),
                typeof(IndexData));

            for(int i = 0; i < Count; ++i)
            {
                var entity = entityManager.CreateEntity(archetype);
                entityManager.SetComponentData(entity, new IndexData { Index = i });
            }
        }
    }
}