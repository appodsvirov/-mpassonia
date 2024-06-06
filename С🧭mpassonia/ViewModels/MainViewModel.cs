using ReactiveUI;
using System.Collections.ObjectModel;
using System.IO;
using System.Reactive;
using Avalonia.Controls;
using System;
using Avalonia.Controls.ApplicationLifetimes;
using System.Threading.Tasks;
using System.Diagnostics;
namespace С__mpassonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _dirListFilePath = "dirlist.txt";
    private string _fileListFilePath = "filelist.txt";
    public ObservableCollection<string> DirList { get; set; } = new();
    public ObservableCollection<string> DirNameList { get; set; } = new();
    public ObservableCollection<string> FileList { get; set; } = new();
    public ReactiveCommand<Unit, Unit> AddDirCommand { get; set; }
    public ReactiveCommand<string, Unit> OpenDirCommand { get; set; }
    public ReactiveCommand<string, Unit> DeleteDirCommand { get; set; }

    public MainViewModel()
    {
        AddDirCommand = ReactiveCommand.CreateFromTask(AddDir);
        OpenDirCommand = ReactiveCommand.Create<string>(OpenDirOrFile);
        DeleteDirCommand = ReactiveCommand.Create<string>(DeleteDir);
        CreateFileIfNotExists(_dirListFilePath);
        CreateFileIfNotExists(_fileListFilePath);
        
        using (StreamReader reader = new StreamReader(_dirListFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                DirList.Add(line);
            }
        }

        using (StreamReader reader = new StreamReader(_fileListFilePath))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                FileList.Add(line);
            }
        }

        // Создание файлов, если они не существуют

    }
    private async Task AddDir()
    {
        OpenFolderDialog dialog = new OpenFolderDialog();
        var parentWindow = (App.Current.ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        string selectedDir = await dialog.ShowAsync(parentWindow);
        if (!string.IsNullOrEmpty(selectedDir))
        {
            DirList.Add(selectedDir);
            UpdateFile(_dirListFilePath, DirList);
        }
    }
    private void DeleteDir(string path)
    {
        DirList.Remove(path);
        UpdateFile(_dirListFilePath, DirList);
    }
    public void OpenDirOrFile(string path)
    {
        try
        {
            bool isDirectory = System.IO.Directory.Exists(path);
            bool isFile = System.IO.File.Exists(path);

            if (isDirectory || isFile)
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = isDirectory ? path : System.IO.Path.GetDirectoryName(path),
                    UseShellExecute = true,
                    Verb = "open"
                });
            }
            else
            {
                Console.WriteLine("Указанный путь не существует.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при открытии файла или директории: " + ex.Message);
        }
    }

    private void CreateFileIfNotExists(string filePath)
    {
        if (!File.Exists(filePath))
        {
            using (File.Create(filePath)) 
            {
                
            }
        }
    }

    private void UpdateFile(string path, ObservableCollection<string> source)
    {
        using (var sw = new StreamWriter(path))
        {
            sw.WriteLine(string.Join("\n", source));
        }
    }
}
