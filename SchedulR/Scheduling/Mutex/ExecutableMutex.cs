namespace SchedulR.Scheduling.Mutex;

internal class ExecutableMutex
{
    private readonly HashSet<string> _mutexCollection = [];
    private readonly object _mutexLock = new();

    /// <summary>
    /// Attempts to acquire a mutex lock for the given key. If a lock is acquired, returns true, otherwise returns false.
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public bool TryAcquire(string key)
    {
        lock (_mutexLock)
        {
            return _mutexCollection.Add(key);
        }
    }
    /// <summary>
    /// Releases the mutex lock for the given key.
    /// </summary>
    /// <param name="key"></param>
    public void Release(string key)
    {
        lock (_mutexLock)
        {
            _mutexCollection.Remove(key);
        }
    }
}
