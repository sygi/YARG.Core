using System.IO;
using YARG.Core.Extensions;
using YARG.Core.IO;

namespace YARG.Core.Engine.Drums
{
    public class DrumsEngineParameters : BaseEngineParameters
    {
        public enum DrumMode : byte
        {
            NonProFourLane,
            ProFourLane,
            FiveLane,
            Elite
        }

        /// <summary>
        /// What mode the inputs should be processed in.
        /// </summary>
        public readonly DrumMode Mode;

        //Ghost notes are below this threshold, Accent notes are above 1 - threshold
        public readonly float VelocityThreshold;

        // The maximum allowed time (seconds) between notes to use context-sensitive velocity scoring
        public readonly float SituationalVelocityWindow;

        // Whether or not we can earn more star power while its already active
        public readonly bool NoStarPowerOverlap;

        // Whether or not overhitting (hitting a pad with no matching note) is penalized
        // (combo reset, rock meter damage, overhit stat). When false, such inputs are ignored.
        public readonly bool PenalizeOverhit;

        public DrumsEngineParameters(HitWindowSettings hitWindow, int maxMultiplier, float[] starMultiplierThresholds, float[] soloBonusStarMultiplierThresholds,
            DrumMode mode, bool noStarPowerOverlap, bool penalizeOverhit, bool enableLanes)
            : base(hitWindow, maxMultiplier, 0, 0, starMultiplierThresholds, soloBonusStarMultiplierThresholds, enableLanes)
        {
            Mode = mode;
            VelocityThreshold = 0.35f;
            SituationalVelocityWindow = 1.5f;
            NoStarPowerOverlap = noStarPowerOverlap;
            PenalizeOverhit = penalizeOverhit;
        }

        public DrumsEngineParameters(ref FixedArrayStream stream, int version)
            : base(ref stream, version)
        {
            Mode = (DrumMode) stream.ReadByte();
            VelocityThreshold = stream.Read<float>(Endianness.Little);
            SituationalVelocityWindow = stream.Read<float>(Endianness.Little);
            if (version >= 9) {
                NoStarPowerOverlap = stream.ReadBoolean();
            }
            if (version >= 19) {
                PenalizeOverhit = stream.ReadBoolean();
            }
            else {
                PenalizeOverhit = true;
            }
        }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write((byte) Mode);
            writer.Write(VelocityThreshold);
            writer.Write(SituationalVelocityWindow);
            writer.Write(NoStarPowerOverlap);
            writer.Write(PenalizeOverhit);
        }

        public override string ToString()
        {
            return
                $"{base.ToString()}\n" +
                $"Velocity threshold: {VelocityThreshold}\n" +
                $"Situational velocity window: {SituationalVelocityWindow}\n" +
                $"No star power overlap: {NoStarPowerOverlap}\n" +
                $"Penalize overhit: {PenalizeOverhit}";
        }
    }
}