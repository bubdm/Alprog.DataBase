﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace Alprog.DataBase.DataProviders
{
    public class JsonDataEnumerator : IEnumerator<object>, IEnumerator
    {
        Stream stream = null;
        WaitHandle syncwaiter;
        public JsonDataEnumerator(JsonDataProvider provider)
        {
            Provider = provider;
            syncwaiter = provider.Context.SyncWaiter;
            Provider.Context.OnSyncBlock += (s, e) =>
            {
                stream.Close();
            };
            Provider.Context.OnSyncRelease += (s, e) =>
            {
                stream = Provider.Context.File.OpenRead();
            };
            Reset();
        }

        public JsonDataEnumerator()
        {
        }

        public JsonDataProvider Provider { get; private set; }

        JsonTextReader reader;
        JsonSerializer serializer;
        public object Current { get; private set; }

        bool isDisposed = false;
        public void Dispose()
        {
            if (!isDisposed)
            {
                stream.Close();
                serializer = null;
                reader = null;
                isDisposed = true;
            }
        }

        public bool MoveNext()
        {
            syncwaiter.WaitOne();
            bool canRead = false;

            canRead = reader.Read();
            if (!canRead)
                return false;

            while (reader.TokenType != JsonToken.StartObject &&
                reader.TokenType != JsonToken.String &&
                reader.TokenType != JsonToken.Integer &&
                reader.TokenType != JsonToken.Float)
            {
                canRead = reader.Read();
                if (!canRead)
                    return false;
            }
            Current = serializer.Deserialize(reader);
            
            return true;
        }

        public void Reset()
        {
            syncwaiter.WaitOne();
            stream = Provider.Context.File.OpenRead();
            reader = new JsonTextReader(new StreamReader(stream));
            serializer = new JsonSerializer();
        }
    }

    public class JsonDataEnumerable : IEnumerable<object>
    {
        public JsonDataEnumerable(JsonDataProvider provider)
        {
            Provider = provider;
        }
        public JsonDataProvider Provider { get; private set; }
        public IEnumerator<dynamic> GetEnumerator()
        {
            return new JsonDataEnumerator(Provider);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new JsonDataEnumerator(Provider);
        }
    }


}
