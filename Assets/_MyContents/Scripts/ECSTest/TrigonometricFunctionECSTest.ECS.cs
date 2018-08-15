namespace MainContents
{
    using UnityEngine;
    using UnityEngine.Assertions;

    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Rendering;
    using Unity.Transforms;
    using Unity.Burst;
    using Unity.Jobs;
    using Unity.Collections;

    public struct IndexData : IComponentData { public int Index; }

    // ----------------------------------------------------------------------
    public struct MathFunctionTestData : IComponentData { }
    public sealed class MathFunctionTesSystem : JobComponentSystem
    {
        [BurstCompile]
        struct Job : IJobProcessComponentData<MathFunctionTestData, IndexData>
        {
            // Jobで実行されるコード
            public void Execute(ref MathFunctionTestData data, ref IndexData index)
            {
                float rad = math.radians(index.Index);
                var s = math.sin(rad);
                var c = math.cos(rad);
            }
        }

        Job _job;

        protected override void OnCreateManager(int capacity)
        {
            base.OnCreateManager(capacity);
            this._job = new Job();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Restart();

            var handle = this._job.Schedule(this, 7, inputDeps);
            handle.Complete();

            sw.Stop();
            Debug.Log(" --- MathFunctionTesSystem Result : " + sw.Elapsed);
            return handle;
        }
    }



    // ----------------------------------------------------------------------
    public struct LUTTestData : IComponentData { }
    public sealed class LUTTestSystem : JobComponentSystem
    {
        [BurstCompile]
        struct Job : IJobProcessComponentData<LUTTestData, IndexData>
        {
            [ReadOnly] NativeArray<float> SinTable;

            public Job(NativeArray<float> sinTable)
            {
                this.SinTable = sinTable;
            }

            public void Execute(ref LUTTestData data, ref IndexData index)
            {
                var s = Sin(index.Index);
                var c = Cos(index.Index);
            }

            float Sin(int deg)
            {
                deg = deg % 360;
                // 0~90(度)
                if (deg <= 90) { return this.SinTable[deg]; }
                // 90~180
                else if (deg <= 180) { return this.SinTable[180 - deg]; }
                // 180~270
                else if (deg <= 270) { return -this.SinTable[deg - 180]; }
                // 270~360
                else { return -this.SinTable[360 - deg]; }
            }

            float Cos(int deg)
            {
                return Sin(deg + 90);
            }
        }

        NativeArray<float> SinTable;
        Job _job;

        protected override void OnCreateManager(int capacity)
        {
            base.OnCreateManager(capacity);
            const int Length = 90;
            this.SinTable = new NativeArray<float>(Length + 1, Allocator.Persistent);
            for (int i = 0; i < Length; ++i)
            {
                this.SinTable[i] = math.sin(math.radians(i));
            }
            this._job = new Job(this.SinTable);
        }

        protected override void OnDestroyManager()
        {
            if (this.SinTable.IsCreated) { this.SinTable.Dispose(); }
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Restart();

            var handle = this._job.Schedule(this, 7, inputDeps);
            handle.Complete();

            sw.Stop();
            Debug.Log(" --- LUTTestSystem Result : " + sw.Elapsed);
            return handle;
        }
    }



    // ----------------------------------------------------------------------
    public struct MaclaurinTestData : IComponentData { }
    public sealed class MaclaurinTestSystem : JobComponentSystem
    {
        [BurstCompile]
        struct Job : IJobProcessComponentData<MaclaurinTestData, IndexData>
        {
            public void Execute(ref MaclaurinTestData data, ref IndexData index)
            {
                var s = Sin(index.Index);
                var c = Cos(index.Index);
            }

            float Sin(int deg)
            {
                deg = deg % 360;
                int sign = 1;
                // 0~90(度)
                if (deg <= 90) { }
                // 90~180
                else if (deg <= 180) { deg = 180 - deg; }
                // 180~270
                else if (deg <= 270) { sign = -1; deg = deg - 180; }
                // 270~360
                else { sign = -1; deg = 360 - deg; }

                var rad = deg * Mathf.Deg2Rad;
                float pow2 = rad * rad;
                float pow3 = pow2 * rad;    // x^3
                float pow5 = pow3 * pow2;   // x^5
                float pow7 = pow5 * pow2;   // x^7
                float pow9 = pow7 * pow2;   // x^7

                // 階乗は算出コストを省くために数値リテラルで持つ
                float ret = rad - (pow3 / 6f)   // 3!
                            + (pow5 / 120f)     // 5!
                            - (pow7 / 5040f)    // 7!
                            + (pow9 / 362880f); // 9!
                return ret * sign;
            }

            float Cos(int deg)
            {
                return Sin(deg + 90);
            }
        }

        Job _job;

        protected override void OnCreateManager(int capacity)
        {
            base.OnCreateManager(capacity);
            this._job = new Job();
        }

        protected override JobHandle OnUpdate(JobHandle inputDeps)
        {
            var sw = new System.Diagnostics.Stopwatch();
            sw.Restart();

            var handle = this._job.Schedule(this, 7, inputDeps);
            handle.Complete();

            sw.Stop();
            Debug.Log(" --- MaclaurinTestSystem Result : " + sw.Elapsed);
            return handle;
        }
    }
}
