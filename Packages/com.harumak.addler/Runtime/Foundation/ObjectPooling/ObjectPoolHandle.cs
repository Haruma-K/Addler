namespace Addler.Runtime.Foundation.ObjectPooling
{
    /// <summary>
    ///     Handle for handling objects retrieved from the pool.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public readonly struct ObjectPoolHandle<T>
    {
        public ObjectPoolHandle(int id, T instance)
        {
            Id = id;
            Instance = instance;
        }

        public int Id { get; }
        public T Instance { get; }
    }
}