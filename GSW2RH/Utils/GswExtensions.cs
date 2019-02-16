using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Xml.Linq;
using GunshotWound2.GswWorld;
using GunshotWound2.Localization;
using Leopotam.Ecs;
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

        public static int NextMinMax(this Random rand, MinMaxInt minMax)
        {
            return rand.Next(minMax.Min, minMax.Max);
        }

        public static T NextFromArray<T>(this Random random, T[] array)
        {
            int index = random.Next(0, array.Length);
            return array[index];
        }

        public static T NextFromList<T>(this Random random, List<T> list)
        {
            int index = random.Next(0, list.Count);
            return list[index];
        }

        public static T NextEnum<T>(this Random random) where T : Enum
        {
            var array = (T[]) Enum.GetValues(typeof(T));
            return random.NextFromArray(array);
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
                throw new Exception($"Can't find element {elementName} in {root.Parent?.Name}/{root.Name.LocalName}");
            }

            return element;
        }

        public static string GetAttributeValue(this XElement node, string attributeName)
        {
            XAttribute attribute = node.Attribute(attributeName);
            if (attribute == null)
            {
                throw new Exception($"Can't find attribute {attributeName} in " +
                                    $"{node.Parent?.Name}/{node.Name.LocalName}");
            }

            return attribute.Value;
        }

        public static bool GetBool(this XElement node, string attributeName = "Value")
        {
            string valueString = string.IsNullOrEmpty(attributeName)
                ? node.Value
                : node.GetAttributeValue(attributeName);

            return !string.IsNullOrEmpty(valueString) && bool.Parse(valueString);
        }

        public static int GetInt(this XElement node, string attributeName = "Value")
        {
            string valueString = string.IsNullOrEmpty(attributeName)
                ? node.Value
                : node.GetAttributeValue(attributeName);

            return int.Parse(valueString);
        }

        public static long GetLong(this XElement node, string attributeName = "Value")
        {
            string valueString = string.IsNullOrEmpty(attributeName)
                ? node.Value
                : node.GetAttributeValue(attributeName);

            return long.Parse(valueString);
        }

        public static float GetFloat(this XElement node, string attributeName = "Value")
        {
            string valueString = string.IsNullOrEmpty(attributeName)
                ? node.Value
                : node.GetAttributeValue(attributeName);

            return float.Parse(valueString, CultureInfo.InvariantCulture);
        }

        public static T GetEnum<T>(this XElement node, string attributeName = "Value") where T : Enum
        {
            string valueString = string.IsNullOrEmpty(attributeName)
                ? node.Value
                : node.GetAttributeValue(attributeName);

            return (T) Enum.Parse(typeof(T), valueString);
        }

        public static MinMax GetMinMax(this XElement node)
        {
            return new MinMax
            {
                Min = node.GetFloat("Min"),
                Max = node.GetFloat("Max")
            };
        }

        public static MinMaxInt GetMinMaxInt(this XElement node)
        {
            return new MinMaxInt
            {
                Min = node.GetInt("Min"),
                Max = node.GetInt("Max")
            };
        }

        public static string GetEntityName(this int entity)
        {
            EcsWorld ecsWorld = EcsWorld.Active;
            var localizationKey = ecsWorld.GetComponent<LocalizationKeyComponent>(entity);
            if (localizationKey != null) return $"{localizationKey.Key}({entity})";

            var gswPed = ecsWorld.GetComponent<GswPedComponent>(entity);
            if (gswPed != null) return gswPed.ThisPed.Name(entity);

            return $"Entity #{entity}";
        }

        public static string Name(this Ped ped, int entity)
        {
            string name = ped.Exists()
                ? ped.Model.Name
                : "NOT_EXISTS";
            return $"{name}({entity})";
        }

        public static void SetHealth(this Ped ped, float health)
        {
            int finalHealth = (int) Math.Floor(health) + 100;
            ped.Health = finalHealth;
        }

        public static void SetMaxHealth(this Ped ped, float health)
        {
            ped.MaxHealth = (int) Math.Ceiling(health) + 101;
        }

        public static float GetHealth(this Ped ped)
        {
            return (float) ped.Health - 100;
        }

        public static float GetDeltaTime()
        {
            return Game.Console.IsOpen
                ? 0f
                : Game.TimeScale * Game.FrameTime;
        }
    }
}