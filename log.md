## Assumptions
- **Third-Party Libraries**: Based on the Readme, Services.AdministratorOne.Abstractions and Services.AdministratorTwo.Abstractions are third-party libraries, so these cannot be modified.
- **Single Product Application**: Each Administrator is assumed to be specific to a Product, with checks in place to ensure the correct Product per Administrator. I just have to keep in mind the Readme and make the code ready for future Products and Administrators additions or even changes.
- **Configuration Stability**: There is a need to set payment thresholds and age restrictions as per Readme, so the plan is to set them within the code, but are subject to future changes which might be moved to a configurable environment.
- **KYC Service**: As per Readme, the process can only occur if KYC service returns `IsSuccess=true`, then there is a need to handle that with precision.
- **Testing Strategy**: Focused on unit testing critical logic paths to ensure reliability as per Readme requirements. Planning to also add integration tests if there is time.

## Decisions
- **Code Separation and Flexibility**: Implemented separate handlers for each Administrator (`AdministratorOneHandler` and `AdministratorTwoHandler`) to maintain clear separation and facilitate potential future changes in handling different products.
- **Dynamic Handler Selection**: Implemented an `AdministrationServiceHandlerFactory` to manage the dynamic selection of administration service handlers based on the product code, enhancing flexibility and reducing hardcoded dependencies.
- **App Settings**: Chose not to externalize configuration of payment thresholds and age limits due to time constraints, though this is planned for future implementation to enhance flexibility, so they will be kept in appsettings.json. Also added models under `Services.Applications.Model` to map it properly during the Dependency Injection.
- **Testing Framework**: Added unit testing for the critical logic paths, using both Products (`ProductOne` and `ProductTwo`) and also added an integration test.
- **Error Handling and Logging**: Strategically decided against implementing advanced error handling and logging mechanisms to stay within the project timeline and also follow the Readme recomendation, acknowledging that these are areas for future improvement.
- **Use of AutoMapper**: Used AutoMapper for efficient mapping between domain models `Application` and data transfer objects `CreateInvestorRequest`. Also added the respective mapping profiles under `Services.Applications.MappingProfiles`.
- **Dependency Injection**: Utilized to manage dependencies dynamically, enhancing the flexibility and testability of the application.
- **Domain Events**: Added more domain events for `KycCompleted`, `EligibilityCheckCompleted`, `EligibilityCheckFailed`, `InvestorCreationFailed`, `AccountCreationFailed` and `ApplicationFailed`.
- **Helpers**: Added a few helpers to calculate the age and also generate mocked data for the testing process. `AgeHelper` is under `Services.Applications.Helpers` and `TestHelper` is under `Services.Applications.Tests.Helpers`.

## Observations
- **Payment Handling**: Noted discrepancies in how payments are processed (decimal vs. integer) and the need for potential rounding or validation adjustments.
- **Domain Event Management**: Highlighted the need to ensure all steps of the application process are captured by downstream systems through Domain Events, adhering to requirements.
- **Testing-first approach**: Prioritized creating a testing framework rather than developing console applications to demonstrate the application working.

## Todo
- **Implementation Completion**: Need to fully implement the processing methods for both ProductOne and ProductTwo using respective adapters (Connect with external services or something similar, for example).
- **Dynamic Configuration**: Plan to externalize configuration settings related to eligibility criteria to allow easier adjustments and management outside of code deployments.
- **Enhanced Testing**: Improve the unit tests and end-to-end tests to ensure all components work seamlessly together.
- **Dynamic Configuration Management**: Move all eligibility criteria and payment thresholds to an external configuration system like a database or dedicated configuration service.
- **Robust Error Handling and Logging**: Develop a detailed logging and error handling to provide better diagnostics and system reliability.