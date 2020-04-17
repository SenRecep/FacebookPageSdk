using CefSharp;
using FacebookPageSdk.Entities.Concrete;
using FacebookPageSdk.Services;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace FacebookPageSdk.WpfUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string PageName = "";
        private int fileCount = 0;
        public readonly string FacebookUrl = "https://m.facebook.com";
        public readonly System.Timers.Timer pageDownTimer;
        public int PageCount = 0;

        private const string TimerJs = "var myInterval = setInterval(function(){window.scrollTo(0, document.body.scrollHeight);},900)";
        private const string TimerClear = "clearInterval(myInterval)";
        private const string RemoveDivs = "document.querySelectorAll('._3drp').forEach(function(a) {a.parentElement.parentElement.remove()})";

        private List<Post> posts;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Browser.BrowserSettings = new BrowserSettings()
            {
                //ImageLoading = CefState.Disabled,
            };
            pageDownTimer = new System.Timers.Timer()
            {
                Interval = TimeSpan.FromMilliseconds(900).TotalMilliseconds,
            };
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Title = $"Social Scrapper";
            BtnPageDown.Click += BtnPageDown_Click;
            BtnPageDownStop.Click += BtnPageDownStop_Click;
            BtnLoad.Click += BtnLoad_Click; ;
            pageDownTimer.Elapsed += PageDownTimer_Elapsed;
            dg.SelectionChanged += Dg_SelectionChanged;
            cb.SelectionChanged += Cb_SelectionChanged;
            BtnSearch.Click += BtnSearch_Click;
            //Browser.ShowDevTools();
        }

        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(SearchBox.Text))
                dg.ItemsSource = posts;
            else
            {
                foreach (Post item in posts)
                {
                    var des = item.Description == null ? "" :item.Description;
                    var tit = item.Title == null ? "" :item.Title;
                    item.Title = tit;
                    item.Description = des;
                }

                dg.ItemsSource = posts.Where(p=>
                p.Description.Contains(SearchBox.Text) || 
                p.Title.Contains(SearchBox.Text)).ToList();
            }
               
            SearchCount.Content = $"Toplam Post Sayısı: {((List<Post>)dg.ItemsSource).Count}";
        }

        private void BtnLoad_Click(object sender, RoutedEventArgs e)
        {
            if (cb.SelectedItem is ComboBoxItem item)
            {
                PageName = item.Tag.ToString();
                var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), PageName);
                if (Directory.Exists(dir))
                {
                    var files = Directory.GetFiles(dir);
                    var postFiles = files.Where(f => f.Contains("Posts-") && f.Contains(".json"));
                    posts = new List<Post>();
                    postFiles.OrderBy(p=>p.Replace("Posts-","").Replace(".json","")).ToList().ForEach((p) => {
                        var jsonPosts = File.ReadAllText(p);
                        var parsedPosts = JsonConvert.DeserializeObject<List<Post>>(jsonPosts);
                        posts.AddRange(parsedPosts);
                    });
                    dg.ItemsSource = posts;
                    string json = JsonConvert.SerializeObject(posts,Formatting.Indented);
                    File.WriteAllText(Path.Combine(dir, $"{PageName}-All.json"),json);
                    SearchCount.Content = $"Toplam Post Sayısı: {posts.Count}";
                }
            }
           
        }

        private void Cb_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cb.SelectedItem is ComboBoxItem item)
            {
                Browser.Address = $"{FacebookUrl}/{item.Tag}";
                PageName = item.Tag.ToString();
            }

        }

        private void Dg_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {

        }

        private void PageDownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            PageCount++;
            dg.Dispatcher.Invoke(() =>
            {
                Title = PageCount.ToString();
            });
            if (PageCount > 50)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((callback) =>
                {
                    pageDownTimer.Stop();
                    PageCount = 0;
                    Browser.ExecuteScriptAsync(TimerClear);
                    string source = Browser.GetSourceAsync().Result;
                    FacebookPageService pageService = new FacebookPageService(source);
                    var posts = pageService.GetPosts();
                    string json = JsonConvert.SerializeObject(posts, Formatting.Indented);
                    string FOLDER = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{PageName}");
                    Directory.CreateDirectory(FOLDER);
                    File.WriteAllText(Path.Combine(FOLDER, $"Posts-{++fileCount}.json"), json);
                    Browser.ExecuteScriptAsync(RemoveDivs);
                    Browser.ExecuteScriptAsync(TimerJs);
                    pageDownTimer.Start();
                    Post post = posts.LastOrDefault();
                    if (post.Date.Contains((DateTime.Now.Year - 8).ToString()))
                    {
                        pageDownTimer.Stop();
                        PageCount = 0;
                        Browser.ExecuteScriptAsync(TimerClear);
                    }
                }));
            }
        }

        private void BtnPageDown_Click(object sender, RoutedEventArgs e)
        {
            if (!pageDownTimer.Enabled)
            {
                Browser.ExecuteScriptAsync(TimerJs);
                pageDownTimer.Start();
            }

        }

        private void BtnPageDownStop_Click(object sender, RoutedEventArgs e)
        {
            if (pageDownTimer.Enabled)
            {
                Browser.ExecuteScriptAsync(TimerClear);
                pageDownTimer.Stop();
            }
        }

    }
}
