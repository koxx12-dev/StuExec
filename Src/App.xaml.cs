using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Windows;
using Newtonsoft.Json;
using Semver;
using StuExec.Json.Github.Repos;
using StuExec.Settings;

namespace StuExec;

public partial class App
{
    public const string RbxStuPipe = "CommunicationPipe";
    public const string RbxStuDll = "Module.dll";
    private const string UpdateUrl = "https://github.com/koxx12-dev/StuExec/releases/latest";
    
    public static readonly SettingsStorage.Settings Settings = SettingsStorage.LoadXml() ?? new();

    public static string? CurrentVersion;
    
    private static readonly HttpClient GenericWebClient = new();
    public static readonly HttpClient GithubApiRepoClient = new()
    {
        BaseAddress = new("https://api.github.com/repos/"),
        Timeout = TimeSpan.FromSeconds(5),
    };
    
    public App()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version;
        
        if (version is not null)
            CurrentVersion = $"{version.Major}.{version.Minor}.{version.Build}";
        
        GithubApiRepoClient.DefaultRequestHeaders.Add("User-Agent",$"StuExec ({CurrentVersion ?? "unknown version"}) - (https://github.com/koxx12-dev/StuExec)");
        
        Exit += (_, _) => OnExit();
        
        CheckStuExecVersion();
        CheckRbxStuVersion();
    }
    
    private static void OnExit()
    {
        SettingsStorage.SaveXml(Settings);
    }

    private async void CheckRbxStuVersion()
    {
        if (!Settings.UpdateSettings.RbxStuAutoUpdate) return;
        
        var response = await GithubApiRepoClient.GetAsync("RbxStu/RbxStu-V2/releases/latest");
        
        if (!response.IsSuccessStatusCode) return;
        
        var json = await response.Content.ReadAsStringAsync();
        
        var latestRelease = JsonConvert.DeserializeObject<LatestRelease>(json, Converter.Settings);
        
        if (latestRelease is null) return;
        
        var version = latestRelease.TagName;
        
        var downloadAsset = latestRelease.Assets.FirstOrDefault(asset => asset.Name == RbxStuDll) ??
                           latestRelease.Assets.FirstOrDefault(asset => asset.Name == "Release.zip");
        
        var isZip = downloadAsset?.Name.EndsWith(".zip") ?? false;
        var downloadUrl = downloadAsset?.BrowserDownloadUrl;
        
        if (Settings.UpdateSettings.RbxStuAutoUpdateLastTag == version || downloadUrl is null) return;
        
        var result = MessageBox.Show(
            $"A new version of RbxStu is available.\n" +
            $"Current Version: {Settings.UpdateSettings.RbxStuAutoUpdateLastTag ?? "Not Installed"}\n" +
            $"Version: {version}\n" +
            "Would you like to update it?",
            "Stu-Exec - Update Available",
            MessageBoxButton.YesNo
        );
        
        if (result != MessageBoxResult.Yes) return;
        
        try
        {
            var downloadResponse = await GenericWebClient.GetAsync(downloadUrl);

            if (!downloadResponse.IsSuccessStatusCode) throw new Exception("Failed to download the zip file.");

            var downloadStream = await downloadResponse.Content.ReadAsStreamAsync();
            
            await using var fileStream = new FileStream($"{Environment.CurrentDirectory}/{RbxStuDll}", FileMode.Create,
                FileAccess.Write);

            if (isZip)
            {
                using var archive = new ZipArchive(downloadStream);

                var entry = archive.GetEntry(RbxStuDll);

                if (entry is null) throw new Exception("Failed to find RbxStu dll in the zip file.");
                
                var entryStream = entry.Open();
                
                //yea this is pretty jank but if I used CopyToAsync in another place it would throw an exception
                await entryStream.CopyToAsync(fileStream);
            } else
            {
                await downloadStream.CopyToAsync(fileStream);
            }
            
            if (fileStream.Length == 0) throw new Exception("Failed to write to the file stream.");
            
            await fileStream.FlushAsync();
            
            Settings.UpdateSettings.RbxStuAutoUpdateLastTag = version;

            MessageBox.Show("RbxStu has been updated successfully.", "Stu-Exec - Update Complete",
                MessageBoxButton.OK);
        } catch (Exception e)
        {
            MessageBox.Show("Failed to update RbxStu.", "Stu-Exec - Update Failed", MessageBoxButton.OK);
            Console.WriteLine(e.Message);
        }
    }
    
    private async void CheckStuExecVersion()
    {
        if (CurrentVersion == null || !Settings.UpdateSettings.StuExecUpdateNotif) return;
        
        var response = await GithubApiRepoClient.GetAsync("koxx12-dev/StuExec/releases/latest");
        
        if (!response.IsSuccessStatusCode) return;
        
        var json = await response.Content.ReadAsStringAsync();
        
        var latestRelease = JsonConvert.DeserializeObject<LatestRelease>(json, Converter.Settings);
        
        if (latestRelease is null) return;
        
        var version = latestRelease.TagName;

        try
        {
            var latestVersion = SemVersion.Parse(version, SemVersionStyles.AllowTrailingWhitespace);
            var currentVersion = SemVersion.Parse(CurrentVersion, SemVersionStyles.AllowTrailingWhitespace);
            
            if (latestVersion.ComparePrecedenceTo(currentVersion) <= 0) return;
            
            var result = MessageBox.Show(
                $"A new version of Stu-Exec is available.\n" +
                $"Current Version: {currentVersion}\n" +
                $"Latest Version: {latestVersion}\n" +
                "Would you like to open the release page?",
                "Stu-Exec - Update Available",
                MessageBoxButton.YesNo
            );
            
            if (result == MessageBoxResult.Yes)
                Process.Start(new ProcessStartInfo()
                {
                    FileName = UpdateUrl,
                    UseShellExecute = true
                });
        } catch (Exception e)
        {
            Console.WriteLine("Failed to parse version.");
            Console.WriteLine(e.Message);
        }
    }
}