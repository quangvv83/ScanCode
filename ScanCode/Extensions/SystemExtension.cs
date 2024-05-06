using System;
using System.Runtime.InteropServices;

namespace ScanCode.Extensions
{
    public static class SystemExtension
    {

        public static IDisposable DisposeWithReleaseComObject<T>(this T obj)
        {
            return DisposeWith(obj, (onDispose) => Marshal.ReleaseComObject(obj));
        }

        public static IDisposable DisposeWithReleaseComObject<T>(this T obj, out T outObj)
        {
            return DisposeWith(obj, (onDispose) => Marshal.ReleaseComObject(obj), out outObj);
        }

        public static IDisposable DisposeWith<T>(this T obj, Action<T> onDispose)
        {
            if (obj == null)
            {
                return Disposable.Empty;
            }
            return new Disposable(() => onDispose(obj));
        }

        public static IDisposable DisposeWith<T>(this T obj, Action<T> onDispose, out T outObj)
        {
            outObj = obj;
            return DisposeWith(obj, onDispose);
        }

        private class Disposable : IDisposable
        {
            public static Disposable Empty = new Disposable(null) { _disposed = true };
            private Action _onDispose;
            public Disposable(Action onDispose)
            {
                _onDispose = onDispose;
            }

            private bool _disposed;
            public void Dispose()
            {
                if (_disposed)
                {
                    return;
                }
                _onDispose?.Invoke();
                _disposed = true;
            }
        }

    }

}
