using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Weather
{
    /// <summary>
    /// Логика взаимодействия для Page_Details.xaml
    /// </summary>
    public partial class Page_Details : Page
    {
        string result_url;
        public Page_Details()
        {
            InitializeComponent();
        }
        public Page_Details(string result)
        {
            InitializeComponent();
            result_url = result;
            GetTimes();
        }
        private void GetTimes()
        {

            List<string> temps = new List<string>();
            List<string> tempsS = new List<string>();
            List<string> images = new List<string>();

            Regex r = new Regex($"<tr class=\"temperature\">.*?</tr>");
            MatchCollection m = r.Matches(result_url);
            string g = string.Empty;
            foreach (Match x in m)
            {
                g = (x.Groups[0].Value);
            }
            r = new Regex($"<td class=\".*?\"\\s*>(.*?)&deg;");
            m = r.Matches(g);
            foreach (Match x in m)
            {
                 temps.Add(x.Groups[1].Value);
            }

            r = new Regex($"<tr class=\"temperatureSens\">.*?</tr>");
              m = r.Matches(result_url);
              g = string.Empty;
            foreach (Match x in m)
            {
                g = (x.Groups[0].Value);
            }
            r = new Regex($"<td class=\".*?\"\\s*>(.*?)&deg;");
            m = r.Matches(g);
            foreach (Match x in m)
            {
                tempsS.Add(x.Groups[1].Value);
            }

            //<td class="p2 bR "> <div class="weatherIco n400" title="Хмарно">
            r = new Regex($"<tr class=\"img weatherIcoS\">(.*?)</tr>");
            m = r.Matches(result_url);
            foreach (Match x in m)
            {
                g = (x.Groups[1].Value);
            }
            r = new Regex($"title=\"(.*?)\">");
            m = r.Matches(g);
            foreach (Match x in m)
            {
                images.Add(x.Groups[1].Value);
            }


            DateTime t = new DateTime();
            if (temps.Count == 4)
            {
                t = t.AddHours(3);
            }
            for (int i = 0; i < temps.Count; i++)
            {
                StackPanel stack = new StackPanel();
                stack.Orientation = Orientation.Vertical;
                stack.HorizontalAlignment = HorizontalAlignment.Center;
                stack.Margin = new Thickness(15);

                Button button = new Button();
                button.IsEnabled = false;
                button.Style = Resources["Details"] as Style;

                TextBlock text = new TextBlock();
                text.Text = $"{t.Hour}:00";
                text.Style = Resources["TextBlockStatic"] as Style;

                System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                img.Source = new BitmapImage(new Uri(Image.GetImage(images[i]), UriKind.Relative));
                img.Height = 50;
                img.Width = 50;
 
                TextBlock text1 = new TextBlock();
                text1.Text = $"{temps[i]}°C";
                text1.Style = Resources["TextBlockStatic"] as Style;

                TextBlock text2 = new TextBlock();
                text2.Text = $"{tempsS[i]}°C";
                text2.Style = Resources["TextBlockStatic"] as Style;
                
                stack.Children.Add(text);
                stack.Children.Add(img);
                stack.Children.Add(text1);
                stack.Children.Add(text2);
                button.Content = stack;
                mainstack.Children.Add(button);

                t = t.AddHours(3);
            }
        }
    }
}
