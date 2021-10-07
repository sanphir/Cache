using System;
using System.Collections.Generic;
using System.Linq;

namespace Cache
{
	public class CachingService<TKey, TItem>
	{
		readonly int _casheSize;
		readonly TimeSpan _lifeTime;
		readonly IGetItem<TKey, TItem> _getItem;

		readonly Dictionary<TKey, CahcingItem> _cache = new Dictionary<TKey, CahcingItem>();

		class CahcingItem : IComparable
		{
			public TItem Item { get; set; }
			public byte Prioritet { get; set; }
			public DateTime LastAccess { get; set; }
			public DateTime FirstAccess { get; set; }

			public int CompareTo(object obj)
			{
				if (obj is CahcingItem comparedItem)
				{
					if (Prioritet == comparedItem.Prioritet)
					{
						return LastAccess.CompareTo(comparedItem.Prioritet);
					}
					else
					{
						return Prioritet.CompareTo(comparedItem.Prioritet);
					}
				}
				throw new ArgumentException();
			}
		}

		public CachingService(int casheSize, TimeSpan lifeTime, IGetItem<TKey, TItem> getItem)
		{
			_casheSize = casheSize;
			_lifeTime = lifeTime;
			_getItem = getItem ?? throw new ArgumentNullException(nameof(getItem));
		}

		public TItem GetItem(TKey key)
		{
			RemoveObsoletteItems();
			if (_cache.ContainsKey(key))
			{
				var cacheValue = _cache[key];
				_cache[key].LastAccess = DateTime.Now;
				if (cacheValue.Prioritet < 255)
				{
					cacheValue.Prioritet++;
				}
				return cacheValue.Item;
			}
			else
			{
				var item = _getItem.GetItem(key);
				if (item == null) return default(TItem);

				//remove cache item with minimum priority if cache size is reached
				while (_cache.Count >= _casheSize)
				{
					var minItem = _cache.Min(r => r.Value);
					var itemToRemove = _cache.Where(r => r.Value == minItem).FirstOrDefault();
					_cache.Remove(itemToRemove.Key);
				}

				AddToCache(key, item);
			}

			return default(TItem);
		}

		public void SetItem(TKey key, TItem item)
		{
			RemoveObsoletteItems();
			if (_cache.ContainsKey(key))
			{
				var cacheValue = _cache[key];
				_cache[key].FirstAccess = DateTime.Now;
				_cache[key].LastAccess = DateTime.Now;
				if (cacheValue.Prioritet < 255)
				{
					cacheValue.Prioritet++;
				}
			}
			else
			{
				AddToCache(key, item);
			}
		}

		private void AddToCache(TKey key, TItem item)
		{
			var now = DateTime.Now;
			_cache.Add(key, new CahcingItem
			{
				LastAccess = now,
				FirstAccess = now,
				Item = item,
				Prioritet = 1
			});
		}

		private void RemoveObsoletteItems()
		{
			var dateTimeNow = DateTime.Now;
			foreach (var item in _cache.Where(r => (DateTime.Now - r.Value.FirstAccess) > _lifeTime))
			{
				_cache.Remove(item.Key);
			}
		}
	}
}
