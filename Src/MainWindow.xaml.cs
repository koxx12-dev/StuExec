using System.CodeDom;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using StuExec.Script;
using Path = System.IO.Path;

namespace StuExec;

public partial class MainWindow
{
    // ReSharper disable once NotAccessedField.Local - i need to keep a reference to the watcher so it doesn't get garbage collected
    private FileSystemWatcher? _watcher;
    private DispatcherTimer? _pipeTimer;
    private NamedPipeClientStream? _pipeClient;
    private StreamWriter? _pipeWriter;
    private int _lastProcessPid = -1;

    public MainWindow()
    {
        InitializeComponent();
        InitializeScriptList();
        InitializeEditor();
        InitializePipe();
    }

    private void InitializeScriptList()
    {
        var scriptFolder = @$"{Environment.CurrentDirectory}\scripts";
        var autoExecFolder = @$"{Environment.CurrentDirectory}\autoexec";

        if (!Directory.Exists(scriptFolder)) Directory.CreateDirectory(scriptFolder);
        if (!Directory.Exists(autoExecFolder)) Directory.CreateDirectory(autoExecFolder);

        var watcher = new FileSystemWatcher();
        watcher.Path = scriptFolder;
        watcher.Filter = "*.*";
        watcher.EnableRaisingEvents = true;
        watcher.Renamed += OnScriptList_Renamed;
        watcher.Created += OnScriptList_Created;
        watcher.Deleted += OnScriptList_Deleted;

        _watcher = watcher;

        var files = Directory.GetFiles(scriptFolder, "*.*", SearchOption.TopDirectoryOnly)
            .Select(f => f.Replace(scriptFolder, "").TrimStart('\\'))
            .ToArray();

        foreach (var file in files)
        {
            if (!file.EndsWith(".lua") && !file.EndsWith(".luau") && !file.EndsWith(".txt")) continue;

            ScriptList.Items.Add(file);
        }
    }

    private void InitializeEditor()
    {
        var storage = ScriptTabStorage.LoadXml();

        if (storage == null || storage.Tabs.Count == 0)
            ScriptTabContainer.AddMemoryScript();
        else
            foreach (var tab in storage.Tabs)
                if (tab.Type == ScriptTabStorage.ScriptTabType.File)
                    ScriptTabContainer.AddFileScript(tab.Data);
                else
                    ScriptTabContainer.AddMemoryScript(tab.Data);

        ScriptEditor.Source = new Uri(@$"{AppDomain.CurrentDomain.BaseDirectory}\Monaco\index.html");

        ScriptEditor.CoreWebView2InitializationCompleted += (_, _) =>
        {
            ScriptEditor.CoreWebView2.WebMessageReceived += async (__, e) =>
            {
                var message = e.TryGetWebMessageAsString();
                var currentTab = ScriptTabContainer.SelectedTab;

                if (message == null || currentTab == null) return;

                switch (message)
                {
                    case "Save":

                        if (currentTab.FilePath == null) goto case "SaveAs";

                    {
                        var script = await GetEditorScript();
                        if (script == null) return;


                        _ = File.WriteAllTextAsync(currentTab.FilePath, script);

                        break;
                    }
                    case "SaveAs":
                        var dialog = new Microsoft.Win32.SaveFileDialog
                        {
                            Filter = "Luau Script|*.luau|Lua Script|*.lua|Text File|*.txt",
                            Title = "Save Script",
                            DefaultDirectory = @$"{Environment.CurrentDirectory}\scripts",
                            DefaultExt = ".luau"
                        };

                        dialog.ShowDialog();

                        if (dialog.FileName != "")
                        {
                            var script = await GetEditorScript();
                            if (script == null) return;

                            _ = File.WriteAllTextAsync(dialog.FileName, script);

                            if (Path.GetDirectoryName(dialog.FileName) != @$"{Environment.CurrentDirectory}\scripts")
                                return;

                            currentTab.ScriptText = null;
                            currentTab.Header = Path.GetFileNameWithoutExtension(dialog.FileName);
                            currentTab.FilePath = dialog.FileName;
                        }

                        break;
                    case "Execute":
                        ExecuteButton_OnClick(null!, null!);
                        break;
                    default:
                        throw new InvalidEnumArgumentException($"{message} is not a valid message.");
                }
            };
        };

        ScriptEditor.NavigationCompleted += (_, _) => { ScriptPanel.Visibility = Visibility.Visible; };

        // ReSharper disable once AsyncVoidLambda
        ScriptTabContainer.RequestedTabClose += async (current, __) =>
        {
            if (current is not ScriptTab tab) return;

            if (tab.FilePath != null)
            {
                var script = await GetEditorScript();

                if (script == null) return;

                _ = File.WriteAllTextAsync(tab.FilePath, script);
            }

            tab.Close();
        };

        // ReSharper disable once AsyncVoidLambda
        ScriptTabContainer.SelectedTabChanged += async (_, _) =>
        {
            var lastTab = ScriptTabContainer.LastTab;
            var currentTab = ScriptTabContainer.SelectedTab;

            if (lastTab != null)
            {
                var script = await GetEditorScript();

                if (script == null) return;

                if (lastTab.FilePath != null)
                    _ = File.WriteAllTextAsync(lastTab.FilePath, script);
                else
                    lastTab.ScriptText = script;
            }

            if (currentTab == null) return;

            if (currentTab.FilePath != null)
            {
                var script = await File.ReadAllTextAsync(currentTab.FilePath);
                SetEditorScript(script);
            }
            else
            {
                SetEditorScript(currentTab.ScriptText!);
            }
        };
    }

