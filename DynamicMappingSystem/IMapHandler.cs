namespace DynamicMappingSystem
{
    public interface IMapHandler
    {
        object Map(object data, string sourceType, string targetType);
    }
}