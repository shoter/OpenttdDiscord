using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenttdDiscord.Common.Memory
{
    public interface IObjectPool<T>
        where T : class
    {
        ObjectPoolItem<T> GetObject();
        void PutObject(T item);
    }
}
