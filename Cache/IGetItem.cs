using System;
using System.Collections.Generic;
using System.Text;

namespace Cache
{
	public interface IGetItem<in TKey, out TItem>
	{
		public TItem GetItem(TKey key);
	}
}
