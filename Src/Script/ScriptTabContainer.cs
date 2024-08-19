using System.Windows.Controls;
using System.Windows.Media;

namespace StuExec.Script;

public class ScriptTabContainer : TabControl
{
    private int MemoryIndex { get; set; } = 1;

    private int _lastIndex;
    public ScriptTab? LastTab => _lastIndex >= 0 && _lastIndex < Items.Count ? (ScriptTab?)Items[_lastIndex] : null;

    public ScriptTab? SelectedTab => (ScriptTab)SelectedItem;

    public EventHandler? SelectedTabChanged;
    public EventHandler? ScriptTabClosed;
    public EventHandler? ScriptTabAdded;

    public EventHandler? RequestedTabClose;

    protected override void OnSelectionChanged(SelectionChangedEventArgs e)
    {
        var tab = SelectedTab;

        if (tab != null) SelectedTabChanged?.Invoke(this, EventArgs.Empty);

        _lastIndex = SelectedIndex;

        base.OnSelectionChanged(e);
    }

    public ScriptTab? AddFileScript(string path)
    {
        var header = System.IO.Path.GetFileNameWithoutExtension(path);

        var isTabUnique = Items.Cast<ScriptTab>().All(tab => tab.Header != header || tab.FilePath != path);

        if (!isTabUnique) return null;

        var tab = new ScriptTab
        {
            Header = header,
            FilePath = path,
            Background = Resources["ColorBrush6"] as Brush
        };

        Items.Add(tab);
        SelectedItem = tab;

        ScriptTabAdded?.Invoke(this, EventArgs.Empty);

        return tab;
    }

    public ScriptTab AddMemoryScript(string script = "print(\"Hello RbxStu!\")")
    {
        var header = $"Script {MemoryIndex}";

        MemoryIndex++;

        var tab = new ScriptTab
        {
            Header = header,
            ScriptText = script,
            Background = Resources["ColorBrush6"] as Brush
        };

        Items.Add(tab);
        SelectedItem = tab;

        ScriptTabAdded?.Invoke(this, EventArgs.Empty);

        return tab;
    }
}