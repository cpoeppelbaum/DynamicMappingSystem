using DynamicMappingSystem.Core;
using DynamicMappingSystem.BookingDotCom.Mappers;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using BookingDotComModels = BookingDotCom;
using DynamicMappingSystem.BookingDotCom.Validators;

namespace DynamicMappingSystem.BookingDotCom
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all BookingDotCom-specific mappers and validators
        /// </summary>
        public static IServiceCollection AddDynamicMappingSystemBookingDotComMappers(this IServiceCollection services)
        {
            // Register validators
            services.AddSingleton<IValidator<BookingDotComModels.Booking>, BookingValidator>();
            services.AddSingleton<IValidator<BookingDotComModels.Guest>, GuestValidator>();
            services.AddSingleton<IValidator<BookingDotComModels.Room>, RoomValidator>();

            // Register mappers
            services.AddSingleton<IMapper<BookingDotComModels.Booking, Models.Reservation>, FromBookingMapper>();
            services.AddSingleton<IMapper<Models.Reservation, BookingDotComModels.Booking>, ToBookingMapper>();
            services.AddSingleton<IMapper<BookingDotComModels.Room, Models.Room>, FromRoomMapper>();
            
            return services;
        }
    }
}