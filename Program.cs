using System;
using System.Configuration;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Automation;
using System.Windows.Forms;

namespace HelloWorld
{
    class Program
    {
        [DllImport("user32.dll")]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        static void Main(string[] args)
        {
            Automation.AddAutomationEventHandler(
                eventId: WindowPattern.WindowOpenedEvent,
                element: AutomationElement.RootElement,
                scope: TreeScope.Children,
                eventHandler: OnWindowOpened);
            if (bool.Parse(ConfigurationManager.AppSettings.Get("hideconsole")))
            {
                IntPtr ownhandle = Process.GetCurrentProcess().MainWindowHandle;
                ShowWindow(ownhandle, 0);
            }
            Console.ReadLine();
            Automation.RemoveAllEventHandlers();
        }
        private static async void OnWindowOpened(object sender, AutomationEventArgs automationEventArgs)
        {
            try
            {
                var element = sender as AutomationElement;
                if (element != null)
                {
                    Console.WriteLine("New Window opened ({0}), getting Window Title ...", element.Current.ProcessId);
                    await Task.Delay(100);
                    string name = element.Current.Name;
                    int handle = element.Current.NativeWindowHandle;
                    string targetname = ConfigurationManager.AppSettings.Get("name");
                    Console.WriteLine("New Window opened: {0}, handle:{1}", name, handle);
                    if (name == targetname)
                    {
                        int width = Int32.Parse(ConfigurationManager.AppSettings.Get("width"));
                        int height = Int32.Parse(ConfigurationManager.AppSettings.Get("height"));
                        int screenwidth = Screen.PrimaryScreen.Bounds.Width;
                        int screenheight = Screen.PrimaryScreen.Bounds.Height;
                        int top = (screenheight - height) / 2;
                        int left = (screenwidth - width) / 2;
                        ShowWindow((IntPtr)handle, 1);
                        await Task.Delay(50);
                        SetWindowPos((IntPtr)handle, IntPtr.Zero, left, top, width, height, 0);
                        Console.WriteLine("New Window is \"{0}\", setting top:{1}, left:{2}, width:{3}, height:{4} (Screen {5}x{6})", name, top, left, width, height, screenwidth, screenheight);
                    }
                }
            }
            catch (ElementNotAvailableException)
            {
            }
        }
    }
}
