namespace Bluefragments.Utilities.Data.Cosmos
{
    public interface ICosmosEntityBase<T>
    {
        T Id { get; set; }

        string Type { get; set; }
    }
}
