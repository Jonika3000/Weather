using ControlzEx.Theming;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace Weather
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow  
    {
        DateTime now = DateTime.Now;
        List<Day> weekend = new List<Day>();
        WebClient webClient = new WebClient();
         string city;
         string url = $"https://ua.sinoptik.ua/погода-";
        public MainWindow()
        {
            InitializeComponent();
              SetBackgroundImage();
            GetCityStart();
            SetNowPage();
            SetWeekend();
            SetListWeekend();
            SetToday();
             
          
         } 
        private void GetCityStart()
        {
            try
            {
                using (FileStream fs = new FileStream("city.json", FileMode.OpenOrCreate))
                {
                    city = JsonSerializer.Deserialize<string>(fs);
                }
            }
            catch
            {
                city = "Киев";
            }
            if (city == string.Empty || city == null)
            {
                city = "Киев";
            }
        }
        private void SetToday()
        {
            
            Town.Text =  Regex.Replace(city.ToLower(), @"\b[a-zа-яё]", m => m.Value.ToUpper());
            string Month = GetMonthString();
            string Day = GetDayString();
            var result = webClient.DownloadString($"{url + city}/{now.Year}-{Month}-{Day}");
            Regex r = new Regex($"<p class=\"today-temp\">(.*?)&deg;C</p>");
            MatchCollection m = r.Matches(result);
            foreach (Match x in m)
            {
                weekend[0].month_hight = x.Groups[1].Value;
                weekend[0].month_low = x.Groups[1].Value;
                
            } 
        }
        private void SetBackgroundImage()
        {
            ImageBrush myBrush = new ImageBrush();
            if (DateTime.Now.Hour >= 6 && DateTime.Now.Hour <= 13)
            {
                myBrush.ImageSource =
                    new BitmapImage(new Uri("pack://application:,,,/Image/bk8.jpg", UriKind.Absolute));
           
            }
            else if (DateTime.Now.Hour >= 13 && DateTime.Now.Hour <= 19)
            {
                myBrush.ImageSource =
                  new BitmapImage(new Uri("pack://application:,,,/Image/bk6.jpg", UriKind.Absolute));
             
            }
            else
            {
                myBrush.ImageSource =
                    new BitmapImage(new Uri("pack://application:,,,/Image/bk7.jpg", UriKind.Absolute));
          
            }
            this.Background = myBrush;
        }
  
        private void SetListWeekend()
        {
            StackPanelDays.Children.Clear();
            foreach (var day in weekend)
            {
                Button button = new Button();
                button.Click += Button_Click1;
                 
                if(Convert.ToDateTime(day.date) == DateTime.Now.Date)
                {
                    button.Style = Resources["ButtonToday"] as Style;
                    
                }
                else
                {
                    button.Style = Resources["ButtonDays"] as Style;
                }
                 StackPanel stackPanel = new StackPanel();
                stackPanel.Background = Brushes.Transparent;
                stackPanel.Orientation = Orientation.Vertical;

                TextBlock textBlock1 = new TextBlock();
                textBlock1.Style = Resources["TextBlockDay"] as Style;
                if (Convert.ToDateTime(day.date) == DateTime.Now.Date)
                {
                    textBlock1.Text = "Зараз";
                }
                else
                {
                    textBlock1.Text = $"{day.date_day} {day.day}";
                }
                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                img.Source = new BitmapImage(new Uri(day.source_image, UriKind.Relative) );
                img.Height = 50;
                img.Width = 50;
                img.HorizontalAlignment = HorizontalAlignment.Center;

                TextBlock textBlock2 = new TextBlock();
                textBlock2.Style = Resources["TextBlockDay"] as Style;
                textBlock2.Text = $"{(Convert.ToInt32(day.month_low)+ Convert.ToInt32( day.month_hight))/2}°";
 
                stackPanel.Children.Add(textBlock1);
                stackPanel.Children.Add(img);
                stackPanel.Children.Add(textBlock2);
                

                button.Tag = day.date;
                button.Content = stackPanel;
                StackPanelDays.Children.Add(button);
            }
        }
        private void SetNowPage()
        {
            now = DateTime.Now;
            string Month = GetMonthString();
            string Day = GetDayString();
            var result = webClient.DownloadString($"{url + city}/{now.Year}-{Month}-{Day}");
            Page_Details page = new Page_Details(result);
            this.Container.Navigate(page);
        }
        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button; 
            var result = webClient.DownloadString($"{url + city}/{button.Tag}");
            Page_Details page = new Page_Details(result);
            this.Container.Navigate(page);
        }
 
        private void SetWeekend()
        {
            //https://ua.sinoptik.ua/%D0%BF%D0%BE%D0%B3%D0%BE%D0%B4%D0%B0-%D0%BB%D1%83%D1%86%D1%8C%D0%BA
            weekend.Clear();
            now = DateTime.Now;
             for (int i = 0; i < 7; i++)
            {
                string Month = GetMonthString();
                string Day = GetDayString();
                var result = webClient.DownloadString($"{url + city}/{now.Year}-{Month}-{Day}");
                Day day = new Day();
                Regex r = new Regex($"<p class=\"day-link\".*?\">(.*?)</p>");
                MatchCollection m = r.Matches(result);
                foreach (Match x in m)
                {
                    day.day = x.Groups[1].Value;
                }
                day.date_day = Day;
                day.date_month = Month;
                day.date = $"{now.Year}-{Month}-{Day}";
                day.month_low = GetTempLow(Day, result);
                day.month_hight = GetTempHigh(Day, result);

                day.source_image = GetImage(Day,result);
                
                weekend.Add(day);
                now = now.AddDays(1);
            }
        }
        private string GetImage(string Day , string result)
        {
            Regex r = new Regex($"<p class=\"date \">{Day}</p>.*?title=\"(.*?)\">");
            MatchCollection m = r.Matches(result);
            string res = string.Empty;
            foreach (Match x in m)
            {
                res = Image.GetImage(x.Groups[1].Value);
            }
            if (res == null || res == string.Empty)
            {
                r = new Regex($"<p class=\"date dateFree\">{Day}</p>.*?title=\"(.*?)\">");
                m = r.Matches(result);
                foreach (Match x in m)
                {
                    res = Image.GetImage(x.Groups[1].Value);
                }
            }
            return res;
        }
        private string GetTempLow(string Day , string result)
        {
            Regex r = new Regex($"<p class=\"date \">{Day}</p>(.*?)<div class=\"min\">мін. <span>(.*?)&deg;</span></div>");
            MatchCollection m = r.Matches(result);
            string res = string.Empty;
            foreach (Match x in m)
            {
                res = x.Groups[2].Value;
            }
            if (res == null || res == string.Empty)
            {
                r = new Regex($"<p class=\"date dateFree\">{Day}</p>(.*?)<div class=\"min\">мін. <span>(.*?)&deg;</span></div>");
                m = r.Matches(result);
                foreach (Match x in m)
                {
                    res = x.Groups[2].Value;
                }
            }
            return res;
        }
        private string GetTempHigh(string Day, string result)
        {
            Regex r = new Regex($"<p class=\"date dateFree\">{Day}</p>.*?<div class=\"max\">макс. <span>(.*?)&deg;</span></div>");
            MatchCollection m = r.Matches(result);
            string res = string.Empty;
            foreach (Match x in m)
            {
                res = x.Groups[1].Value;
            }
            if (res == null || res == string.Empty)
            {
                r = new Regex($"<p class=\"date \">{Day}</p>.*?<div class=\"max\">макс. <span>(.*?)&deg;</span></div>");
                m = r.Matches(result);
                foreach (Match x in m)
                {
                    res = x.Groups[1].Value;
                }
            }
            return res;
        }
        private string GetDayString()
        {
            string Day = now.Day.ToString();
            if (Convert.ToInt32(Day) < 10)
            {
                Day = "0" + Day;
            }
            return Day;
        }
        private string GetMonthString()
        {
            string Month = now.Month.ToString();
            if (Convert.ToInt32(Month) < 10)
            {
                Month = "0" + Month;
            }
            return Month;
        }
        private void MonthSet()
        {
            Border mainborder = SetBorder();
        }
        private void SetTempNow()
        {
            now = DateTime.Now;
        }
        private Border SetBorder()
        {
            Border border = new Border();
            border.Background = new SolidColorBrush(Colors.White);
            border.BorderThickness = new Thickness(10);
            border.BorderBrush = new SolidColorBrush(Colors.White);
            border.CornerRadius = new CornerRadius(15);
            border.Width = 150;
            border.Height = 150;
            return border;
        }
 

        private void UpdateButtonClick(object sender, RoutedEventArgs e)
        {
            SetBackgroundImage();
            SetNowPage();
            SetToday();
            SetWeekend();
            SetListWeekend();
        }

      

        private async void TextBox_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
               
               
                SearchTermTextBox.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                string Month = GetMonthString();
                string Day = GetDayString();
                try
                {
                    var result = webClient.DownloadString($"{url + SearchTermTextBox.Text}/{now.Year}-{Month}-{Day}");
                    city = SearchTermTextBox.Text;
                    
                }
                catch
                {
                    SearchTermTextBox.Text = string.Empty;
                    //await this.ShowMessageAsync("Error 404", "This city was not found."); дописать
                    return; 
                }
                SearchTermTextBox.Text = string.Empty;
                SetToday();
                SetWeekend();
                SetListWeekend();
                SetNowPage();
            }
        }

        private async void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
         
            using (FileStream fs = new FileStream("city.json", FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync<string>(fs, city);
            }
        }

        private void Button_Click_Stop(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click_Max(object sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.WindowState != WindowState.Maximized)
                Application.Current.MainWindow.WindowState = WindowState.Maximized;
            else
                Application.Current.MainWindow.WindowState = WindowState.Normal;
        }

        private void Button_Clicl_Minimize(object sender, RoutedEventArgs e)
        {
            Application.Current.MainWindow.WindowState = WindowState.Minimized;
        }

       

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }
    }
}
