using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SPT.Reflection.Patching;
using SPT.Reflection.Utils;
using HarmonyLib;
using Newtonsoft.Json;

namespace WTT_VoicePatcher
{
    public class WTTVoicePatcher : ModulePatch
    {
        private static Dictionary<string, string> customVoices = new Dictionary<string, string>();
        private static bool initialized = false;

        protected override MethodBase GetTargetMethod()
        {
            Type targetType = PatchConstants.EftTypes.Single(IsTargetType);
            return targetType.GetMethod("TakePhrasePath", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
        }

        private static bool IsTargetType(Type type)
        {
            return type.GetMethod("TakePhrasePath") != null;
        }

        [PatchPrefix]
        private static bool PatchPrefix(string name, ref string __result)
        {
            if (!initialized)
            {
                InitializeCustomVoices();
                initialized = true;
            }

            if (customVoices.TryGetValue(name, out string path))
            {
                __result = path;
                return false;
            }

            return true;
        }

        private static void InitializeCustomVoices()
        {
            string voicesPath = Path.Combine(Environment.CurrentDirectory, "BepInEx", "plugins", "WTT-VoicePatcher", "Voices");

            if (!Directory.Exists(voicesPath))
            {
                Console.WriteLine("Error: Voices directory not found.");
                return;
            }

            string[] jsonFiles = Directory.GetFiles(voicesPath, "*.json");

            if (jsonFiles.Length == 0)
            {
                Console.WriteLine("Error: No JSON files found in the voices directory.");
                return;
            }

            foreach (string jsonFile in jsonFiles)
            {
                string text = File.ReadAllText(jsonFile);
                Dictionary<string, string> dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(text);

                foreach (string key in dictionary.Keys)
                {
                    if (!customVoices.ContainsKey(key))
                    {
                        customVoices.Add(key, dictionary[key]);
                        Console.WriteLine($"Added custom voice {key} with path {dictionary[key]}");
                    }
                    else
                    {
                        Console.WriteLine($"Voice {key} already exists, skipping.");
                    }
                }
            }
        }
    }
}