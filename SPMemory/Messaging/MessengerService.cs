namespace SPMemory.Messaging
{
    public class MessengerService
    {
        private MessengerService() { }

        private static MessengerService? _instance = null;
        public static MessengerService Instance => _instance ??= new MessengerService();

        public Dictionary<Type, Dictionary<object, object>> Subscriptions { get; } = new Dictionary<Type, Dictionary<object, object>>();

        public void Subscribe<TMessage>(object subscriber, Action<object, TMessage> processMessage)
        {
            if (Subscriptions.TryGetValue(typeof(TMessage), out var subscriptions))
            {
                subscriptions[subscriber] = processMessage;
            }
            else
            {
                Subscriptions.Add(typeof(TMessage), new Dictionary<object, object>()
                {
                    { subscriber, processMessage }
                });
            }
        }

        public void Unsubscribe<TMessage>(object subscriber)
        {
            if (!Subscriptions.TryGetValue(typeof(TMessage), out var subscriptions) || !subscriptions.ContainsKey(subscriber))
            {
                throw new ArgumentException($"No subscription to message {typeof(TMessage).FullName} is known for this subscriber");
            }
            subscriptions.Remove(subscriber);
        }

        public void SendMessage<TMessage>(object sender, TMessage message)
        {
            if (Subscriptions.TryGetValue(typeof(TMessage), out var subscriptions))
            {
                foreach (Action<object, TMessage> processMessage in subscriptions.Values.ToList())
                {
                    processMessage(sender, message);
                }
            }
        }
    }
}
