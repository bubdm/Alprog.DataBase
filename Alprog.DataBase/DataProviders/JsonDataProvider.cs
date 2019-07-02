using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Alprog.DataBase.DataProviders
{
    public class JsonDataContext : IDataContext
    {
        public JsonDataContext()
        {
            waiter = new EventWaitHandle(false, EventResetMode.ManualReset);
            SyncRelease();
        }
        public JsonDataContext(FileInfo file) : this()
        {
            File = file;
        }
        public FileInfo File { get; set; }
        public void Init()
        {
            Contexts.Add(File, this);
        }
        EventWaitHandle waiter;
        public static Hashtable Contexts { get; private set; } = new Hashtable();
        public WaitHandle SyncWaiter { get { return waiter; } }
        public event EventHandler OnSyncBlock;
        public event EventHandler OnSyncRelease;

        public void SyncBlock()
        {
            waiter.Reset();
            OnSyncBlock?.Invoke(this, null);
        }

        public void SyncRelease()
        {
            waiter.Set();
            OnSyncRelease?.Invoke(this, null);
        }

        bool isDisposed = false;
        public void Dispose()
        {
            if (isDisposed)
                return;
            Contexts.Remove(File);
        }

        ~JsonDataContext()
        {
            Dispose();
        }
    }
    public class JsonDataProvider : IDataProvider
    {
        public JsonDataProvider(JsonDataContext context)
        {
            ProvideContext(context);
        }

        bool ProvideContext(JsonDataContext context)
        {
            bool contains = JsonDataContext.Contexts.ContainsKey(context.File);
            if (contains)
            {
                Context = (JsonDataContext)JsonDataContext.Contexts[context.File];
                return false;
            }
            else
            {
                context.Init();
                Context = context;
                return true;
            }
        }

        public void Save()
        {
            Context.SyncWaiter.WaitOne();

            List<object> items = Data.ToList();

            Context.SyncBlock();

            
            Stream stream = Context.File.Create();
            JsonTextWriter writer = new JsonTextWriter(new StreamWriter(stream));
            JsonSerializer serializer = new JsonSerializer();

            writer.WriteStartArray();

            foreach (object item in items)
            {
                serializer.Serialize(writer, item);
                writer.Flush();
            }
            items = null;
            writer.WriteEndArray();
            Context.SyncRelease();
        }

        public IDataContext Context { get; private set; }

        JsonDataList data = null;
        public IData Data
        {
            get
            {
                data = new JsonDataList(this);
                return data;
            }
        }
    }
}
