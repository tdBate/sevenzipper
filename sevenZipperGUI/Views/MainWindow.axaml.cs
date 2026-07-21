using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace sevenZipperGUI.Views;


public partial class MainWindow : Window
{
    LocationPath location;

    public MainWindow()
    {
        InitializeComponent();
    }

    async public void OpenFile(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var files = await topLevel.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select a File",
            AllowMultiple = false,
        });

        if (files.Count >= 1)
        {
            var selectedFile = files[0];
            location = new LocationPath(selectedFile.Path.LocalPath);
            DisplayPath();
        }
    }

    async public void OpenFolder(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var folders = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select a Folder",
            AllowMultiple = false
        });

        if (folders.Count >= 1)
        {
            var selectedFolder = folders[0];
            location = new LocationPath(selectedFolder.Path.LocalPath);
            DisplayPath();
        }
    }

    private void DisplayPath()
    {
        lblPath.Content = location.FullPath();
    }

    private void StartCompress(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        string command = $"cd {location.folderPath}/ && 7z a testadpaef.7z {location.fileName}"; //set command
        ProcessStartInfo processInfo = new ProcessStartInfo("/bin/bash")
        {
            CreateNoWindow = false,          // Hides the terminal window
            UseShellExecute = false,        // Required to redirect output
            RedirectStandardOutput = true,  // Captures normal output
            RedirectStandardError = true    // Captures error output
        };

        processInfo.ArgumentList.Add("-c");
        processInfo.ArgumentList.Add(command);

        using (Process process = Process.Start(processInfo))
        {
            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();

            process.WaitForExit();

            if (!string.IsNullOrEmpty(output))
            {
                Console.WriteLine("OUTPUT:\n" + output);
            }
            if (!string.IsNullOrEmpty(error))
            {
                Console.WriteLine("ERROR:\n" + error);
            }
        }
    }
}