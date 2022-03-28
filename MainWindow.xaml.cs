using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AutoBroswer.Action;
using KAutoHelper;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoBroswer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //ProcessStartInfo processStartInfo = new ProcessStartInfo();
            //processStartInfo.FileName = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe";
            //processStartInfo.Arguments = "--profile-directory=\"Profile 1\"";
            //Process.Start(processStartInfo);

            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-gpu");

            var driver = new ChromeDriver(chromeOptions);

            var windowHandle = IntPtr.Zero;
            while (true)
            {

                var listProcess = AutoControl.FindWindowHandlesFromProcesses(null, "data:, - Google Chrome");

                if (listProcess.Count > 0)
                {
                    windowHandle = listProcess[0];
                }
                if (windowHandle != IntPtr.Zero)
                {
                    break;
                }
            }

            driver.Url = "https://h5.topwargame.com/h5game/index.html";
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;

            string key = "topwar_app_serverInfoToken";

            while (true)
            {
                var text = (String)js.ExecuteScript("return localStorage.getItem('" + key + "')");
                if (!string.IsNullOrEmpty(text))
                {
                    Console.WriteLine(text);
                    break;
                }

            }
            string value = "MTQwMSx3ZWJnYW1lZ2xvYmFsLDlhOTEwOGNlLTA3ZTYtNDFhOS05ZDkyLTk2YzI1MTQwNTM1YSx3c3M6Ly9zZXJ2ZXIta25pZ2h0LXMxMjAwLnJpdmVyZ2FtZS5uZXQvczE0MDEsMTY0ODQ0NzQ0NDkzNiw3MDJkMjNjYWM1MmFkOWUwYjhjZTY2YTY3MTYzYjBjZCwsLGdvb2dsZXBsYXksZXlKdmNHVnVhV1FpT2lJeE1UQXlNREl3TXpBNU1URTBOVFF5TmpFd01EWWlMQ0p1WVcxbElqb2lUbWQxZVdWdUlFaHZZVzVuSWl3aWNHbGpkSFZ5WlNJNkltaDBkSEJ6T2k4dmJHZ3pMbWR2YjJkc1pYVnpaWEpqYjI1MFpXNTBMbU52YlM5aEwwRkJWRmhCU25sMFZFVklUbEZIZURkME0yNU9VamRYV201a1ZtTjZUSGMzWW5FMU0wbzJjMVYyTVhVdFBYTTVOaTFqSWl3aVpXMWhhV3dpT2lKdVlYQmhMbTVuZFhsbGJtaHZZVzVuUUdkdFlXbHNMbU52YlNKOSw2Mzk4MDQ5MzM0NDIsWFNPTExBXzFlOTUxNzVhLTM2MmItNGVkZS05ZjkyLWU2Njg2M2JlNTk2ZCws";

            js.ExecuteScript("localStorage.setItem('" + key + "','\"" + value + "\"');");

            driver.Url = "https://h5.topwargame.com/h5game/index.html";

            AutoControl.MoveWindow(windowHandle, 0, 0, 1024, 768, true);

            int waitTime = 2000;


            bool isClicked = false;
            isClicked = ClickAction.ClickByImage(windowHandle, "image/1/x.png", 20, 1000);
            AutoControl.SendText(windowHandle, "Top1");
            Thread.Sleep(waitTime);
            isClicked = ClickAction.ClickByImage(windowHandle, "image/1/world_map.png", 10, 1000);
            Thread.Sleep(waitTime);
            isClicked = ClickAction.ClickByImage(windowHandle, "image/1/search_btn.png", 10, 1000);
            Thread.Sleep(waitTime);
            isClicked = ClickAction.ClickByImageOrImage(windowHandle, "image/1/rally_checked.png", "image/1/rally_unckeck.png", 10, 1000);
            Thread.Sleep(waitTime);
            isClicked = ClickAction.ClickByImage(windowHandle, "image/1/rally_search.png", 10, 1000);
            Thread.Sleep(3000);
            while (true)
            {

                ClickAction.ClickByPosition(windowHandle, 514, 461);
                isClicked = ClickAction.ClickByImage(windowHandle, "image/1/rally_10_btn.png", 2, 1000);
                Thread.Sleep(waitTime);
                if (isClicked)
                {
                    break;
                }
            }
            var foundPoint = FindAction.FindByImage(windowHandle, "image/1/save_infomation_btn.png", 10, 1000);
            if (foundPoint.HasValue)
            {
                isClicked = ClickAction.ClickByImage(windowHandle, "image/1/team_1.png", 10, 1000);
                Thread.Sleep(waitTime);
                ClickAction.ClickByPosition(windowHandle, 510, 420);

            }

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            var str = ImageScanOpenCV.RecolizeText(ImageScanOpenCV.GetImage("image/1/armyNumber.png"));
            

        }
    }
}
