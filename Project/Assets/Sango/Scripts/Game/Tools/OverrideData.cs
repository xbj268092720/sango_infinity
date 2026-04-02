namespace Sango.Core.Tools
{
    public class OverrideData<T>
    {
        public T Value { get; set; }
        public OverrideData(T baseValue) { Value = baseValue; }
        public OverrideData<T> Set(T baseValue) { Value = baseValue; return this; }
    }

}
