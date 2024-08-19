using System.IO;
using System.Xml.Serialization;

namespace StuExec.Script;

public static class ScriptTabStorage
{
    private static readonly string FileName = "script_tabs.xml";
    private static readonly string FilePath = Environment.CurrentDirectory + @"\Data\" + FileName;

    public static ScriptTabs? LoadXml()
    {
        if (!File.Exists(FilePath)) return new ScriptTabs();

        var serializer = new XmlSerializer(typeof(ScriptTabs));
        using var reader = new StreamReader(FilePath);
        return (ScriptTabs?)serializer.Deserialize(reader);
    }

    public static void SaveXml(ScriptTabs tabs)
    {
        if (!Directory.Exists(Path.GetDirectoryName(FilePath)))
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));

        var serializer = new XmlSerializer(typeof(ScriptTabs));
        using var writer = new StreamWriter(FilePath);
        serializer.Serialize(writer, tabs);
    }

    [Serializable]
    public class Script()
    {
        [XmlAttribute("ScriptType")] public ScriptTabType Type { get; set; } = ScriptTabType.Memory;

        // File path or memory data depending on the type
        [XmlAttribute("ScriptData")] public string Data { get; set; } = "print(\"Hello RbxStu!\")";

        public Script(ScriptTabType type, string data) : this()
        {
            Type = type;
            Data = data;
        }
    }

    [XmlRoot("ScriptTabs")]
    public class ScriptTabs
    {
        [XmlArray("Tabs")]
        [XmlArrayItem("Tab")]
        public List<Script> Tabs { get; set; } = [];
    }

    public enum ScriptTabType
    {
        Memory,
        File
    }
}