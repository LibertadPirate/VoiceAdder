using BepInEx;

namespace WTT_VoicePatcher
{
    [BepInPlugin("com.wtt.voicepatcher", "WTT Voice Patcher", "1.0.1")]
    public class WTTVoiceAdd : BaseUnityPlugin
    {
        private void Awake()
        {
            new WTTVoicePatcher().Enable();
        }
    }
}