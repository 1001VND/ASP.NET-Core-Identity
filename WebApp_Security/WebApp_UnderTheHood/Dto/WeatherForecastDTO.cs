namespace WebApp_UnderTheHood.Dto
{
	public class WeatherForecastDTO
	{
		public DateOnly Date { get; set; }

		public int TemperatureC { get; set; }

		public int TemperatureF { get; set; }

		public string? Summary { get; set; }
	}
}
