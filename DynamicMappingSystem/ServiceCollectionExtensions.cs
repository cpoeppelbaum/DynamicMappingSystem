using DynamicMappingSystem.Core;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Models;

namespace DynamicMappingSystem
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers internal model validators
        /// </summary>
        public static IServiceCollection AddDynamicMappingSystem(this IServiceCollection services)
        {
            // Add internal mappers
            services.AddSingleton<IValidator<Reservation>, Validators.ReservationValidator>();
            services.AddSingleton<IValidator<Room>, Validators.RoomValidator>();

            // Add mapper system
            services.AddSingleton<IMapHandler, MapHandler>();

            return services;
        }
    }
}