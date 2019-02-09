namespace GunshotWound2.Effects.MovementClipset
{
    public class NewMovementClipsetComponent
    {
        public string Player;
        public string PedMale;
        public string PedFemale;

        public override string ToString()
        {
            return $"{nameof(NewMovementClipsetComponent)}: " +
                   $"{nameof(Player)} {Player}; " +
                   $"{nameof(PedMale)} {PedMale}; " +
                   $"{nameof(PedFemale)} {PedFemale}; ";
        }
    }
}