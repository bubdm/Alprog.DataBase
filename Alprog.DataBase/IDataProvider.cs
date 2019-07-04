using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Alprog.DataBase
{
    public interface IDataProvider<T>
    {
        IDataContext Context { get; }
        IData<T> Data { get; }
        void Save();
    }
    public interface IData<T> : ICollection<T>, IEnumerable<T>
    {
    }
}
