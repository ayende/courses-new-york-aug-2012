namespace NewYork.Models
{
	public class Shirt
	{
		public string Name { get; set; }
		public string Id { get; set; }
		public ShirtType[] Types { get; set; }
	}

	public class ShirtType
	{
		public string Size { get; set; }
		public string Color { get; set; }
	}
}