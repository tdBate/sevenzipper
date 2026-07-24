using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace sevenZipperGUI.Views;


public partial class MainWindow : Window
{
    List<LocationPath> location = new List<LocationPath>();
    string target;

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
            AllowMultiple = true,
        });

        if (files.Count >= 1)
        {
            for (int i = 0; i < files.Count; i++)
            {
                location.Add(new LocationPath(files[i].Path.LocalPath));
            }
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
            AllowMultiple = true
        });

        if (folders.Count >= 1)
        {
            for (int i = 0; i < folders.Count; i++)
            {
                location.Add(new LocationPath(folders[i].Path.LocalPath));
            }
            DisplayPath();
        }
    }

    private void DisplayPath()
    {
        lbSelected.Items.Clear();
        for (int i = 0; i < location.Count; i++)
        {
            ListBoxItem l1 = new ListBoxItem();
            l1.Content = location[i].FullPath();
            lbSelected.Items.Add(l1);
        }

    }

    private void StartCompress(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //command setup
        string command = $"7z a {target} ";
        for (int i = 0; i < location.Count; i++)
        {
            command += location[i].FullPath() + " ";
        }

        System.Console.WriteLine(command);

        //compression process start
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

        //reset selected items
        lbSelected.Items.Clear();
        location.Clear();
    }

    async private void SelectTarget(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        if (topLevel == null) return;

        var file = await topLevel.StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Save Archive As...",
            SuggestedFileName = "archive.7z",
            DefaultExtension = "7z",
            FileTypeChoices = new[]
        {
            new FilePickerFileType("Text Documents") { Patterns = new[] { "*.7z" } },
        }
        });

        if (file != null)
        {
            target = file.Path.LocalPath;
            lblTarget.Content = file.Path.LocalPath;
        }
    }

    private void DeleteSelectedItems(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        List<ListBoxItem> removeItems = lbSelected.SelectedItems.Cast<ListBoxItem>().ToList();
        for (int i = 0; i < removeItems.Count; i++)
        {
            ListBoxItem l1 = (ListBoxItem)removeItems[i];
            int objIndex = location.FindIndex(r => r.FullPath() == l1.Content.ToString());

            location.RemoveAt(objIndex);
            lbSelected.Items.Remove(l1);
        }
    }
}