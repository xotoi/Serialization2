using System;
using System.Runtime.Serialization;
using Task.DB;

namespace Task.Surrogates
{
    public class OrderDetailSurrogateSelector : ISurrogateSelector
    {
        private ISurrogateSelector _nextSelector;

        public void ChainSelector(ISurrogateSelector selector)
        {
            _nextSelector = selector;
        }

        public ISerializationSurrogate GetSurrogate(Type type, StreamingContext context, out ISurrogateSelector selector)
        {
            selector = this;
            return type == typeof(Order_Detail) ? new OrderDetailSerializationSurrogate() : null;
        }

        public ISurrogateSelector GetNextSelector()
        {
            return _nextSelector;
        }
    }
}
