namespace OctoXR.Collections
{
    public interface IHandBoneKeyedReadOnlyCollection<T>
    {
        T this[HandBoneId boneId] { get; }
    }
}
