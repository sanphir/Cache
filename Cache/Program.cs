using System;

namespace Cache
{
	class Program
	{
		static void Main(string[] args)
		{
			var productProvider = new ProductProvider();
			var cache = new CachingService<int, Product>(casheSize: 2, lifeTime: TimeSpan.FromSeconds(30), getItem: productProvider);

			cache.GetItem(1);
			cache.GetItem(2);
			cache.GetItem(3);

			Console.ReadLine();
		}
	}
}
