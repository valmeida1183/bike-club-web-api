<task>I want to create a plan to refactor this entire project into Vertical slice architeture.</task>

<role>As a software Architect you are responsible to create a plan to refactor this entire application.</role>

<requirements>
  ### Business

- Refactor the architeture of Data-Drive Api to more robust Vertical slice architeture.
- Mantain the actual business logic without break it.
- The main focus is to create a plan, not the implementation.
- The api routes should be preserved.
- Use minimal Api approach.

### Technical

- A Features folder should be created at root of project, check <features>.
- A Configurations folder should be created at root of project, check <configuration>.
- A SharedKernel Folder should be created at root of project.
- A Domain folder should be created at root of project.
-
- Payload validation should use FluentValidation library.

- Endpoints should use minimal Api approach.
- Models folder should be renamed to Entities.
- Controller folder should be removed after the migration.
  </requirements>

  <features>
    ### This section describes how Features folder structure should be.
  - Inside feature folder each controller should have a respective feature folder ex: AddressController --> Address (folder)
  - Inside each feature folder we should have one folder for each controller endpoint operation. ex: GetAll, GetById, Create, Update, Delete.
  - Each operation folder should have separate file for each responsability.
  - If Some logic is repeated in multiple operations it should be centralized into a service class and place at Shared folder inside the feature folder.
  - If a Validator is repeaded in multiple operations it should be centralized and place at Shared folder inside the feature folder.

  #### Files inside operation folder:
  - A Record called Request that represents the body payload (if is needed).
  - A Record called Response that represents the object data that Api will return.
  - A Endpoint class that inherts from IEndpoint interface and is a minimal Api endpoint. Check <shared-kernel>.
  - A Handler class that contains the logic orquestration.
  - A Validator class that use FluentValidator to validate the Request object (if needed).

  - Handler class can receive by Dependency injection the Validator, DataContext, request.

  </features>

<configurations>
  ### This section describes how Configurations folder structure should be.

- Migrate the configuration methods in the ./Program.cs to a specific extension method inside a separate class.
- Below is the methos in the Program class that should be migrated:

* ConfigureAuthentication
* ConfigureCompression
* ConfigureCORS
* ConfigureDataContext
* ConfigureSwagger
* LoadSettings

- We will need one more configuration extension class for our endpoints mapping. So create a AddEnpoints class to extend IServiceCollection.
  The objective is map all endpoints in the project by getting the assmbly, use reflection or use context7 MCP and chekc if Scrutor library os a good choice for this case.

- Register all configurations that are in extension methos in the Program.cs. Pay atenttion to the correct order if needed.

</configurations>

  <shared-kernel>
    ### This section describes how SharedKernel folder structure should be.

- A IEndpoint interface that have a void method signature below:
  --void MapEndpoint(IEndpointRouteBuilder app)

- Settings class should be moved from root to here.
- Static folder should be moved from root to here.
- If is possible move Resources folder from root to here. Check if is possible, because this folder provides static files, like images, mayber .Net needs it on the root of project.
  <shared-kernel>

<domain>
</domain>
