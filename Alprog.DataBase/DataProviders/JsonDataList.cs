using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Newtonsoft.Json;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;
namespace Alprog.DataBase.DataProviders
{
    public class JsonDataList : JsonDataEnumerable, IData
    {
        public JsonDataList(JsonDataProvider provider) : base(provider)
        {

        }

        public int Count
        {
            get
            {
                int count = 0;
                foreach (object i in this)
                {
                    count++;
                }
                return count;
            }
        }

        public bool IsReadOnly { get { return false; } }

        public bool IsSynchronized { get { return false; } }

        public object SyncRoot { get { return null; } }

        public void Add(object item)
        {
            Provider.Context.SyncWaiter.WaitOne();
            Provider.Context.SyncBlock();
            Stream stream = Provider.Context.File.Open(FileMode.OpenOrCreate, FileAccess.ReadWrite);
            JsonTextWriter writer =
                new JsonTextWriter(new StreamWriter(stream));
            JsonTextReader reader =
                new JsonTextReader(new StreamReader(stream));
            int openCount = 0;
            while (reader.Read())
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    openCount++;
                }
                else if (reader.TokenType == JsonToken.EndArray)
                {
                    openCount--;
                    if (openCount == 0)
                        break;
                }
            }
            JsonSerializer serializer = new JsonSerializer();
            stream.Position -= 1;
            writer.WriteRaw(", ");
            serializer.Serialize(writer, item);
            writer.WriteRaw("]");
            writer.Flush();
            stream.Close();
            Provider.Context.SyncRelease();
        }

        public void Clear()
        {
            Provider.Context.SyncWaiter.WaitOne();
            Provider.Context.SyncBlock();

            Stream stream = Provider.Context.File.Create();
            JsonTextWriter writer = new JsonTextWriter(new StreamWriter(stream));
            writer.WriteStartArray();
            writer.WriteEndArray();
            writer.Flush();

            stream.Close();

            Provider.Context.SyncRelease();
        }

        public bool Contains(object item)
        {
            foreach (object element in this)
            {
                if (element.Equals(item))
                {
                    return true;
                }
            }
            return false;
        }

        public void CopyTo(object[] array, int arrayIndex)
        {
            int i = arrayIndex;
            foreach (var item in this)
            {
                array[i] = item;
                i++;
                if (i >= array.Length)
                    break;
            }
        }

        public void CopyTo(Array array, int index)
        {
            int i = index;
            foreach (var item in this)
            {
                _ = array.SetValue(item, i);
                i++;
                if (i >= array.Length)
                    break;
            }
        }

        public bool Remove(object item)
        {
            Provider.Context.SyncWaiter.WaitOne();
            Provider.Context.SyncBlock();

            FileInfo tempFile = Provider.Context.File.CopyTo("temp", true);
            JsonDataProvider tempProvider = new JsonDataProvider(new JsonDataContext(tempFile));

            Stream stream = Provider.Context.File.Create();
            JsonTextWriter writer = new JsonTextWriter(new StreamWriter(stream));
            JsonSerializer serializer = new JsonSerializer();

            bool removed = false;
            writer.WriteStartArray();
            foreach (object element in tempProvider.Data)
            {
                if (item.Equals(element))
                {
                    removed = true;
                    continue;
                }
                serializer.Serialize(writer, element);
            }
            writer.WriteEndArray();
            writer.Flush();
            stream.Close();
            tempProvider.Context.Dispose();
            tempFile.Delete();

            Provider.Context.SyncRelease();
            return removed;
        }
    }
}
