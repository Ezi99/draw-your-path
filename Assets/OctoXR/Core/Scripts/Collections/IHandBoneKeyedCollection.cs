namespace OctoXR.Collections
{
    public interface IHandBoneKeyedCollection<T>
    {
        T this[HandBoneId boneId] { get; set; }
    }
}
