using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Alprog.DataBase
{
    public interface IDataContext : IDisposable
    {
        FileInfo File { get; set; }
        WaitHandle SyncWaiter { get; }
        void Init();

        event EventHandler OnSyncBlock;
        event EventHandler OnSyncRelease;

        void SyncBlock();
        void SyncRelease();
    }
}
