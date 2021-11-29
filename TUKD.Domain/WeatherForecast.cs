using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TUKD.Domain
{
    public class WeatherForecast
    {
        [Required]
        public DateTime? Date { get; set; } = DateTime.UtcNow;

        public int TemperatureC { get; set; }

        [Required]
        public string? Summary { get; set; }

        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}
