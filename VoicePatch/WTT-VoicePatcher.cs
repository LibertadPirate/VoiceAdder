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

        public WTTVoicePatcher()
        {
            WTTVoicePatcher._targetType = Enumerable.Single<Type>(PatchConstants.EftTypes, new Func<Type, bool>(this.IsTargetType));
        }


        private bool IsTargetType(Type type)
        {
            return type.GetMethod("TakePhrasePath") != null;
        }


        protected override MethodBase GetTargetMethod()
        {
            return WTTVoicePatcher._targetType.GetMethod("TakePhrasePath");
        }


        [PatchPrefix]
        private static void PatchPrefix()
        {
            string voicesPath = Path.Combine(Environment.CurrentDirectory, "BepInEx", "plugins", "WTT-VoicePatcher");

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
                Dictionary<string, string> value = Traverse.Create(WTTVoicePatcher._targetType).Field<Dictionary<string, string>>("dictionary_0").Value;

                foreach (string key in dictionary.Keys)
                {
                    if (!value.ContainsKey(key))
                    {
                        value.Add(key, dictionary[key]);
                    }
                }
            }
        }

        private static Type _targetType;
    }
}
