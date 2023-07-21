using Avalonia.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yorot_Avalonia;

namespace Yorot
{
    internal static class Dialogs
    {
        public static async void RunInputBoxDialog(Window parent, MessageBox.Avalonia.BaseWindows.Base.IMsBoxWindow<MessageBox.Avalonia.DTO.MessageWindowResultDTO> box, Action<MessageBox.Avalonia.DTO.MessageWindowResultDTO> OnSuccess)
        {
            var result = await box.ShowDialog(parent);
            OnSuccess(result);
        }

        public static async void RunFileDialog(OpenFileDialog window, Window parent, Action<string[]?> OnSuccess)
        {
            var files = await window.ShowAsync(parent);
            OnSuccess(files);
        }

        public static async void RunFolderDialog(OpenFolderDialog window, Window parent, Action<string?> OnSuccess)
        {
            var result = await window.ShowAsync(parent);
            OnSuccess(result);
        }

        public static async void RunDialog(Window window, Window parent, Action OnSuccess)
        {
            await window.ShowDialog(parent);
            OnSuccess();
        }

        public static async void RunMessageBoxDialog(MessageBox.Avalonia.BaseWindows.Base.IMsBoxWindow<string> box, Window parent, Action<string> OnSuccess)
        {
            var result = await box.ShowDialog(parent);
            OnSuccess(result);
        }

        public static async void RunSaveFileDialog(Window parent, string title, string[] filetypes, Action<string> OnSuccess)
        {
            if (YorotGlobal.Main != null)
            {
                SaveFileDialog saveFileDialog = new()
                {
                    Title = string.IsNullOrWhiteSpace(title) ? YorotGlobal.Main.CurrentLanguage.GetItemText("DialogBox.SaveFileDialog") : title
                };
                List<FileDialogFilter> Filters = new();

                for (int i = 0; i < filetypes.Length; i++)
                {
                    FileDialogFilter filter = new();
                    List<string> extension = new()
                    {
                        filetypes[i]
                    };
                    filter.Extensions = extension;
                    filter.Name = YorotGlobal.Main.CurrentLanguage.GetItemText("FileTypes." + filetypes[i].ToUpperInvariant());
                    Filters.Add(filter);
                }
                saveFileDialog.Filters = Filters;

                saveFileDialog.DefaultExtension = filetypes[0];

                saveFileDialog.Directory = YorotGlobal.Main.CurrentSettings.DownloadManager.DownloadFolder;

                var filename = await saveFileDialog.ShowAsync(YorotGlobal.Main.MainForm);

                if (!string.IsNullOrWhiteSpace(filename))
                {
                    OnSuccess(filename);
                }
            }
        }
    }
}