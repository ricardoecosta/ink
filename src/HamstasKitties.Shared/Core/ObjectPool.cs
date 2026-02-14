using System;
using System.Collections.Generic;

namespace HamstasKitties.Core;

/// <summary>
/// Generic object pool for reducing garbage collection pressure.
/// Reuses objects instead of creating new ones.
/// </summary>
/// <typeparam name="T">The type of object to pool</typeparam>
public class ObjectPool<T> where T : class, new()
{
    private readonly Stack<T> _pool;
    private readonly Action<T>? _resetAction;
    private readonly int _maxSize;

    public ObjectPool(int initialSize = 16, int maxSize = 256, Action<T>? resetAction = null)
    {
        _pool = new Stack<T>(initialSize);
        _maxSize = maxSize;
        _resetAction = resetAction;

        // Pre-populate pool
        for (int i = 0; i < initialSize; i++)
        {
            _pool.Push(new T());
        }
    }

    public int Count => _pool.Count;

    /// <summary>
    /// Get an object from the pool, or create a new one if empty.
    /// </summary>
    public T Get()
    {
        if (_pool.Count > 0)
        {
            return _pool.Pop();
        }
        return new T();
    }

    /// <summary>
    /// Return an object to the pool for reuse.
    /// </summary>
    public void Return(T obj)
    {
        if (obj == null) return;
        if (_pool.Count >= _maxSize) return;

        _resetAction?.Invoke(obj);
        _pool.Push(obj);
    }

    /// <summary>
    /// Clear all objects from the pool.
    /// </summary>
    public void Clear()
    {
        _pool.Clear();
    }
}

/// <summary>
/// Pool for reusable timer objects.
/// </summary>
public class TimerPool : ObjectPool<HamstasKitties.Animation.Timer>
{
    public TimerPool(int initialSize = 32, int maxSize = 128)
        : base(initialSize, maxSize, timer =>
        {
            timer.Stop();
            timer.RedefineTimerDuration(0f);
        })
    {
    }

    public HamstasKitties.Animation.Timer Get(float duration)
    {
        var timer = Get();
        timer.RedefineTimerDuration(duration);
        return timer;
    }
}
