namespace GunshotWound2.Utils
{
    public struct MinMaxInt
    {
        public int Min;
        public int Max;

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