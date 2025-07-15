using DynamicMappingSystem.Core;

namespace DynamicMappingSystemTest
{
    public class MapHandlerStub : IMapHandler
    {
        private Dictionary<Tuple<Type, Type>, IMapper> Mappers = new Dictionary<Tuple<Type, Type>, IMapper>();
        private Dictionary<Type, IDMSValidator> Validators = new Dictionary<Type, IDMSValidator>();
        private List<Tuple<Type, Type>> FoundMappers = new List<Tuple<Type, Type>>();
        private List<Type> FoundValidators = new List<Type>();

        public object Map(object data, string sourceType, string targetType)
        {
            // should not be called in this test.
            throw new NotImplementedException();
        }

        public IMapHandler RegisterMapper<TSource, TTarget>(IMapper<TSource, TTarget> converter)
        {
            var key = Tuple.Create(typeof(TSource), typeof(TTarget));
            Mappers.Add(key, converter);
            FoundMappers.Add(key);
            return this;
        }

        public IMapHandler RegisterValidator<T>(AbstractDMSValidator<T> validator)
        {
            var key = typeof(T);
            Validators.Add(key, validator);
            FoundValidators.Add(key);
            return this;
        }

        public IMapper<TSource, TTarget> GetMapper<TSource, TTarget>()
        {
            Assert.True(FoundMappers.Any(), "No Mappers have been found.");
            var key = Tuple.Create(typeof(TSource), typeof(TTarget));
            if (Mappers.TryGetValue(key, out var mapper))
            {
                FoundMappers.Remove(key);
                return (IMapper<TSource, TTarget>)mapper;
            }
            throw new KeyNotFoundException($"No mapper registered for {typeof(TSource).Name} to {typeof(TTarget).Name}");
        }

        public AbstractDMSValidator<T> GetValidator<T>()
        {
            Assert.True(FoundValidators.Any(), "No Validators have been found.");
            var key = typeof(T);
            if (Validators.TryGetValue(key, out var validator))
            {
                FoundValidators.Remove(key);
                return (AbstractDMSValidator<T>)validator;
            }
            throw new KeyNotFoundException($"No validator registered for {typeof(T).Name}");
        }

        public void ValidateAllMappersHaveBeenAskedFor()
        {
            Assert.True(!FoundMappers.Any(), "Not all mappers have been requested.");
        }

        public void ValidateAllValidatorsHaveBeenAskedFor()
        {
            Assert.True(!FoundValidators.Any(), "Not all validators have been requested.");
        }
    }
}
