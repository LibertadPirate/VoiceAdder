using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

[BepInPlugin("com.wtt.voiceadder", "VoiceAdder", "1.0.2")] // Update version for 4.0
public class VoiceAdder : BaseUnityPlugin
{
    private static readonly ManualLogSource Logger = BepInEx.Logging.Logger.CreateLogSource("VoiceAdder");

    private void Awake()
    {
        new Harmony("com.wtt.voiceadder").PatchAll();
        Logger.LogInfo("VoiceAdder loaded for SPT 4.0");
        LoadCustomVoices();
    }

    private void LoadCustomVoices()
    {
        string voiceDir = Path.Combine(BepInEx.Paths.PluginPath, "WTT-VoicePatcher");
        if (!Directory.Exists(voiceDir))
        {
            Logger.LogWarning("VoicePatcher directory not found!");
            return;
        }

        foreach (string jsonFile in Directory.GetFiles(voiceDir, "*.json"))
        {
            string json = File.ReadAllText(jsonFile);
            VoiceData voiceData = JsonConvert.DeserializeObject<VoiceData>(json);
            if (voiceData != null)
            {
                InjectVoiceToAssembly(voiceData);
            }
        }
    }

    private void InjectVoiceToAssembly(VoiceData data)
    {
        // Patch or reflect into the voice database (e.g., VoiceManager or GlobalVoiceStorage)
        // Example: Use reflection to add to Dictionary<string, VoiceLine[]>
        var voiceStorage = AccessTools.TypeByName("EFT.VoiceStorage"); // Update type name if changed in 4.0
        var dict = (Dictionary<string, List<VoiceLine>>)AccessTools.Field(voiceStorage, "VoiceLines").GetValue(null);
        // Add data.VoiceLines to dict[data.Id]
        Logger.LogInfo($"Injected voice: {data.Id}");
    }
}

public class VoiceData
{
    public string Id { get; set; }
    public List<VoiceLine> VoiceLines { get; set; } // Phrases for mumble, fight, etc.
}

public class VoiceLine
{
    public string PhraseId { get; set; }
    public string AudioPath { get; set; }
    // Add new fields for SPT 4.0 audio (e.g., SpatialParams if needed)
}

// Harmony patch example for loading hook
[HarmonyPatch(typeof(GInterface29), "LoadVoices")] // Update method if signature changed
public class VoiceLoadPatch
{
    static void Postfix()
    {
        // Trigger custom load here if not done in Awake
    }
}