using CefSharp;
using FacebookPageSdk.Entities.Concrete;
using FacebookPageSdk.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Timers;
using System.Windows;

namespace FacebookPageSdk.WpfUi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public readonly string FacebookUrl = "https://m.facebook.com";
        public readonly System.Timers.Timer pageDownTimer;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            pageDownTimer = new System.Timers.Timer()
            {
                Interval = TimeSpan.FromMilliseconds(700).TotalMilliseconds,
            };
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Title = $"Recep Berat";
            Browser.Address = $"{FacebookUrl}/YeniSafak";
            BtnPageDown.Click += BtnPageDown_Click;
            BtnPageDownStop.Click += BtnPageDown_Click;
            BtnDownload.Click += BtnDownload_Click;
            pageDownTimer.Elapsed += PageDownTimer_Elapsed;
            dg.SelectionChanged += Dg_SelectionChanged;
            //ThreadPool.QueueUserWorkItem(new WaitCallback((callback) =>
            //{
            //    List<Post> Oldposts = JsonConvert.DeserializeObject<List<Post>>(File.ReadAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Posts.json")));
            //    dg.Dispatcher.Invoke(() =>
            //    {
            //        dg.ItemsSource = Oldposts;
            //    });

            //}));
        }

        private void Dg_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (dg.SelectedItem is Post post)
                Browser.Address = $"{FacebookUrl}{post.DetailPageLink}";
        }

        private void PageDownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Browser.ExecuteScriptAsync("window.scrollTo(0, document.body.scrollHeight);");
        }

        private void BtnDownload_Click(object sender, RoutedEventArgs e)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((callback) =>
            {
                string source = Browser.GetSourceAsync().Result;
                FacebookPageService pageService = new FacebookPageService(source);
                var posts = pageService.GetPosts();
                dg.Dispatcher.Invoke(() =>
                {
                    dg.ItemsSource = posts;
                });
                var json = JsonConvert.SerializeObject(posts,Formatting.Indented);
                File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "Posts.json"), json);
            }));

        }


        private void BtnPageDown_Click(object sender, RoutedEventArgs e)
        {
            if (!pageDownTimer.Enabled)
                pageDownTimer.Start();
            else
                pageDownTimer.Stop();
        }

    }
}
