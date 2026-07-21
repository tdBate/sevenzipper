using Avalonia.Controls;
using Avalonia.Platform.Storage;
using System;

namespace sevenZipperGUI.Views;


public partial class MainWindow : Window
{
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
            string filePath = selectedFile.Path.LocalPath;
            Console.WriteLine(filePath);
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
            string filePath = selectedFolder.Path.LocalPath;
            Console.WriteLine(filePath);
        }
    }
}