# Development Workflow Standards (Backend)

## Build Verification
1. **Mandatory Build**: Every change to the C# code MUST be verified by running `dotnet build` from the solution root or the specific project directory.
2. **Clean Architecture**: Ensure changes comply with the established Layered Architecture (Domain -> Application -> Infrastructure -> WebAPI).
3. **Entity Rules**: All new entities MUST inherit from `BaseEntity` and implement `IAggregateRoot` (refer to Domain Entity Patterns skill).

## API Alignment
1. **Frontend Compatibility**: Before changing API response structures, verify how the changes will impact the Frontend (FE).
2. **Naming Convention**: Use PascalCase for Role identifiers in responses to match FE requirements (`SystemAdmin`, `Ophthalmologist`, `ClinicStaff`, `Patient`).
3. **Documentation**: Keep Swagger/OpenAPI documentation up to date to facilitate FE integration.
