using System;
using System.Drawing;
using System.Globalization;
using System.Xml.Linq;
using Rage;
using Rage.Native;

namespace GunshotWound2.Utils
{
    public static class GswExtensions
    {
        public static bool IsTrueWithProbability(this Random random, double probability)
        {
            if (probability >= 1) return true;
            if (probability <= 0) return false;
            return random.NextDouble() < probability;
        }

        public static float NextFloat(this Random rand, float min, float max)
        {
            if (min > max)
            {
                throw new ArgumentException("Min must be less than Max");
            }

            return (float) rand.NextDouble() * (max - min) + min;
        }

        public static float NextMinMax(this Random rand, MinMax minMax)
        {
            return rand.NextFloat(minMax.Min, minMax.Max);
        }

        public static void ShowInGsw(this string text, float x, float y, float scale = 1f, Color color = new Color())
        {
            //NativeFunction.Natives.SetTextFont(0);
            //NativeFunction.Natives.SetTextDropShadow(2, 2, 0, 0, 0);
            //NativeFunction.Natives.SetTextEdge(1, 0, 0, 0, 205);
            NativeFunction.Natives.SET_TEXT_COLOUR((int) color.R, (int) color.G, (int) color.B, 255);
            NativeFunction.Natives.SET_TEXT_SCALE(scale, scale);
            NativeFunction.Natives.BEGIN_TEXT_COMMAND_DISPLAY_TEXT("STRING");
            NativeFunction.Natives.ADD_TEXT_COMPONENT_SUBSTRING_PLAYER_NAME(text);
            NativeFunction.Natives.END_TEXT_COMMAND_DISPLAY_TEXT(x, y);
        }

        public static XElement GetElement(this XElement root, string elementName)
        {
            XElement element = root.Element(elementName);
            if (element == null)
            {
                throw new Exception("Can't find " + elementName);
            }

            return element;
        }

        public static bool GetBool(this XElement node, string attributeName = "Value")
        {
            string value = string.IsNullOrEmpty(attributeName)
                ? node.Value
                : node.Attribute(attributeName).Value;

            return !string.IsNullOrEmpty(value) && bool.Parse(value);
        }

        public static int GetInt(this XElement node, string attributeName = "Value")
        {
            string value = string.IsNullOrEmpty(attributeName)
                ? node.Value
                : node.Attribute(attributeName).Value;

            return int.Parse(value);
        }

        public static float GetFloat(this XElement node, string attributeName = "Value")
        {
            string value = string.IsNullOrEmpty(attributeName)
                ? node.Value
                : node.Attribute(attributeName).Value;

            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        public static MinMax GetMinMax(this XElement node)
        {
            return new MinMax
            {
                Min = node.GetFloat("Min"),
                Max = node.GetFloat("Max")
            };
        }

        public static string Name(this Ped ped, int entity)
        {
            return $"{ped.Model.Name}({entity})";
        }

        public static void SetHealth(this Ped ped, float health)
        {
            ped.Health = (int) Math.Floor(health) + 100;
        }

        public static void SetMaxHealth(this Ped ped, float health)
        {
            ped.MaxHealth = (int) Math.Ceiling(health) + 101;
        }

        public static int GetHealth(this Ped ped)
        {
            return ped.Health - 100;
        }

        public static float GetDeltaTime()
        {
            return Game.Console.IsOpen 
                ? 0f 
                : Game.TimeScale * Game.FrameTime;
        }
    }
}