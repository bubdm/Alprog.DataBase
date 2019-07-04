using System;

namespace Alprog.DataBase
{
    public class DataContextLocker<T> : IDisposable
    {
        public IDataContext Context { get; private set; }
        public DataContextLocker(IDataContext context, bool lockNow = true)
        {
            Context = context;

        }
        public DataContextLocker(IDataProvider<T> provider, bool lockNow = true)
            : this(provider.Context, lockNow) { }

        public void Lock()
        {
            Context.SyncBlock();
        }
        public void Release()
        {
            Context.SyncRelease();
        }

        #region IDisposable Support
        private bool disposedValue = false; // Для определения избыточных вызовов

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Release();
                }



                disposedValue = true;
            }
        }


        ~DataContextLocker()
        {

            Dispose(true);
        }

        // Этот код добавлен для правильной реализации шаблона высвобождаемого класса.
        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки выше, в методе Dispose(bool disposing).
            Dispose(true);
            // TODO: раскомментировать следующую строку, если метод завершения переопределен выше.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
