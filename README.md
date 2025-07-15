# Dynamic Mapping System

A flexible and extensible data mapping system for converting between internal C# data models and external data formats.
It dynamically applies validators and mappers by given types as fully qualified strings, thereby being type safe and validates the input and output models based on given rules.

## Table of Contents

- [Overview](#overview)
- [History](#history)
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
- **Runtime Type Resolution**: Automatically instantiates appropriate mappers based on string type identifiers.
- **Dual Validation**: Validates both input and output data using FluentValidation rules.
- **Type Safety**: Ensures data integrity through strongly-typed mappers.
- **Extensibility**: Allows easy addition of new partners and data formats without modifying existing code.

## History

The project was done in TDD as a showcase and shows a development from a quick prototype to a more sophisticated approach.
Disclaimer: I relied heavily on Copilot for generating the test cases and admitably didn't take a lot of time to review them.

### V1

The first version made use of IServiceCollection to register the mappers, to serve asp.net habits and also used reflection to resolve the types of the given strings from the very generic method signature which was a requirement.

### V2

The second version uses custom registration methods with generic parameters. This registration of the converters also stores the source and target types in a dictionary by basically using the "toString" of the generic parameters as keys and the types as values.
Therefore no reflection is used anymore. The use of IServiceCollection could be easily reintroduced by providing an extension method which does the "AddSingleton" while also calling the custom Registration method.

## Dependencies

This project uses the nuget package FluentValidation, Version="12.0.0" (Apache License 2.0)

## Project Requirements

The system`s functionality is to handle both the conversion of internal models to 3rd party formats and vise versa. Easily extendable and with a specific interface for the given context.

### 1. Mapping System Core

- Core mapping handler class (`MapHandler`) with the following method signature:

```csharp
_mapHandler.Map(object data, string sourceType, string targetType);
_mapHandler.Map(data: {}, sourceType: "Models.Reservation", targetType: "Google.Reservation"); // e.g. map an internal `Reservation` object to a data model that is expected by Google
_mapHandler.Map(data: {}, sourceType: "Google.Reservation", targetType: "Models.Reservation"); // e.g. map a `Reservation` payload from Google to the internal `Reservation` data
```

This method maps data between various source and target formats based on the provided `sourceType` and `targetType`.

### 2. Supported Data Formats

The system supports the following mappings:
- From the internal C# data models (e.g., `Models.Reservation`, `Models.Room`, etc.) to
	- **imagined** formats of a google vacations booking system
	- imagined formats of the booking.com vacations booking system
	- back to the project specific *internal* models

This project is not related to, or uses any package from any of the aforementioned companies.

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

Add the necessary NuGet package to your project:
- FluentValidation Version="12.0.0"

### 2. Setup and Registration

```csharp
// Create the mapping handler
var mapHandler = MapHandler.Create();

// Register mappers and validators directly
mapHandler.RegisterMapper<Models.Reservation, Google.Reservation>(new ToGoogleReservationMapper());
mapHandler.RegisterMapper<Google.Reservation, Models.Reservation>(new FromGoogleReservationMapper());
mapHandler.RegisterValidator<Models.Reservation>(new ReservationValidator());
mapHandler.RegisterValidator<Google.Reservation>(new GoogleReservationValidator());
// ...register other mappers/validators as needed
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
- Resolves appropriate mapper and validator from internal registries
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
DynamicMappingSystem.{PartnerName}/
├── DynamicMappingSystem.{PartnerName}.csproj
├── Mappers/
│   ├── From{PartnerName}ReservationMapper.cs
│   ├── To{PartnerName}ReservationMapper.cs
│   ├── From{PartnerName}RoomMapper.cs
│   └── To{PartnerName}RoomMapper.cs
└── Validators/
    ├── ReservationValidator.cs
    └── RoomValidator.cs

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

It is good practice to make this class internal rather than public, and provide a common method to register all Validators and Mappers at once (as shown in Step 4), hence the client consuming these mappers and validators won't accidentally forget to register any.

#### Step 3: Create Validators

Implement FluentValidation validators for the partner models:

```csharp
using FluentValidation;
using Airbnb;

namespace DynamicMappingSystem.Airbnb.Validators
{
    internal class BookingValidator : AbstractValidator<Booking>
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

It is good practice to make this class internal rather than public, and provide a common method to register all Validators and Mappers at once (as shown in Step 4), hence the client consuming these mappers and validators won't accidentally forget to register any.

#### Step 4: Create public Extension method 

```csharp
    public static class MapHandlerAirbnbExtension
    {
        public static IMapHandler AddAirbnbMappers(this IMapHandler mapHandler)
        {
            // Your new validators will be registered here
            mapHandler.RegisterValidator<Airbnb.Booking>(new BookingValidator());

            // Your new mappers will be registered here
            mapHandler.RegisterMapper<Airbnb.Booking, Models.Reservation>(new FromBookingMapper());
            
            return mapHandler;
        }
    }
```

#### Step 5: Register Mappers and Validators

Register your mappers and validators via the extension method on the `MapHandler` instance:

```csharp
var mapHandler = MapHandler.Create();
mapHandler.AddAirbnbMappers();
```


## Assumptions

- I am aware of mapping libraries like AutoMapper, but this project is supposed to show how this can be done manually.
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
- Type mismatched e.g. when the provided object is not of given source type, or missing mappers will be detected on runtime only. As far as I know there is no solution that could handle that while compile time. Let me know if there is.

