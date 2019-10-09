namespace Gemstar.BSPMS.Common.Tools
{
    public class KeyValuePairModel<TKey, TValue>
    {
        public KeyValuePairModel(){}

        public KeyValuePairModel(TKey key, TValue value)
        {
            Key = key;
            Value = value;
        }

        public TKey Key { get; set; }

        public TValue Value { get; set; }

        public object Data { get; set; }
    }
}
