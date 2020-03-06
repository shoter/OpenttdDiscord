using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common.Memory
{
    public class ObjectPoolItem<T> : IAsyncDisposable
        where T:class
    {
        public T Item { get; }
        private readonly IObjectPool<T> origin;
        private bool disposed = false;

        public ObjectPoolItem(T item, IObjectPool<T> origin)
        {
            this.Item = item;
            this.origin = origin;
        }

        public async ValueTask DisposeAsync()
        {
            if (disposed == false)
            {
                disposed = true;
                origin.PutObject(this.Item);
            }
        }
    }
}
