using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
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
        List<Token> currentListToken = new List<Token>();
        int count = 0;
        int useVit = 0;
        string currentPath = string.Empty;
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


            Thread new_thread = new Thread(new ThreadStart(() =>
            {
                windows.Clear();
                try
                {
                    ChromeOptions chromeOptions = new ChromeOptions();
                    chromeOptions.AddArgument("--disable-gpu");
                    int totalToken = currentListToken.Count;
                    int currentCount = 0;
                    ConcurrentQueue<Token> tokenBag = new ConcurrentQueue<Token>(currentListToken);

                Open_Broser_Block:
                    WriteLog("Tìm trình duyệt");
                    var driver = new ChromeDriver(chromeOptions);
                    var windowHandle = IntPtr.Zero;

                    WriteLog("Tìm windowProcess");
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
                    AutoControl.MoveWindow(windowHandle, 10, 10, 1024, 768, true);
                    WriteLog("Vào game");
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
                Start_Job_Block:
                    while (tokenBag.Count > 0)
                    {
                        tokenBag.TryDequeue(out var item);
                        WriteLog("SetTokenRunning");
                        SetTokenRunning(item);
                        js.ExecuteScript("localStorage.setItem('" + key + "','" + item.StringToken + "');");

                        driver.Url = "https://h5.topwargame.com/h5game/index.html";

                        int waitTime = int.Parse(ConfigurationManager.AppSettings["waitTime"].ToString());
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

                        //loai rally
                        string content_rl = string.Empty;
                        bool is_only_one = false;
                        string team_rl = item.Slot.ToString();
                        this.Dispatcher.Invoke(new System.Action(() =>
                        {
                            content_rl = ((Button)sender).Content.ToString();
                            is_only_one = RunOne_checkbox.IsChecked.HasValue ? RunOne_checkbox.IsChecked.Value : false;
                            //team_rl = Team_Textbox.Text;
                        }));

                    BeforeCheckSlot:
                        //Kiem tra hang cho
                        bool isSlotAvailable = false;
                        WriteLog("Kiem tra rally trong");
                        int rl_slot = item.TotalRally;
                        //this.Dispatcher.Invoke(new System.Action(() =>
                        //    {
                        //        rl_slot = int.Parse(Queue_Textbox.Text);
                        //    }));
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
                                        "image/1/rally_slot/1/f.png", 4, 1000);
                                break;
                            case 4:
                                //isSlotAvailable = FindAction.FindByImageNorImage(windowHandle, "image/1/rally_slot/4/3.png", "image/1/rally_slot/4/4.png", 4, 1000);
                                isSlotAvailable = FindAction.FindByImageListNorImage(windowHandle, new List<string>(){
                                "image/1/rally_slot/4/0.png",
                                "image/1/rally_slot/4/1.png",
                                "image/1/rally_slot/4/2.png",
                                "image/1/rally_slot/4/3.png"
                            }.ToArray(),
                                        "image/1/rally_slot/4/4.png", 4, 1000);
                                break;
                            default:
                                break;
                        }

                        if (!isSlotAvailable)
                        {
                            if (content_rl == "Hammer Rally" && !is_only_one)
                            {
                                WriteLog("Rally full, dang nhap token tiep theo");
                                tokenBag.Enqueue(item);
                                continue;
                            }
                            Thread.Sleep(waitTime * 5);
                            goto BeforeCheckSlot;
                        }
                        //

                        if (content_rl == "SOS")
                        {
                            isClicked = ClickAction.ClickByImage(windowHandle, "image/1/bag.png", 10, 1000);
                            Thread.Sleep(waitTime);
                            isClicked = ClickAction.ClickByImage(windowHandle, "image/1/letterSoS.png", 10, 1000);
                            Thread.Sleep(waitTime);
                            isClicked = ClickAction.ClickByImage(windowHandle, "image/1/use_btn.png", 10, 1000);
                            goto ClickAI;

                        }

                        WriteLog(String.Format("Find: {0}", "image/1/search_btn.png"));
                        isClicked = ClickAction.ClickByImage(windowHandle, "image/1/search_btn.png", 10, 1000);
                        Thread.Sleep(waitTime);


                        if (content_rl == "DF_5" || content_rl == "DF_25")
                        {
                            WriteLog(String.Format("Find: {0}", "image/1/df_checked.png"));
                            isClicked = ClickAction.ClickByImageOrImage(windowHandle, "image/1/df_checked.png", "image/1/df_unchecked.png", 10, 1000);
                        }
                        else
                        {
                            WriteLog(String.Format("Find: {0}", "image/1/rally_checked.png"));
                            isClicked = ClickAction.ClickByImageOrImage(windowHandle, "image/1/rally_checked.png", "image/1/rally_unckeck.png", 10, 1000);
                            Thread.Sleep(waitTime / 2);
                            int countClick = 0;
                            while (true)
                            {
                                int lv = CheckLevelRally(windowHandle);
                                countClick++;
                                if (lv == item.Level)
                                {
                                    break;
                                }
                                ClickAction.ClickByImage(windowHandle, "image/1/plus_btn.png", 10, 1000);
                                if (countClick >= 3)
                                {
                                    ClickAction.ClickByImage(windowHandle, "image/1/minus_btn.png", 10, 1000);
                                    break;

                                }
                                Thread.Sleep(waitTime / 2);
                            }


                        }
                        Thread.Sleep(waitTime / 2);
                    ClickRally:
                        WriteLog(String.Format("Find: {0}", "image/1/rally_search.png"));
                        isClicked = ClickAction.ClickByImage(windowHandle, "image/1/rally_search.png", 10, 1000);
                        Thread.Sleep(2000);
                        if (FindAction.FindByImageNorImage(windowHandle, "image/1/rally_search.png", "image/1/map_icon_btn.png", 10, 1000))
                        {
                            ClickAction.ClickByImage(windowHandle, "image/1/minus_btn.png", 10, 1000);
                            goto ClickRally;
                        }

                    ClickAI:
                        var clickCount = 0;
                        while (true)
                        {
                            if (clickCount == 5)
                            {
                                // Cho nó chạy lại từ đầu check slot 
                                goto BeforeCheckSlot;
                            }
                            ClickAction.ClickByPosition(windowHandle, 514, 461);
                            if (content_rl == "DF_5")
                            {

                                WriteLog(String.Format("Find: {0}", "image/1/rally_5_btn.png"));
                                isClicked = ClickAction.ClickByImage(windowHandle, "image/1/rally_5_btn.png", 2, 1000);
                            }
                            else if (content_rl == "SOS")
                            {

                                WriteLog(String.Format("Find: {0}", "image/1/rally_5_btn1.png"));
                                isClicked = ClickAction.ClickByImage(windowHandle, "image/1/rally_5_btn1.png", 2, 1000);
                            }
                            else if (content_rl == "DF_25")
                            {

                                WriteLog(String.Format("Find: {0}", "image/1/atk_25_btn.png"));
                                isClicked = ClickAction.ClickByImage(windowHandle, "image/1/atk_25_btn.png", 2, 1000);
                            }
                            else
                            {

                                WriteLog(String.Format("Find: {0}", "image/1/rally_10_btn.png"));
                                isClicked = ClickAction.ClickByImage(windowHandle, "image/1/rally_10_btn.png", 2, 1000);
                            }
                            clickCount++;
                            Thread.Sleep(waitTime);
                            if (isClicked)
                            {
                                break;
                            }
                        }
                        WriteLog(String.Format("Find: {0}", "image/1/save_infomation_btn.png"));
                        var foundPoint = FindAction.FindByImage(windowHandle, "image/1/save_infomation_btn.png", 10, 1000);
                        if (foundPoint.HasValue)
                        {
                            switch (int.Parse(team_rl))
                            {
                                case 0:
                                    ClickAction.ClickByPosition(windowHandle, 512, 726);
                                    break;
                                case 1:
                                    WriteLog(String.Format("Find: {0}", "image/1/team_1.png"));
                                    isClicked = ClickAction.ClickByImage(windowHandle, "image/1/team_1.png", 10, 1000);
                                    break;
                                case 3:
                                    WriteLog(String.Format("Find: {0}", "image/1/team_3.png"));
                                    isClicked = ClickAction.ClickByImage(windowHandle, "image/1/team_3.png", 10, 1000);
                                    break;
                                default:
                                    WriteLog(String.Format("Find: {0}", "image/1/team_quick.png"));
                                    isClicked = ClickAction.ClickByImage(windowHandle, "image/1/team_quick.png", 10, 1000);
                                    break;


                            }
                            Thread.Sleep(waitTime);
                            if (content_rl == "DF_5")
                            {
                                WriteLog(String.Format("Find: {0}", "DF_5"));
                                isClicked = ClickAction.ClickByImage(windowHandle, "image/1/silo_slot.png", 10, 1000);
                                Thread.Sleep(waitTime);
                                isClicked = ClickAction.ClickByImage(windowHandle, "image/1/silo_item.png", 10, 1000);
                                Thread.Sleep(waitTime);

                            }

                            ClickAction.ClickByPosition(windowHandle, 510, 420);

                            if (content_rl == "DF_5")
                            {
                                Thread.Sleep(waitTime);
                                isClicked = ClickAction.ClickByImage(windowHandle, "image/1/silo_ok.png", 10, 1000);

                            }
                            WriteLog(String.Format("Find: {0}", "Completed Rally"));
                            Thread.Sleep(waitTime);
                            goto BeforeCheckSlot;
                        }
                        else
                        {
                            WriteLog(String.Format("Find: {0}", "image/1/image/1/vit_table.png"));
                            foundPoint = FindAction.FindByImage(windowHandle, "image/1/vit_table.png", 2, 1000);
                            if (foundPoint.HasValue)
                            {
                                WriteLog(String.Format("Het The Luc"));
                                switch (useVit)
                                {
                                    case 10:
                                        ClickAction.ClickByImage(windowHandle, "image/1/vit_10.png");
                                        Thread.Sleep(waitTime/2);
                                        for(int i = 0; i < 5; i++)
                                        {
                                            ClickAction.ClickByImage(windowHandle, "image/1/vit_use.png");
                                            Thread.Sleep(waitTime / 2);
                                        }
                                        break;
                                    case 50:
                                        ClickAction.ClickByImage(windowHandle, "image/1/vit_50.png");
                                        Thread.Sleep(waitTime/2);
                                        ClickAction.ClickByImage(windowHandle, "image/1/vit_use.png");
                                        Thread.Sleep(waitTime / 2);
                                        break;
                                    default:
                                        if (++currentCount == totalToken)
                                        {
                                            goto LoopEnd;
                                        }
                                        break;

                                }
                                ClickAction.ClickByImage(windowHandle, "image/1/vit_out_btn.png");
                                if (content_rl == "DF_25")
                                {
                                    goto ClickAI;
                                }

                            }
                            else
                            {
                                if (++currentCount == totalToken)
                                {
                                    goto LoopEnd;
                                }
                            }
                        }
                    }
                LoopEnd:
                    KAutoHelper.FindWindow.GetWindowThreadProcessId(windowHandle, out int pId);
                    Process.GetProcessById(pId).Kill();
                    WriteLog("Finished");
                }
                catch (Exception ex)
                {
                    WriteLog(ex.Message);
                }
            }));
            new_thread.Start();
        }

        private int CheckLevelRally(IntPtr windowHandle)
        {
            List<string> listPathRallyLevel = new List<string>();
            listPathRallyLevel.Add("image/1/rally_level/10.png");
            listPathRallyLevel.Add("image/1/rally_level/20.png");
            listPathRallyLevel.Add("image/1/rally_level/30.png");
            return FindAction.CheckExistByListImage(windowHandle, listPathRallyLevel, 10, 1000);
        }

        private void SetTokenRunning(Token item)
        {
            this.Dispatcher.Invoke(new System.Action(() =>
            {

                foreach (var item1 in Token_DataGrid.Items)
                {
                    try
                    {

                        if (((Token)item1).Name == item.Name)
                        {
                            ((Token)item1).IsRunning = true;
                        }
                        else
                        {
                            ((Token)item1).IsRunning = false;
                        }
                    }
                    catch (Exception e)
                    {

                    }
                }
                Token_DataGrid.Items.Refresh();
            }));
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Token_DataGrid.ItemsSource = new List<Token>();
        }

        private void OpenFile_Button_Click(object sender, RoutedEventArgs e)
        {
            var fbd = new OpenFileDialog();
            fbd.InitialDirectory = Environment.CurrentDirectory;
            fbd.Filter = "Json files (*.json)|*.json";
            var result = fbd.ShowDialog();

            if (result.HasValue && result.Value)
            {
                currentPath = fbd.FileName;
                currentListToken = JsonConvert.DeserializeObject<List<Token>>(File.ReadAllText(fbd.FileName));
                Token_DataGrid.ItemsSource = null;
                Token_DataGrid.ItemsSource = currentListToken;
                WriteLog(string.Format("Open: {0}", currentPath));
            }
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            currentListToken = (List<Token>)Token_DataGrid.ItemsSource;
            FileUtil.WriteJsonToFile<List<Token>>(currentListToken, currentPath);
            WriteLog(string.Format("Save into: {0}", currentPath));
        }

        private void WriteLog(string textLog)
        {
            this.Dispatcher.Invoke(new System.Action(() =>
                        {
                            Log_RichTextBox.AppendText(textLog);
                            Log_RichTextBox.AppendText("\r");
                        }));
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {
            var content = ((Button)sender).Content.ToString();
            if (content == "Hide Window")
            {
                AutoControl.MoveWindow(windows[0], -1000, -1000, 1024, 768, true);
            }
            else
            {
                AutoControl.MoveWindow(windows[0], 10, 10, 1024, 768, true);
            }
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            using (Process process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = System.IO.Path.Combine(Environment.SystemDirectory, "cmd.exe");
                process.StartInfo.Arguments = "/C taskkill /IM \"chromedriver.exe\"";
                process.StartInfo.CreateNoWindow = true;
                process.Start();
            }
        }

        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            ChromeOptions chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("--disable-gpu");
            int totalToken = currentListToken.Count;
            int currentCount = 0;
            ConcurrentQueue<Token> tokenBag = new ConcurrentQueue<Token>(currentListToken);

        Open_Broser_Block:
            WriteLog("Tìm trình duyệt");
            var driver = new ChromeDriver(chromeOptions);
            var windowHandle = IntPtr.Zero;

            WriteLog("Tìm windowProcess");
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
            AutoControl.MoveWindow(windowHandle, 10, 10, 1024, 768, true);
            WriteLog("Vào game");
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

            var item = currentListToken[0];

            WriteLog("SetTokenRunning");
            SetTokenRunning(item);
            js.ExecuteScript("localStorage.setItem('" + key + "','" + item.StringToken + "');");

            driver.Url = "https://h5.topwargame.com/h5game/index.html";

            bool isClicked = false;
            while (!isClicked)
            {
                isClicked = ClickAction.ClickByImage(windowHandle, "image/1/x.png", 20, 1000);
            }
            AutoControl.SendText(windowHandle, item.Name);
        }

        private void Log_RichTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // scroll it automatically
            Log_RichTextBox.ScrollToEnd();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            useVit = int.Parse(((RadioButton)sender).Content.ToString());
        }
    }
}
