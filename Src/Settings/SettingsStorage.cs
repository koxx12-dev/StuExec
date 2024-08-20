using System.IO;
using System.Xml.Serialization;

namespace StuExec.Settings;

public class SettingsStorage
{
    private static readonly string FileName = "settings.xml";
    private static readonly string FilePath = Environment.CurrentDirectory + @"\Data\" + FileName;
    
    public static Settings? LoadXml()
    {
        if (!File.Exists(FilePath)) return new Settings();
        
        var serializer = new XmlSerializer(typeof(Settings));
        using var reader = new StreamReader(FilePath);
        return (Settings?)serializer.Deserialize(reader);
    }
    
    public static void SaveXml(Settings settings)
    {
        if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
        
        var serializer = new XmlSerializer(typeof(Settings));
        using var writer = new StreamWriter(FilePath);
        serializer.Serialize(writer, settings);
    }

    [XmlRoot("Settings")]
    public class Settings
    {
        [XmlElement("UpdateSettings")] public UpdateSettings UpdateSettings { get; set; } = new();
    }
    
    public class UpdateSettings
    {
        [XmlAttribute("UpdateNotifications")] public bool StuExecUpdateNotif { get; set; } = true;
        [XmlAttribute("DllAutoUpdate")] public bool RbxStuAutoUpdate { get; set; } = true;
        [XmlAttribute("DllAutoUpdateLastUsedTag")] public string? RbxStuAutoUpdateLastTag { get; set; }
    }
}