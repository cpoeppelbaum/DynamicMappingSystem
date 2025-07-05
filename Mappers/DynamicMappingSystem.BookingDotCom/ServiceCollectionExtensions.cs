using DynamicMappingSystem.Core;
using DynamicMappingSystem.BookingDotCom.Mappers;
using Microsoft.Extensions.DependencyInjection;
using BookingDotComModels = BookingDotCom;

namespace DynamicMappingSystem.BookingDotCom
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all BookingDotCom-specific mappers
        /// </summary>
        public static IServiceCollection AddBookingDotComMappers(this IServiceCollection services)
        {
            services.AddSingleton<IMapper<BookingDotComModels.Booking, Models.Reservation>, FromBookingMapper>();
            services.AddSingleton<IMapper<Models.Reservation, BookingDotComModels.Booking>, ToBookingMapper>();
            services.AddSingleton<IMapper<BookingDotComModels.Room, Models.Room>, FromRoomMapper>();
            
            return services;
        }
    }
}