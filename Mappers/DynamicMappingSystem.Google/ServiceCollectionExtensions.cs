using DynamicMappingSystem.Core;
using DynamicMappingSystem.Google.Mappers;
using DynamicMappingSystem.Google.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using GoogleModels = Google;

namespace DynamicMappingSystem.Google
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all Google-specific mappers and validators
        /// </summary>
        public static IServiceCollection AddDynamicMappingSystemGoogleMappers(this IServiceCollection services)
        {
            // Register validators
            services.AddSingleton<IValidator<GoogleModels.Reservation>, ReservationValidator>();
            services.AddSingleton<IValidator<GoogleModels.Room>, RoomValidator>();

            // Register mappers
            services.AddSingleton<IMapper<Models.Reservation, GoogleModels.Reservation>, ToReservationMapper>();
            services.AddSingleton<IMapper<GoogleModels.Reservation, Models.Reservation>, FromReservationMapper>();
            services.AddSingleton<IMapper<Models.Room, GoogleModels.Room>, ToRoomMapper>();
            services.AddSingleton<IMapper<GoogleModels.Room, Models.Room>, FromRoomMapper>();
            
            return services;
        }
    }
}