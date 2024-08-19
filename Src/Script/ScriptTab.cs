using System.Windows.Controls;
using System.Windows.Media;

namespace StuExec.Script;

public class ScriptTab : TabItem
{
    private new ScriptTabContainer? Parent => (ScriptTabContainer)base.Parent;

    public new string Header
    {
        get => (string)base.Header;
        set => base.Header = value;
    }

    public string? ScriptText { get; set; }
    public string? FilePath { get; set; }

    public override void OnApplyTemplate()
    {
        var child = (Border)VisualTreeHelper.GetChild(this, 0);
        var closeButton = (Button?)child.FindName("CloseButton");

        if (closeButton != null) closeButton.Click += (_, _) => { BeforeClose(); };

        base.OnApplyTemplate();
    }

    private void BeforeClose()
    {
        if (Parent?.RequestedTabClose is null)
            Close();
        else
            Parent.RequestedTabClose?.Invoke(this, EventArgs.Empty);
    }

    public void Close()
    {
        var parent = Parent;
        if (parent?.Items.Count == 1) return;
        parent?.Items.Remove(this);
        parent?.ScriptTabClosed?.Invoke(this, EventArgs.Empty);
    }
}