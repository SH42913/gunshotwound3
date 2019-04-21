using System;
using Rage;

namespace GSW3.Utils
{
    public class GswLogger
    {
        private const string MAIN_KEY = "GSW3.";
        private readonly string _className;

        public GswLogger(Type targetType)
        {
            _className = targetType.Name;
        }

        public void MakeLog(string log)
        {
            Game.Console.Print(MAIN_KEY + _className + ": " + log);
        }
    }
}