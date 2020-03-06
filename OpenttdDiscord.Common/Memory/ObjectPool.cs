using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common.Memory
{
    public class ObjectPool<T> : IObjectPool<T>
        where T:class
    {
        private readonly IFactory<T> factory;
        private readonly ConcurrentBag<T> pool = new ConcurrentBag<T>();

        public ObjectPool(IFactory<T> factory)
        {
            this.factory = factory;
        }

        public ObjectPoolItem<T> GetObject()
        {
            if (this.pool.TryTake(out T item))
                return new ObjectPoolItem<T>(item, this);
            item = factory.Create();
            this.pool.Add(item);

            return new ObjectPoolItem<T>(item, this);
        }

        public void PutObject(T item)
        {
            this.pool.Add(item);
        }
    }
}
