using System.Collections.Generic;


namespace Infonet.CStoreCommander.BusinessLayer.Entities
{
    public class Sound
    {
        public Sound()
        {
            PumpSounds = new List<SoundInfo>();
            DeviceSounds = new List<SoundInfo>();
            SystemSounds = new List<SoundInfo>();
        }

        public List<SoundInfo> PumpSounds { get; set; }

        public List<SoundInfo> DeviceSounds { get; set; }

        public List<SoundInfo> SystemSounds { get; set; }

    }

    public class SoundInfo
    {
        public string Name { get; set; }

        public string File { get; set; }
    }
}
