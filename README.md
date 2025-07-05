# Dynamic Mapping System

A flexible and extensible data mapping system for converting between internal C# data models and external data formats. It dynamically applies validators and mappers by given types as fully qualified strings, thereby being type safe and validates the input and output models based on given rules.

## Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Key Features](#key-features)
- [Quick Start](#quick-start)
- [System Architecture](#system-architecture)
- [Core Components](#core-components)
- [Usage Examples](#usage-examples)
- [Extending the System](#extending-the-system)
- [System Assumptions](#system-assumptions)
- [Limitations](#limitations)
- [Contributing](#contributing)

## Overview

The Dynamic Mapping System enables seamless data transformation between:
- Internal C# data models (e.g., `Models.Reservation`, `Models.Room`)
- External partner data models (e.g., `Google.Reservation`, `BookingDotCom.Booking`)

The system dynamically resolves mappers and validators at runtime using fully qualified type names as strings, providing flexibility while maintaining type safety through:
- **Runtime Type Resolution**: Automatically discovers and instantiates appropriate mappers based on string type identifiers
- **Dual Validation**: Validates both input and output data using FluentValidation rules
- **Type Safety**: Ensures data integrity through strongly-typed mappers and comprehensive validation
- **Extensibility**: Allows easy addition of new partners and data formats without modifying existing code

### Project Requirements

The system is designed to handle both the conversion of our internal models to partner-specific formats and the mapping of incoming partner data to our internal models.

#### 1. Mapping System Core

- Core mapping handler class (`MapHandler`) with a method signature:_mapHandler.Map(object data, string sourceType, string targetType);- This method maps data between various source and target formats based on the provided `sourceType` and `targetType`.

#### 2. Supported Data Formats

The system supports the following mappings:
- From our internal C# data models (e.g., `Models.Reservation`, `Models.Room`, etc.) to partner-specific data models
- From partner-specific data models to our internal C# data models

#### 3. Mapping Scenarios

The system is capable of handling scenarios such as:

- Mapping an internal `Reservation` object to a data model that is expected by Google:_mapHandler.Map(data: {}, sourceType: "Models.Reservation", targetType: "Google.Reservation");
- Mapping a `Reservation` payload from Google to our internal `Reservation` data model:_mapHandler.Map(data: {}, sourceType: "Google.Reservation", targetType: "Models.Reservation");
#### 4. Extensibility

- Designed to be easily extendable, allowing for the addition of new source/target formats and partner-specific mapping rules without significant changes to the existing codebase
- Clear documentation on how to add new mappings and extend the system

#### 5. Error Handling and Validation

- Robust error handling to manage invalid mappings, incompatible formats, etc.
- Validates the input data against the source and target formats, and provides meaningful error messages where necessary

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

## Key Features

- **Dynamic Type Resolution**: Resolves mappers and validators at runtime using string-based type names
- **Type-Safe Mapping**: Strongly-typed mappers with compile-time safety
- **Comprehensive Validation**: FluentValidation-based validation for all data models (input and output)
- **Dependency Injection**: Built on Microsoft.Extensions.DependencyInjection
- **Extensible Design**: Easy addition of new partners and data formats
- **Exception Handling**: Detailed error reporting with custom exception types
- **Bidirectional Mapping**: Support for both directions of data conversion
- **Runtime Safety**: Validates data types and ensures mapping compatibility at runtime

## Quick Start

### 1. Installation

Add the necessary NuGet packages to your project:
<PackageReference Include="FluentValidation" Version="12.0.0" />
<PackageReference Include="Microsoft.Extensions.DependencyInjection" Version="9.0.0" />
### 2. Setup Dependency Injection
var services = new ServiceCollection();

// Register core system
services.AddDynamicMappingSystem();

// Register partner-specific mappers
services.AddDynamicMappingSystemGoogleMappers();
services.AddDynamicMappingSystemBookingDotComMappers();

var serviceProvider = services.BuildServiceProvider();
var mapHandler = serviceProvider.GetRequiredService<IMapHandler>();
### 3. Basic Usage
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
## System Architecture

### Core Components

#### 1. IMapHandler
The main entry point for all mapping operations.
public interface IMapHandler
{
    object Map(object data, string sourceType, string targetType);
}
#### 2. IMapper<TSource, TTarget>
Specific mapper interface for transformation between two particular types.
public interface IMapper<TSource, TTarget>
{
    TTarget Map(TSource source);
}
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
## Core Components

### Dynamic Type Resolution

The system uses reflection to resolve .NET types from string identifiers:
// Automatically resolves "Models.Reservation" to the actual Type
var result = mapHandler.Map(data, "Models.Reservation", "Google.Reservation");
### Exception Types

The system provides specific exception types for different error scenarios:

- `MappingException`: Base exception for mapping errors
- `UnsupportedMappingException`: Thrown when no mapper is found for the requested type combination
- `SourceDataTypeMismatchException`: Thrown when input data type doesn't match expected source type
- `ValidationException`: Thrown when validation fails on input or output data

### Validation System

All models require validation using FluentValidation. The system validates both input and output:
public class Reservation