using DynamicMappingSystem.Core;

namespace DynamicMappingSystemTest
{
    public class MapHandlerStub : IMapHandler
    {
        private Dictionary<Tuple<Type, Type>, IMapper> Mappers = new Dictionary<Tuple<Type, Type>, IMapper>();
        private List<Tuple<Type, Type>>? FoundMappers;

        public object Map(object data, string sourceType, string targetType)
        {
            // should not be called in this test.
            throw new NotImplementedException();
        }

        public IMapHandler RegisterMapper<TSource, TTarget>(IMapper<TSource, TTarget> converter)
        {
            Mappers.Add(Tuple.Create(typeof(TSource), typeof(TTarget)), converter);
            FoundMappers = Mappers.Select(kvp => kvp.Key).ToList();
            return this;
        }

        public IMapHandler RegisterValidator<T>(AbstractDMSValidator<T> validator)
        {
            // do nothing. not tested here.
            return this;
        }

        public IMapper<TSource, TTarget> GetMapper<TSource, TTarget>()
        {
            Assert.True(FoundMappers != null, "No Mappers have been found.");
            var key = Tuple.Create(typeof(TSource), typeof(TTarget));
            if (Mappers.TryGetValue(key, out var mapper))
            {
                FoundMappers.Remove(key);
                return (IMapper<TSource, TTarget>)mapper;
            }
            throw new KeyNotFoundException($"No mapper registered for {typeof(TSource).Name} to {typeof(TTarget).Name}");
        }

        public bool ValidateAllMappersHaveBeenAskedFor()
        {
            Assert.True(FoundMappers != null, "No Mappers have been found.");
            Assert.True(Mappers.Count > 0, "No mappers have been registered.");
            // Check if all mappers have been requested at least once.
            return !FoundMappers.Any(); // FoundMappers must be an empty list at this point
        }
    }
}
