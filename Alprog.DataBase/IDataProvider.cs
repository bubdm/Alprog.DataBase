using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Alprog.DataBase
{
    public interface IDataProvider
    {
        IDataContext Context { get; }
        IData Data { get; }
        void Save();
    }
    public interface IData : ICollection<object>, IEnumerable<object>
    {
    }
}