    private void InitializePipe()
    {
        _pipeTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(1)
        };

        _pipeTimer.Tick += (_, _) =>
        {
            if (_pipeClient is { IsConnected: true })
                if (_lastProcessPid != -1 && Process.GetProcesses().All(p => p.Id != _lastProcessPid))
                {
                    _pipeWriter = null;
                    _pipeClient?.Dispose();
                    _pipeClient = null;
                }

            if (_pipeClient is { IsConnected: true })
                StatusLabel.Content = "Status: Connected";
            else if (Process.GetProcessesByName("RobloxStudioBeta").Length > 0)
                StatusLabel.Content = "Status: Disconnected";
            else
                StatusLabel.Content = "Status: No Studio process found";
        };

        _pipeTimer.Start();
    }

    private void OnScriptList_Deleted(object sender, FileSystemEventArgs e)
    {
        if (e.Name == null) return;

        if (!e.Name.EndsWith(".lua") && !e.Name.EndsWith(".luau") && !e.Name.EndsWith(".txt")) return;

        Dispatcher.Invoke(() => { ScriptList.Items.Remove(e.Name); });
    }

    private void OnScriptList_Created(object sender, FileSystemEventArgs e)
    {
        if (e.Name == null) return;

        if (!e.Name.EndsWith(".lua") && !e.Name.EndsWith(".luau") && !e.Name.EndsWith(".txt")) return;

        Dispatcher.Invoke(() => { ScriptList.Items.Add(e.Name); });
    }

    private void OnScriptList_Renamed(object sender, RenamedEventArgs e)
    {
        if (e.Name == null) return;

        if (!e.Name.EndsWith(".lua") && !e.Name.EndsWith(".luau") && !e.Name.EndsWith(".txt")) return;

        Dispatcher.Invoke(() =>
        {
            ScriptList.Items.Remove(e.OldName);
            ScriptList.Items.Add(e.Name);
        });
    }

    private void InjectAndOpenPipe()
    {
        var robloxStudioProcesses = Process.GetProcessesByName("RobloxStudioBeta");
        var rbxStuDll = Path.Combine(Environment.CurrentDirectory, App.RbxStuDll);

        if (DoesPipeExist() && _pipeClient is not { IsConnected: true })
        {
            var processWithModule = robloxStudioProcesses.FirstOrDefault(p =>
                p.Modules.Cast<ProcessModule>().Any(m => m.ModuleName == App.RbxStuDll));

            if (processWithModule == null) return;

            _lastProcessPid = processWithModule.Id;
            ConnectToPipe(100);
            return;
        }

        if (!File.Exists(rbxStuDll))
        {
            MessageBox.Show(
                $"Failed to find \"{App.RbxStuDll}\".\n" +
                "Please make sure it's in the same directory as the executable."
            );
            return;
        }

        var process = robloxStudioProcesses.FirstOrDefault();

        if (process == null)
        {
            MessageBox.Show("Failed to find Roblox Studio process.");
            return;
        }

        try
        {
            Injector.Inject(process.Id.ToString(), rbxStuDll);
            ConnectToPipe();

            _lastProcessPid = process.Id;
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message);
        }
    }

    private void ConnectToPipe(int timeout = 30000)
    {
        if (_pipeClient is { IsConnected: true }) return;

        _pipeClient?.Dispose();

        _pipeClient = new NamedPipeClientStream(".", App.RbxStuPipe, PipeDirection.Out);

        try
        {
            _pipeClient.Connect(timeout);
            _pipeWriter = new StreamWriter(_pipeClient, System.Text.Encoding.Default, 1024 * 1024 /* 1MiB */);
            
            var autoExecFolder = @$"{Environment.CurrentDirectory}\autoexec";
            if (!Directory.Exists(autoExecFolder)) Directory.CreateDirectory(autoExecFolder);
            
            var files = Directory.GetFiles(autoExecFolder, "*.*", SearchOption.TopDirectoryOnly)
                .Where(f => f.EndsWith(".lua") || f.EndsWith(".luau") || f.EndsWith(".txt"))
                .ToArray();

            foreach (var file in files)
            {
                var script = File.ReadAllText(file);
                SendScriptToPipe(script);
            }
        }
        catch (Exception e)
        {
            if (e is TimeoutException)
            {
                Console.WriteLine("Failed to connect to the pipe.");
                return;
            }

            MessageBox.Show(e.Message);
        }
    }

    private void SendScriptToPipe(string script)
    {
        if (_pipeClient is not { IsConnected: true }) return;

        try
        {
            _pipeWriter?.Write(script);
            _pipeWriter?.Flush();
        }
        catch (Exception e)
        {
            if (_pipeClient is { IsConnected: false })
            {
                _pipeWriter = null;
                _pipeClient?.Dispose();
                _pipeClient = null;
                return;
            }

            MessageBox.Show(e.Message);
        }
    }

    private bool DoesPipeExist()
    {
        return Directory.GetFiles(@"\\.\pipe\").Contains(@$"\\.\pipe\{App.RbxStuPipe}");
    }


    private void MinimizeButton_Click(object sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        CloseWindow();
    }

    private void InjectButton_OnClick(object sender, RoutedEventArgs e)
    {
        new Thread(InjectAndOpenPipe).Start();
    }

    private async void ExecuteButton_OnClick(object sender, RoutedEventArgs e)
    {
        var script = await GetEditorScript();

        if (script == null) return;

        SendScriptToPipe(script);
    }

    private void ExecuteFile_Click(object sender, RoutedEventArgs e)
    {
        var scriptFolder = @$"{Environment.CurrentDirectory}\scripts";
        var scriptIndex = ScriptList.SelectedIndex;

        if (scriptIndex == -1) return;

        if (ScriptList.Items.GetItemAt(scriptIndex) is not string scriptName) return;

        var scriptPath = Path.Combine(scriptFolder, scriptName);

        if (!File.Exists(scriptPath)) return;

        var script = File.ReadAllText(scriptPath);

        SendScriptToPipe(script);
    }

    private void LoadFileIntoCurrentEditor_Click(object sender, RoutedEventArgs e)
    {
        var scriptFolder = @$"{Environment.CurrentDirectory}\scripts";
        var scriptIndex = ScriptList.SelectedIndex;

        if (scriptIndex == -1) return;

        if (ScriptList.Items.GetItemAt(scriptIndex) is not string scriptName) return;

        var scriptPath = Path.Combine(scriptFolder, scriptName);

        if (!File.Exists(scriptPath)) return;

        var script = File.ReadAllText(scriptPath);
        
        SetEditorScript(script);
    }

    private void LoadFileInteoNewEditor_Click(object sender, RoutedEventArgs e)
    {
        var scriptFolder = @$"{Environment.CurrentDirectory}\scripts";
        var scriptIndex = ScriptList.SelectedIndex;

        if (scriptIndex == -1) return;

        if (ScriptList.Items.GetItemAt(scriptIndex) is not string scriptName) return;

        var scriptPath = Path.Combine(scriptFolder, scriptName);

        if (!File.Exists(scriptPath)) return;

        ScriptTabContainer.AddFileScript(scriptPath);
    }

    private void MainWindow_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        base.OnMouseLeftButtonDown(e);

        DragMove();
    }

    private void AddScriptButton_OnClick(object sender, RoutedEventArgs e)
    {
        ScriptTabContainer.AddMemoryScript();
    }

    private async void CloseWindow()
    {
        var currentTab = ScriptTabContainer.SelectedTab;

        if (currentTab != null)
        {
            var script = await GetEditorScript();

            if (script == null) return;

            if (currentTab.FilePath != null)
                _ = File.WriteAllTextAsync(currentTab.FilePath, script);
            else
                currentTab.ScriptText = script;
        }

        var storage = new ScriptTabStorage.ScriptTabs();

        foreach (var tab in ScriptTabContainer.Items)
        {
            if (tab is not ScriptTab scriptTab) continue;

            if (scriptTab.FilePath != null)
                storage.Tabs.Add(new ScriptTabStorage.Script(ScriptTabStorage.ScriptTabType.File, scriptTab.FilePath));
            else if (scriptTab.ScriptText != null)
                storage.Tabs.Add(new ScriptTabStorage.Script(ScriptTabStorage.ScriptTabType.Memory,
                    scriptTab.ScriptText));
        }

        ScriptTabStorage.SaveXml(storage);

        Close();
    }
    
    //https://stackoverflow.com/questions/323640/can-i-convert-a-c-sharp-string-value-to-an-escaped-string-literal#324812 -- no point in adding a new library for one function
    private static string ToLiteral(string input)
    {
        using (var writer = new StringWriter())
        {
            using (var provider = CodeDomProvider.CreateProvider("CSharp"))
            {
                provider.GenerateCodeFromExpression(new CodePrimitiveExpression(input), writer, null);
                return writer.ToString();
            }
        }
    }

    private async Task<string?> GetEditorScript()
    {
        var script = await ScriptEditor.ExecuteScriptAsync("getText()");

        if (script == null) return null;

        script = script.Substring(1, script.Length - 2);
        script = Regex.Unescape(script);

        return script;
    }

    private async void SetEditorScript(string script)
    {
        script = ToLiteral(script);
        await ScriptEditor.ExecuteScriptAsync($"setText({script})");
    }
}