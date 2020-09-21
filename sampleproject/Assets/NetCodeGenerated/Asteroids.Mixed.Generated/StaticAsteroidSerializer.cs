//THIS FILE IS AUTOGENERATED BY GHOSTCOMPILER. DON'T MODIFY OR ALTER.
using System;
using AOT;
using Unity.Burst;
using Unity.Networking.Transport;
using Unity.NetCode.LowLevel.Unsafe;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Collections;
using Unity.NetCode;
using Unity.Transforms;
using Unity.Mathematics;

namespace Asteroids.Mixed.Generated
{
    [BurstCompile]
    public struct StaticAsteroidGhostComponentSerializer
    {
        static StaticAsteroidGhostComponentSerializer()
        {
            State = new GhostComponentSerializer.State
            {
                GhostFieldsHash = 663308771019498107,
                ExcludeFromComponentCollectionHash = 0,
                ComponentType = ComponentType.ReadWrite<StaticAsteroid>(),
                ComponentSize = UnsafeUtility.SizeOf<StaticAsteroid>(),
                SnapshotSize = UnsafeUtility.SizeOf<Snapshot>(),
                ChangeMaskBits = ChangeMaskBits,
                SendMask = GhostComponentSerializer.SendMask.Interpolated | GhostComponentSerializer.SendMask.Predicted,
                SendForChildEntities = 1,
                CopyToSnapshot =
                    new PortableFunctionPointer<GhostComponentSerializer.CopyToFromSnapshotDelegate>(CopyToSnapshot),
                CopyFromSnapshot =
                    new PortableFunctionPointer<GhostComponentSerializer.CopyToFromSnapshotDelegate>(CopyFromSnapshot),
                RestoreFromBackup =
                    new PortableFunctionPointer<GhostComponentSerializer.RestoreFromBackupDelegate>(RestoreFromBackup),
                PredictDelta = new PortableFunctionPointer<GhostComponentSerializer.PredictDeltaDelegate>(PredictDelta),
                CalculateChangeMask =
                    new PortableFunctionPointer<GhostComponentSerializer.CalculateChangeMaskDelegate>(
                        CalculateChangeMask),
                Serialize = new PortableFunctionPointer<GhostComponentSerializer.SerializeDelegate>(Serialize),
                Deserialize = new PortableFunctionPointer<GhostComponentSerializer.DeserializeDelegate>(Deserialize),
                #if UNITY_EDITOR || DEVELOPMENT_BUILD
                ReportPredictionErrors = new PortableFunctionPointer<GhostComponentSerializer.ReportPredictionErrorsDelegate>(ReportPredictionErrors),
                #endif
            };
            #if UNITY_EDITOR || DEVELOPMENT_BUILD
            State.NumPredictionErrorNames = GetPredictionErrorNames(ref State.PredictionErrorNames);
            #endif
        }
        public static readonly GhostComponentSerializer.State State;
        public struct Snapshot
        {
            public int InitialPosition_x;
            public int InitialPosition_y;
            public int InitialVelocity_x;
            public int InitialVelocity_y;
            public int InitialAngle;
            public uint SpawnTick;
        }
        public const int ChangeMaskBits = 4;
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.CopyToFromSnapshotDelegate))]
        private static void CopyToSnapshot(IntPtr stateData, IntPtr snapshotData, int snapshotOffset, int snapshotStride, IntPtr componentData, int componentStride, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData, snapshotOffset + snapshotStride*i);
                ref var component = ref GhostComponentSerializer.TypeCast<StaticAsteroid>(componentData, componentStride*i);
                ref var serializerState = ref GhostComponentSerializer.TypeCast<GhostSerializerState>(stateData, 0);
                snapshot.InitialPosition_x = (int) math.round(component.InitialPosition.x * 100);
                snapshot.InitialPosition_y = (int) math.round(component.InitialPosition.y * 100);
                snapshot.InitialVelocity_x = (int) math.round(component.InitialVelocity.x * 100);
                snapshot.InitialVelocity_y = (int) math.round(component.InitialVelocity.y * 100);
                snapshot.InitialAngle = (int) math.round(component.InitialAngle * 100);
                snapshot.SpawnTick = (uint)component.SpawnTick;
            }
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.CopyToFromSnapshotDelegate))]
        private static void CopyFromSnapshot(IntPtr stateData, IntPtr snapshotData, int snapshotOffset, int snapshotStride, IntPtr componentData, int componentStride, int count)
        {
            for (int i = 0; i < count; ++i)
            {
                ref var snapshotInterpolationData = ref GhostComponentSerializer.TypeCast<SnapshotData.DataAtTick>(snapshotData, snapshotStride*i);
                ref var snapshotBefore = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotInterpolationData.SnapshotBefore, snapshotOffset);
                ref var snapshotAfter = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotInterpolationData.SnapshotAfter, snapshotOffset);
                float snapshotInterpolationFactor = snapshotInterpolationData.InterpolationFactor;
                ref var component = ref GhostComponentSerializer.TypeCast<StaticAsteroid>(componentData, componentStride*i);
                var deserializerState = GhostComponentSerializer.TypeCast<GhostDeserializerState>(stateData, 0);
                deserializerState.SnapshotTick = snapshotInterpolationData.Tick;
                component.InitialPosition = new float2(snapshotBefore.InitialPosition_x * 0.01f, snapshotBefore.InitialPosition_y * 0.01f);
                component.InitialVelocity = new float2(snapshotBefore.InitialVelocity_x * 0.01f, snapshotBefore.InitialVelocity_y * 0.01f);
                component.InitialAngle = snapshotBefore.InitialAngle * 0.01f;
                component.SpawnTick = (uint) snapshotBefore.SpawnTick;
            }
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.RestoreFromBackupDelegate))]
        private static void RestoreFromBackup(IntPtr componentData, IntPtr backupData)
        {
            ref var component = ref GhostComponentSerializer.TypeCast<StaticAsteroid>(componentData, 0);
            ref var backup = ref GhostComponentSerializer.TypeCast<StaticAsteroid>(backupData, 0);
            component.InitialPosition.x = backup.InitialPosition.x;
            component.InitialPosition.y = backup.InitialPosition.y;
            component.InitialVelocity.x = backup.InitialVelocity.x;
            component.InitialVelocity.y = backup.InitialVelocity.y;
            component.InitialAngle = backup.InitialAngle;
            component.SpawnTick = backup.SpawnTick;
        }

        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.PredictDeltaDelegate))]
        private static void PredictDelta(IntPtr snapshotData, IntPtr baseline1Data, IntPtr baseline2Data, ref GhostDeltaPredictor predictor)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline1 = ref GhostComponentSerializer.TypeCast<Snapshot>(baseline1Data);
            ref var baseline2 = ref GhostComponentSerializer.TypeCast<Snapshot>(baseline2Data);
            snapshot.InitialPosition_x = predictor.PredictInt(snapshot.InitialPosition_x, baseline1.InitialPosition_x, baseline2.InitialPosition_x);
            snapshot.InitialPosition_y = predictor.PredictInt(snapshot.InitialPosition_y, baseline1.InitialPosition_y, baseline2.InitialPosition_y);
            snapshot.InitialVelocity_x = predictor.PredictInt(snapshot.InitialVelocity_x, baseline1.InitialVelocity_x, baseline2.InitialVelocity_x);
            snapshot.InitialVelocity_y = predictor.PredictInt(snapshot.InitialVelocity_y, baseline1.InitialVelocity_y, baseline2.InitialVelocity_y);
            snapshot.InitialAngle = predictor.PredictInt(snapshot.InitialAngle, baseline1.InitialAngle, baseline2.InitialAngle);
            snapshot.SpawnTick = (uint)predictor.PredictInt((int)snapshot.SpawnTick, (int)baseline1.SpawnTick, (int)baseline2.SpawnTick);
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.CalculateChangeMaskDelegate))]
        private static void CalculateChangeMask(IntPtr snapshotData, IntPtr baselineData, IntPtr bits, int startOffset)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline = ref GhostComponentSerializer.TypeCast<Snapshot>(baselineData);
            uint changeMask;
            changeMask = (snapshot.InitialPosition_x != baseline.InitialPosition_x) ? 1u : 0;
            changeMask |= (snapshot.InitialPosition_y != baseline.InitialPosition_y) ? (1u<<0) : 0;
            changeMask |= (snapshot.InitialVelocity_x != baseline.InitialVelocity_x) ? (1u<<1) : 0;
            changeMask |= (snapshot.InitialVelocity_y != baseline.InitialVelocity_y) ? (1u<<1) : 0;
            changeMask |= (snapshot.InitialAngle != baseline.InitialAngle) ? (1u<<2) : 0;
            changeMask |= (snapshot.SpawnTick != baseline.SpawnTick) ? (1u<<3) : 0;
            GhostComponentSerializer.CopyToChangeMask(bits, changeMask, startOffset, 4);
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.SerializeDelegate))]
        private static void Serialize(IntPtr snapshotData, IntPtr baselineData, ref DataStreamWriter writer, ref NetworkCompressionModel compressionModel, IntPtr changeMaskData, int startOffset)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline = ref GhostComponentSerializer.TypeCast<Snapshot>(baselineData);
            uint changeMask = GhostComponentSerializer.CopyFromChangeMask(changeMaskData, startOffset, ChangeMaskBits);
            if ((changeMask & (1 << 0)) != 0)
                writer.WritePackedIntDelta(snapshot.InitialPosition_x, baseline.InitialPosition_x, compressionModel);
            if ((changeMask & (1 << 0)) != 0)
                writer.WritePackedIntDelta(snapshot.InitialPosition_y, baseline.InitialPosition_y, compressionModel);
            if ((changeMask & (1 << 1)) != 0)
                writer.WritePackedIntDelta(snapshot.InitialVelocity_x, baseline.InitialVelocity_x, compressionModel);
            if ((changeMask & (1 << 1)) != 0)
                writer.WritePackedIntDelta(snapshot.InitialVelocity_y, baseline.InitialVelocity_y, compressionModel);
            if ((changeMask & (1 << 2)) != 0)
                writer.WritePackedIntDelta(snapshot.InitialAngle, baseline.InitialAngle, compressionModel);
            if ((changeMask & (1 << 3)) != 0)
                writer.WritePackedUIntDelta(snapshot.SpawnTick, baseline.SpawnTick, compressionModel);
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.DeserializeDelegate))]
        private static void Deserialize(IntPtr snapshotData, IntPtr baselineData, ref DataStreamReader reader, ref NetworkCompressionModel compressionModel, IntPtr changeMaskData, int startOffset)
        {
            ref var snapshot = ref GhostComponentSerializer.TypeCast<Snapshot>(snapshotData);
            ref var baseline = ref GhostComponentSerializer.TypeCast<Snapshot>(baselineData);
            uint changeMask = GhostComponentSerializer.CopyFromChangeMask(changeMaskData, startOffset, ChangeMaskBits);
            if ((changeMask & (1 << 0)) != 0)
                snapshot.InitialPosition_x = reader.ReadPackedIntDelta(baseline.InitialPosition_x, compressionModel);
            else
                snapshot.InitialPosition_x = baseline.InitialPosition_x;
            if ((changeMask & (1 << 0)) != 0)
                snapshot.InitialPosition_y = reader.ReadPackedIntDelta(baseline.InitialPosition_y, compressionModel);
            else
                snapshot.InitialPosition_y = baseline.InitialPosition_y;
            if ((changeMask & (1 << 1)) != 0)
                snapshot.InitialVelocity_x = reader.ReadPackedIntDelta(baseline.InitialVelocity_x, compressionModel);
            else
                snapshot.InitialVelocity_x = baseline.InitialVelocity_x;
            if ((changeMask & (1 << 1)) != 0)
                snapshot.InitialVelocity_y = reader.ReadPackedIntDelta(baseline.InitialVelocity_y, compressionModel);
            else
                snapshot.InitialVelocity_y = baseline.InitialVelocity_y;
            if ((changeMask & (1 << 2)) != 0)
                snapshot.InitialAngle = reader.ReadPackedIntDelta(baseline.InitialAngle, compressionModel);
            else
                snapshot.InitialAngle = baseline.InitialAngle;
            if ((changeMask & (1 << 3)) != 0)
                snapshot.SpawnTick = reader.ReadPackedUIntDelta(baseline.SpawnTick, compressionModel);
            else
                snapshot.SpawnTick = baseline.SpawnTick;
        }
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        [BurstCompile]
        [MonoPInvokeCallback(typeof(GhostComponentSerializer.ReportPredictionErrorsDelegate))]
        private static void ReportPredictionErrors(IntPtr componentData, IntPtr backupData, ref UnsafeList<float> errors)
        {
            ref var component = ref GhostComponentSerializer.TypeCast<StaticAsteroid>(componentData, 0);
            ref var backup = ref GhostComponentSerializer.TypeCast<StaticAsteroid>(backupData, 0);
            int errorIndex = 0;
            errors[errorIndex] = math.max(errors[errorIndex], math.distance(component.InitialPosition, backup.InitialPosition));
            ++errorIndex;
            errors[errorIndex] = math.max(errors[errorIndex], math.distance(component.InitialVelocity, backup.InitialVelocity));
            ++errorIndex;
            errors[errorIndex] = math.max(errors[errorIndex], math.abs(component.InitialAngle - backup.InitialAngle));
            ++errorIndex;
            errors[errorIndex] = math.max(errors[errorIndex],
                (component.SpawnTick > backup.SpawnTick) ?
                (component.SpawnTick - backup.SpawnTick) :
                (backup.SpawnTick - component.SpawnTick));
            ++errorIndex;
        }
        private static int GetPredictionErrorNames(ref FixedString512 names)
        {
            int nameCount = 0;
            if (nameCount != 0)
                names.Append(new FixedString32(","));
            names.Append(new FixedString64("InitialPosition"));
            ++nameCount;
            if (nameCount != 0)
                names.Append(new FixedString32(","));
            names.Append(new FixedString64("InitialVelocity"));
            ++nameCount;
            if (nameCount != 0)
                names.Append(new FixedString32(","));
            names.Append(new FixedString64("InitialAngle"));
            ++nameCount;
            if (nameCount != 0)
                names.Append(new FixedString32(","));
            names.Append(new FixedString64("SpawnTick"));
            ++nameCount;
            return nameCount;
        }
        #endif
    }
}