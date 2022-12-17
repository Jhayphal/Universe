#define LOG_TIME


namespace Universe.Simulator
{
    sealed class GpuMap
    {
        public GpuMap(int sourceIndex, int targetIndex, float force, float areaOfInfluence)
        {
            SourceIndex = sourceIndex;
            TargetIndex = targetIndex;
            Force = force;
            AreaOfInfluence = areaOfInfluence;
        }

        public readonly int SourceIndex;
        public readonly int TargetIndex;
        public readonly float Force;
        public readonly float AreaOfInfluence;

        public float[] ToArray() => new float[] { SourceIndex, TargetIndex, Force, AreaOfInfluence };
    }
}