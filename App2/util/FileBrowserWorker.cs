using Microsoft.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Pickers;

namespace App2.util
{
    internal class FileBrowserWorker
    {
        public async static Task<string> GetFilePathAsync(Window window)
        {
            var filePicker = new FileOpenPicker();

            // Get the current window's HWND by passing in the Window object
            var hwnd = WinRT.Interop.WindowNative.GetWindowHandle(window);

            // Associate the HWND with the file picker
            WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hwnd);

            // Use file picker like normal!
            filePicker.FileTypeFilter.Add(".SLDPRT");
            var file = await filePicker.PickSingleFileAsync();

            return (file != null)? file.Path: null;
        }
    }
}
