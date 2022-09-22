using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weather
{
    static class Image
    {
      public  static string GetImage(string status)
        {
           
            if (status.ToLower().Contains("ясно"))
            {
                return "/Icons_Weather/icons8_sun_40px.png";
            }
            else if (status.ToLower().Contains("дощ"))
            {
                return "/Icons_Weather/icons8_rain_40px.png";
            }
            else if (status.ToLower().Contains("проясненнями") || status.ToLower().Contains("хмарність") )
            {
                return "/Icons_Weather/icons8_partly_cloudy_day_40px.png";
            }
            else if (status.ToLower().Contains("хмарність") || status.ToLower().Contains("хмарно") || status.ToLower().Contains("туман"))
            {
                return "/Icons_Weather/icons8_windy_weather_40px.png";
            }
            else if (status.ToLower().Contains("сніг"))
            {
                return "/Icons_Weather/icons8_snow_40px.png";
            }
            else if (status.ToLower().Contains("гроза"))
            {
                return "/Icons_Weather/icons8_storm_40px.png";
            }
            return "Error";
            //<div class="weatherIco d300" title="Хмарно з проясненнями"><img class="weatherImg" src="//sinst.fwdcdn.com/img/weatherImg/m/d300.gif" alt=""> </div>
        }
    }
}
