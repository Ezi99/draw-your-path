namespace OctoXR.Collections
{
    public interface IHandFingerKeyedReadOnlyCollection<T>
    {
        T this[HandFinger finger] { get; }
    }
}
