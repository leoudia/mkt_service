using Streaming.Crystal.Model.Base;
using Streaming.Crystal.Model.Model.Serialize;

namespace Streaming.Crystal.Notification.Cache
{
    public interface IMessageCache<TMessageBase, TMessageSerialize> where TMessageBase : MessageBase where TMessageSerialize : MessageSerialize
    {
        string Id { get; }
        TMessageSerialize Add(TMessageBase message);
        void Clear();

        BookMessageSerialize Snapshot();

        bool CompareCache(string stock, int size);

        bool CompareCache(string stock);
    }
}
