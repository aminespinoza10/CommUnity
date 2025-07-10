# AppVecinos.API Tests

This project contains unit tests for the AppVecinos.API project, specifically focusing on the "neighbors method" as requested in issue #84.

## Test Coverage

### Unit Tests - NeighborService.GetNeighborsAsync()

The `NeighborServiceTests` class contains comprehensive unit tests for the `GetNeighborsAsync()` method in the `NeighborService` class:

1. **GetNeighborsAsync_ReturnsAllNeighbors**: Tests that the method returns all neighbors when they exist in the repository
2. **GetNeighborsAsync_ReturnsEmptyList_WhenNoNeighborsExist**: Tests that the method returns an empty list when no neighbors exist
3. **GetNeighborsAsync_CallsRepositoryOnce**: Tests that the method correctly calls the repository exactly once

### Endpoint Configuration Tests

The `NeighborsEndpointTests` class verifies the endpoint configuration:

1. **GetNeighborsEndpoint_RequiresAdminPolicyAuthorization**: Confirms that the GET /neighbors endpoint is configured with AdminPolicy authorization
2. **NeighborsEndpoint_ExistsInProgramConfiguration**: Verifies that the endpoint is properly mapped in the application configuration

## Authentication Requirements

Based on the analysis of the codebase, the GET /neighbors endpoint has the following authentication requirements:

- **Authorization Required**: Yes
- **Policy**: AdminPolicy 
- **Required Role**: Admin
- **Endpoint**: `GET /neighbors`
- **Configuration**: Defined in `Program.cs` with `.RequireAuthorization("AdminPolicy")`

## Technologies Used

- **xUnit**: Testing framework
- **Moq**: Mocking framework for unit tests
- **Microsoft.AspNetCore.Mvc.Testing**: For endpoint testing
- **Microsoft.EntityFrameworkCore.InMemory**: For in-memory database testing

## Running the Tests

```bash
# Run all tests
dotnet test

# Run only unit tests
dotnet test --filter "FullyQualifiedName~NeighborServiceTests"

# Run only endpoint tests  
dotnet test --filter "FullyQualifiedName~NeighborsEndpointTests"
```

## Test Results

All tests pass successfully, confirming that:
- The NeighborService.GetNeighborsAsync() method works correctly
- The GET /neighbors endpoint is properly configured with AdminPolicy authorization
- The authentication requirements are correctly implemented