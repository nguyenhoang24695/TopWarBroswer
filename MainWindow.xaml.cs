using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using AutoBroswer.Entity;
using AutoBroswer.Util;
using KAutoHelper;
using Microsoft.Win32;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoBroswer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<Thread> threads = new List<Thread>();
        List<IntPtr> windows = new List<IntPtr>();
        List<ChromeDriver> chromeDrivers = new List<ChromeDriver>();
        int count = 0;
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

            var listToken = FileUtil.GetListTextFromFile("token.txt");

            var fbd = new OpenFileDialog();
            fbd.InitialDirectory = Environment.CurrentDirectory;
            var result = fbd.ShowDialog();

            var s = new List<Token>();
            if (result.HasValue && result.Value)
            {
                s = JsonConvert.DeserializeObject<List<Token>>(File.ReadAllText(fbd.FileName));

            }


            foreach (var item in s)
            {
                Thread new_thread = new Thread(new ThreadStart(() =>
                {
                    bool checkFoundProcess = false;

                    Thread.Sleep(item.ID * 2000);

                    var driver = new ChromeDriver(chromeOptions);
                    var windowHandle = IntPtr.Zero;

                    while (true)
                    {

                        var listProcess = AutoControl.FindWindowHandlesFromProcesses(null, "data:, - Google Chrome");

                        if (listProcess.Count > 0)
                        {
                            windowHandle = listProcess[0];
                            chromeDrivers.Add(driver);
                            windows.Add(windowHandle);
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
                    string value = "\"MTQwMSx3ZWJnYW1lZ2xvYmFsLDlhOTEwOGNlLTA3ZTYtNDFhOS05ZDkyLTk2YzI1MTQwNTM1YSx3c3M6Ly9zZXJ2ZXIta25pZ2h0LXMxMjAwLnJpdmVyZ2FtZS5uZXQvczE0MDEsMTY0ODQ0NzQ0NDkzNiw3MDJkMjNjYWM1MmFkOWUwYjhjZTY2YTY3MTYzYjBjZCwsLGdvb2dsZXBsYXksZXlKdmNHVnVhV1FpT2lJeE1UQXlNREl3TXpBNU1URTBOVFF5TmpFd01EWWlMQ0p1WVcxbElqb2lUbWQxZVdWdUlFaHZZVzVuSWl3aWNHbGpkSFZ5WlNJNkltaDBkSEJ6T2k4dmJHZ3pMbWR2YjJkc1pYVnpaWEpqYjI1MFpXNTBMbU52YlM5aEwwRkJWRmhCU25sMFZFVklUbEZIZURkME0yNU9VamRYV201a1ZtTjZUSGMzWW5FMU0wbzJjMVYyTVhVdFBYTTVOaTFqSWl3aVpXMWhhV3dpT2lKdVlYQmhMbTVuZFhsbGJtaHZZVzVuUUdkdFlXbHNMbU52YlNKOSw2Mzk4MDQ5MzM0NDIsWFNPTExBXzFlOTUxNzVhLTM2MmItNGVkZS05ZjkyLWU2Njg2M2JlNTk2ZCws\"";

                    js.ExecuteScript("localStorage.setItem('" + key + "','" + item.StringToken + "');");

                    driver.Url = "https://h5.topwargame.com/h5game/index.html";

                    AutoControl.MoveWindow(windowHandle, item.ID * 10, item.ID * 10, 1024, 768, true);

                    int waitTime = 2000;
                    var childHandle = KAutoHelper.AutoControl.GetChildHandle(windowHandle)[0];

                    bool isClicked = false;
                    while (!isClicked)
                    {
                        isClicked = ClickAction.ClickByImage(windowHandle, "image/1/x.png", 20, 1000);
                    }
                    AutoControl.SendText(windowHandle, item.Name);

                    Thread.Sleep(waitTime);
                    isClicked = ClickAction.ClickByImage(windowHandle, "image/1/world_map.png", 10, 1000);
                    Thread.Sleep(waitTime);

                BeforeCheckSlot:
                    //Kiem tra hang cho
                    bool isSlotAvailable = false;
                    int rl_slot = 0;
                    this.Dispatcher.Invoke(new System.Action(() =>
                    {
                        rl_slot = int.Parse(Queue_Textbox.Text);
                    }));
                    switch (rl_slot)
                    {
                        case 1:
                            isSlotAvailable = FindAction.FindByImageNorImage(windowHandle, "image/1/rally_slot/1/a.png", "image/1/rally_slot/1/f.png", 4, 1000);
                            break;
                        case 2:
                            isSlotAvailable = FindAction.FindByImageListNorImage(windowHandle, new List<string>(){
                                "image/1/rally_slot/2/0.png",
                                "image/1/rally_slot/2/1.png"
                                }.ToArray(),
                                "image/1/rally_slot/1/f.png", 2, 1000);
                            break;
                        case 4:
                            isSlotAvailable = FindAction.FindByImageNorImage(windowHandle, "image/1/rally_slot/4/3.png", "image/1/rally_slot/4/4.png", 4, 1000);
                            break;
                        default:
                            break;
                    }

                    if (!isSlotAvailable)
                    {
                        goto BeforeCheckSlot;
                    }
                    //

                    isClicked = ClickAction.ClickByImage(windowHandle, "image/1/search_btn.png", 10, 1000);
                    Thread.Sleep(waitTime);

                    //loai rally
                    string content_rl = string.Empty;
                    string team_rl = string.Empty;
                    this.Dispatcher.Invoke(new System.Action(() =>
                    {
                        content_rl = ((Button)sender).Content.ToString();
                        team_rl = Team_Textbox.Text;
                    }));
                    if (content_rl == "DF_5")
                    {
                        isClicked = ClickAction.ClickByImageOrImage(windowHandle, "image/1/df_checked.png", "image/1/df_unchecked.png", 10, 1000);
                    }
                    else
                    {
                        isClicked = ClickAction.ClickByImageOrImage(windowHandle, "image/1/rally_checked.png", "image/1/rally_unckeck.png", 10, 1000);
                    }
                    Thread.Sleep(waitTime);
                    isClicked = ClickAction.ClickByImage(windowHandle, "image/1/rally_search.png", 10, 1000);
                    Thread.Sleep(3000);
                    while (true)
                    {

                        ClickAction.ClickByPosition(windowHandle, 514, 461);
                        if (content_rl == "DF_5")
                        {

                            isClicked = ClickAction.ClickByImage(windowHandle, "image/1/rally_5_btn.png", 2, 1000);
                        }
                        else
                        {

                            isClicked = ClickAction.ClickByImage(windowHandle, "image/1/rally_10_btn.png", 2, 1000);
                        }
                        Thread.Sleep(waitTime);
                        if (isClicked)
                        {
                            break;
                        }
                    }
                    var foundPoint = FindAction.FindByImage(windowHandle, "image/1/save_infomation_btn.png", 10, 1000);
                    if (foundPoint.HasValue)
                    {
                        switch (int.Parse(team_rl))
                        {
                            case 0:
                                ClickAction.ClickByPosition(windowHandle, 512, 726);
                                break;
                            case 1:
                                isClicked = ClickAction.ClickByImage(windowHandle, "image/1/team_1.png", 10, 1000);
                                break;
                            default:
                                isClicked = ClickAction.ClickByImage(windowHandle, "image/1/team_quick.png", 10, 1000);
                                break;


                        }
                        Thread.Sleep(waitTime);
                        if (content_rl == "DF_5")
                        {
                            isClicked = ClickAction.ClickByImage(windowHandle, "image/1/silo_slot.png", 10, 1000);
                            isClicked = ClickAction.ClickByImage(windowHandle, "image/1/silo_item.png", 10, 1000);

                        }

                        Thread.Sleep(waitTime);
                        ClickAction.ClickByPosition(windowHandle, 510, 420);

                        if (content_rl == "DF_5")
                        {
                            isClicked = ClickAction.ClickByImage(windowHandle, "image/1/silo_ok.png", 10, 1000);

                        }
                        goto BeforeCheckSlot;
                    }
                    else
                    {
                        KAutoHelper.FindWindow.GetWindowThreadProcessId(windowHandle, out int pId);
                        Process.GetProcessById(pId).Kill();
                    }
                }));
                threads.Add(new_thread);
                new_thread.Start();
                count++;
            }



        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

            var s = JsonConvert.DeserializeObject<List<Token>>(File.ReadAllText("test.json"));
            File.WriteAllText("test.json", JsonConvert.SerializeObject(s, Formatting.Indented));

            var str = ImageScanOpenCV.RecolizeText(ImageScanOpenCV.GetImage("image/1/armyNumber.png"));


        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            if (windows.Count > 0)
            {
                foreach (var window in windows)
                {
                    ClickAction.ClickByPosition(window, int.Parse(toa_do_x.Text), int.Parse(toa_do_y.Text));
                }
            }
        }


    }
}
