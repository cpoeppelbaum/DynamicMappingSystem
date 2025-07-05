using DynamicMappingSystem.Core;
using DynamicMappingSystem.Google.Mappers;
using Microsoft.Extensions.DependencyInjection;
using GoogleModels = Google;

namespace DynamicMappingSystem.Google
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all Google-specific mappers
        /// </summary>
        public static IServiceCollection AddGoogleMappers(this IServiceCollection services)
        {
            services.AddSingleton<IMapper<Models.Reservation, GoogleModels.Reservation>, ToReservationMapper>();
            services.AddSingleton<IMapper<GoogleModels.Reservation, Models.Reservation>, FromReservationMapper>();
            services.AddSingleton<IMapper<Models.Room, GoogleModels.Room>, ToRoomMapper>();
            services.AddSingleton<IMapper<GoogleModels.Room, Models.Room>, FromRoomMapper>();
            
            return services;
        }
    }
}