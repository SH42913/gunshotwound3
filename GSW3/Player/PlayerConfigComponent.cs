namespace GSW3.Player
{
    public class PlayerConfigComponent
    {
        public bool PlayerEnabled;

        public override string ToString()
        {
            return
                $"{nameof(PlayerConfigComponent)}: " +
                $"{nameof(PlayerEnabled)} {PlayerEnabled}; ";
        }
    }
}