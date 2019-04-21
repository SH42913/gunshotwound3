namespace GSW3.Utils
{
    public struct MinMax
    {
        public float Min;
        public float Max;

        public bool IsDisabled()
        {
            return Min < 0 || Max < 0;
        }

        public override string ToString()
        {
            return Min + "-" + Max;
        }
    }
}