namespace TicketManager.Util
{
    public class LazySingleton<T>
    {
        private static T? _instance;
        private static readonly object _lock = new object();
        private static Func<T>? _instanceCreator;

        protected LazySingleton() { }

        public static void Initialize(Func<T> instanceCreator)
        {
            _instanceCreator = instanceCreator ?? throw new ArgumentNullException(nameof(instanceCreator));
        }

        public static T GetInstance()
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        if (_instanceCreator == null)
                        {
                            throw new InvalidOperationException("LazySingleton not initialized. Call Initialize first.");
                        }
                        _instance = _instanceCreator();
                    }
                }
            }
            return _instance;
        }
    }
}
