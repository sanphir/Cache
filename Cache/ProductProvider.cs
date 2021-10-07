using System;
using System.Collections.Generic;
using System.Linq;

namespace Cache
{
	public class ProductProvider : IGetItem<int, Product>
	{
		private readonly List<Product> _store = new List<Product>()
		{
			new Product { Id=1, Name="Куртка" },
			new Product { Id=2, Name="Джинсы" },
			new Product { Id=3, Name="Рубашка" },
		};

		public Product GetItem(int key)
		{
			return _store.Where(r => r.Id == key).FirstOrDefault();
		}

		public bool SetItem(Product product)
		{
			try
			{
				var item = _store.Where(r => r.Id == product.Id).FirstOrDefault();
				if (item == null)
				{
					_store.Add(product);
				}
				return true;
			}
			catch (Exception)
			{
				return false;
			}
		}
	}
}
