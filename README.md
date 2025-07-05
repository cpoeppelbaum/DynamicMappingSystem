# Dynamic Mapping System

A flexible and extensible data mapping system for converting between internal C# data models and external data formats.
It dynamically applies validators and mappers by given types as fully qualified strings, thereby being type safe and validates the input and output models based on given rules.

## Table of Contents

- [Overview](#overview)
- [Dependencies](#dependencies)
- [Project Requirements](#project-requirements)
- [Architecture](#architecture)
- [Quick Start](#quick-start)
- [System Architecture](#system-architecture)
- [Extending the System](#extending-the-system)
- [Assumptions](#assumptions)
- [Limitations](#limitations)

## Overview

The Dynamic Mapping System enables seamless data transformation. E.g.:
- Internal data models to a partner (`Models.Reservation` --> `Google.Reservation`)
- External partner to internal data models (`BookingDotCom.Booking` --> `Models.Reservation` )

The system dynamically resolves mappers and validators at runtime using fully qualified type names as strings, providing flexibility while maintaining type safety through:
- **Runtime Type Resolution**: Automatically discovers and instantiates appropriate mappers based on string type identifiers.
- **Dual Validation**: Validates both input and output data using FluentValidation rules.
- **Type Safety**: Ensures data integrity through strongly-typed mappers.
- **Extensibility**: Allows easy addition of new partners and data formats without modifying existing code.

## Dependencies

This project uses the nuget package FluentValidation, Version="12.0.0" (Apache License 2.0)

## Project Requirements

The system is designed to handle both the conversion of internal models to partner-specific formats and the mapping of incoming partner data to internal models.

### 1. Mapping System Core

- Core mapping handler class (`MapHandler`) with the following method signature:
```csharp
_mapHandler.Map(object data, string sourceType, string targetType);
```
This method maps data between various source and target formats based on the provided `sourceType` and `targetType`.

### 2. Supported Data Formats

The system supports the following mappings:
- From the internal C# data models (e.g., `Models.Reservation`, `Models.Room`, etc.) to partner-specific data models.
- From partner-specific data models to the internal C# data models.

### 3. Mapping Scenarios

The system is capable of handling scenarios such as:

- Mapping an internal `Reservation` object to a data model that is expected by Google:
 ```csharp
_mapHandler.Map(data: {}, sourceType: "Models.Reservation", targetType: "Google.Reservation");
```

- Mapping a `Reservation` payload from Google to the internal `Reservation` data model:
```csharp
_mapHandler.Map(data: {}, sourceType: "Google.Reservation", targetType: "Models.Reservation");
```

### 4. Extensibility

- Designed to be easily extendable, allowing for the addition of new source/target formats and partner-specific mapping rules without significant changes to the existing codebase.

### 5. Error Handling and Validation

- Robust error handling to manage invalid mappings, incompatible formats, etc.
- Validates the input data against the source and target formats, and provides meaningful error messages where necessary.

## Architecture

The system follows a modular architecture with clear separation of concerns:
DynamicMappingSystem/  
├── DynamicMappingSystem.Core/          # Core interfaces and exceptions  
├── DynamicMappingSystem/               # Main mapping handler and internal validators  
├── Mappers/  
│   ├── DynamicMappingSystem.Google/    # Google-specific mappers and validators  
│   └── DynamicMappingSystem.BookingDotCom/ # BookingDotCom-specific mappers and validators  
├── ModelSamples/  
│   ├── Internal/                       # Internal data models  
│   └── External/                       # External partner data models  
└── DynamicMappingSystemTest/          # Unit tests  

## Quick Start

### 1. Installation

This project was created with Dependency Injection Services in mind, therefore is dependent on an IServiceCollection implementation.
If your project doesn't use any yet, you can add the necessary NuGet package to your project:

<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.0" />
### 2. Setup Dependency Injection

```csharp
var services = new ServiceCollection();

// Register core system
services.AddDynamicMappingSystem();

// Register partner-specific mappers
services.AddDynamicMappingSystemGoogleMappers();
services.AddDynamicMappingSystemBookingDotComMappers();

var serviceProvider = services.BuildServiceProvider();
var mapHandler = serviceProvider.GetRequiredService<IMapHandler>();
```

### 3. Basic Usage

```csharp
// Map from internal model to Google format
var internalReservation = new Models.Reservation { /* ... */ };
var googleReservation = mapHandler.Map(
    internalReservation, 
    "Models.Reservation", 
    "Google.Reservation"
);

// Map from BookingDotCom to internal format
var bookingDotComData = new BookingDotCom.Booking { /* ... */ };
var internalData = mapHandler.Map(
    bookingDotComData, 
    "BookingDotCom.Booking", 
    "Models.Reservation"
);
```

## System Architecture

### Core Components

#### 1. IMapHandler

The main entry point for all mapping operations.
```csharp
public interface IMapHandler
{
    object Map(object data, string sourceType, string targetType);
}
```

#### 2. IMapper<TSource, TTarget>

Specific mapper interface for transformation between two particular types.
```csharp
public interface IMapper<TSource, TTarget>
{
    TTarget Map(TSource source);
}
```

#### 3. MapHandler

Main implementation that:
- Dynamically resolves types from string identifiers using reflection
- Validates input data using FluentValidation
- Resolves appropriate mapper from DI container
- Performs the mapping
- Validates output data to ensure integrity
- Handles exceptions and error reporting

### Data Flow

Input Data → Type Resolution → Input Validation → Mapper Selection → 
Data Transformation → Output Validation → Result
  
## Extending the System
  
### Adding New Partner Integrations

To add support for a new partner (e.g., "Airbnb"), follow these steps to create a complete integration with mappers and validators:

#### Step 1: Create a New Class Library Project

Create a new class library project following the naming convention:

```
DynamicMappingSystem.{PartnerName}/
├── DynamicMappingSystem.{PartnerName}.csproj
├── ServiceCollectionExtensions.cs
├── Mappers/
│   ├── From{PartnerName}ReservationMapper.cs
│   ├── To{PartnerName}ReservationMapper.cs
│   ├── From{PartnerName}RoomMapper.cs
│   └── To{PartnerName}RoomMapper.cs
└── Validators/
    ├── ReservationValidator.cs
    ├── RoomValidator.cs
    └── GuestValidator.cs (if applicable)
```

#### Step 2: Create Mappers
  
Implement mappers using the `IMapper<TSource, TTarget>` interface:
 
**Example: From Partner to Internal Model**

```csharp
using DynamicMappingSystem.Core;
using DynamicMappingSystem.Core.Exceptions;
using Models;
using AirbnbModels = Airbnb;

namespace DynamicMappingSystem.Airbnb.Mappers
{
    internal class FromAirbnbReservationMapper : IMapper<AirbnbModels.Booking, Reservation>
    {
        public Reservation Convert(AirbnbModels.Booking source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            try
            {
                return new Reservation
                {
                    Id = source.BookingReference,
                    CheckInDate = source.CheckIn,
                    CheckOutDate = source.CheckOut,
                    GuestName = $"{source.GuestFirstName} {source.GuestLastName}".Trim(),
                    NumberOfGuests = source.GuestCount,
                    RoomId = source.PropertyId,
                    TotalAmount = source.TotalPrice
                };
            }
            catch (Exception ex) when (!(ex is MappingException))
            {
                throw new MappingException($"Error converting from Airbnb.Booking to Models.Reservation", ex);
            }
        }
    }
}
```

#### Step 3: Create Validators

Implement FluentValidation validators for the partner models:
  
```csharp
using FluentValidation;
using Airbnb;

namespace DynamicMappingSystem.Airbnb.Validators
{
    public class BookingValidator : AbstractValidator<Booking>
    {
        public BookingValidator()
        {
            RuleFor(x => x.BookingReference)
                .NotEmpty()
                .WithMessage("Booking reference is required."); 
            RuleFor(x => x.CheckIn)
                .NotEmpty()
                .WithMessage("Check-in date is required.")
                .LessThan(x => x.CheckOut)
                .WithMessage("Check-in date must be before check-out date.");
            RuleFor(x => x.CheckOut)
                .NotEmpty()
                .WithMessage("Check-out date is required.");
            RuleFor(x => x.GuestFirstName)
                .NotEmpty()
                .WithMessage("Guest first name is required.")
                .MaximumLength(100)
                .WithMessage("Guest first name must not exceed 100 characters.");
            RuleFor(x => x.GuestLastName)
                .NotEmpty()
                .WithMessage("Guest last name is required.")
                .MaximumLength(100)
                .WithMessage("Guest last name must not exceed 100 characters.");
            RuleFor(x => x.GuestCount)
                .GreaterThan(0)
                .WithMessage("Guest count must be at least 1.");
            RuleFor(x => x.TotalPrice)
                .GreaterThan(0)
                .WithMessage("Total price must be greater than 0.");
        }
    }
}
```

#### Step 4: Create Service Collection Extensions

Create an extension method to register all mappers and validators:

```csharp
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using DynamicMappingSystem.Core;
using DynamicMappingSystem.Airbnb.Mappers;
using DynamicMappingSystem.Airbnb.Validators;
using AirbnbModels = Airbnb;
using Models;

namespace DynamicMappingSystem.Airbnb
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers all Airbnb-specific mappers and validators
        /// </summary>
        public static IServiceCollection AddAirbnbMappers(this IServiceCollection services)
        {
            // Register mappers
            services.AddSingleton<IMapper<AirbnbModels.Booking, Reservation>, FromAirbnbReservationMapper>();
            services.AddSingleton<IMapper<Reservation, AirbnbModels.Booking>, ToAirbnbReservationMapper>();
            services.AddSingleton<IMapper<AirbnbModels.Property, Room>, FromAirbnbRoomMapper>();
            services.AddSingleton<IMapper<Room, AirbnbModels.Property>, ToAirbnbRoomMapper>();

            // Register validators
            services.AddSingleton<IValidator<AirbnbModels.Booking>, BookingValidator>();
            services.AddSingleton<IValidator<AirbnbModels.Property>, PropertyValidator>();
            
            return services;
        }
    }
}
```

#### Step 5: Register the new Integration

In your application startup, register the new mappers:

```csharp
var services = new ServiceCollection();
services.AddDynamicMappingSystem();

services.AddGoogleMappers();
services.AddBookingDotComMappers();
services.AddAirbnbMappers(); // Register new Airbnb integration
```

## Assumptions

- I am aware of mapping libraries like AutoMapper, but this project is supposed to show how this can be done manually.
- The solution assumes an **`IServiceProvider`-based dependency injection container** is available. All services are resolved using this mechanism.
- The **public API method signature is fixed to return `object`**, as per the context set by the requirements.
    - Although returning a typed result (e.g. `Result<T>` with error messages) would be preferable, this is not allowed by the interface.
    - Returning `null` on error was considered but rejected, as the system expects meaningful error messages.
    - Therefore, **exceptions are thrown** (with meaningful error messages) on mapping errors. This choice has performance trade-offs and is less graceful for processing large or complex data.
- **In the requirements context, most models would require validation.**  
	- Therefore, if no validator is found, an **exception is thrown**.  
	- A validator must be registered for each mappable type.  
	- If validation is not needed, an empty implementation of a **validator** should be explicitly provided.

## Limitations

- **Logging is not implemented inside the mapping layer.**  
    - Logging must be handled by the **calling class or higher-level application logic**.
- **Performance overhead** due to exception handling in case of errors instead of using result types or return codes.
- **Tight coupling to `IServiceProvider`** may reduce portability to environments with alternative DI containers or service resolution mechanisms.
- **Type safety is compromised** by the use of `object` return types. Consumers must explicitly cast the result and handle invalid types at runtime.
