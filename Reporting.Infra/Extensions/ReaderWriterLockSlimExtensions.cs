namespace Tizpusoft;

public static class ReaderWriterLockSlimExtensions
{
    public static void Write(this ReaderWriterLockSlim @lock, Action action)
    {
        @lock.EnterWriteLock();
        try
        {
            action?.Invoke();
        }
        finally { @lock.ExitWriteLock(); }
    }

    public static T Write<T>(this ReaderWriterLockSlim @lock, Func<T> func)
    {
        @lock.EnterWriteLock();
        try
        {
            return func.Invoke();
        }
        finally { @lock.ExitWriteLock(); }
    }

    public static void Read(this ReaderWriterLockSlim @lock, Action action)
    {
        @lock.EnterReadLock();
        try
        {
            action?.Invoke();
        }
        finally { @lock.ExitReadLock(); }
    }

    public static T Read<T>(this ReaderWriterLockSlim @lock, Func<T> func)
    {
        @lock.EnterReadLock();
        try
        {
            return func.Invoke();
        }
        finally { @lock.ExitReadLock(); }
    }
}
