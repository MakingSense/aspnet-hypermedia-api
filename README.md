# MakingSense AspNet HypermediaApi Helpers

This library pretends to expose some models and utilities useful to create an Hypermedia API using ASP.NET 5 Framework.

The idea is not to create a framework, only a set of utilities that can be used together or not.

## Basic Configuration

Example:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc(options => 
        {
            options.OutputFormatters.Clear();
            options.OutputFormatters.Add(new HypermediaApiJsonOutputFormatter());

            options.InputFormatters.Clear();
            options.InputFormatters.Add(new HypermediaApiJsonInputFormatter());

            options.Filters.Add(new PayloadValidationFilter());
            options.Filters.Add(new RequiredPayloadFilter());

            options.ModelBinders.Add(new CustomRepresentationModelBinder());
        });

        services.AddApiMappers();

        services.AddSuitableValidators();

        services.AddLogging();

        services.AddLinkHelper<CustomLinkHelper>();
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerfactory)
    {
        app.UseApiErrorHandler();

        app.UseMvc();

        app.UseNotFoundHandler();

        loggerfactory.AddProvider(new CustomLoggerProvider());
    }
}
```

## Hypermedia API library architecture

### Abstractions

* Relations

    A Relation represents a behavior in the system, see [Hypermedia and H Factors](https://github.com/publicmediaplatform/pmpdocs/wiki/Hypermedia-and-H-Factors). Each link has a `rel` property with one or more _relations_ separated by spaces.

    A relation defines how a link works. There are [standard relations](http://www.iana.org/assignments/link-relations/link-relations.xhtml), for example `self`, `next`, etc. And custom relations defined by implementer and are expressed as URLs to HTML documents with the behavior details. In general, custom relations are _Action Relations_.

* Action Relations

    Controller actions are decorated with `ActionRelation` attributes that indicate: HTTP method, Input model, Output model, a description and a link to related HTML document.

    These attributes also allow to define the MVC route, and some other metadata:

    * `NotImplemented` - The action is not implemented yet.
    * `IsExperimental` - The behavior is not confirmed and could change in the near future.
    * `Description` - Default description of links to this action.
    * `SuitableValidators` - See related section in under _MVC Filters_.

    example: 

    ```csharp
    public class SubscribersController : Controller
    {
        // . . .
        [ImportSubscribersRelation(
            Template = "/accounts/{accountName}/lists/{listId}/subscribers/import",
            SuitableValidators = new[] { typeof(ImportSubscribersSuitableValidator) })]
        public async Task<CreationResult> ImportSubscribersToList(string accountName, int listId, [FromBody] [NotNull] ImportSubscribersPayload subscribers)
        {
            // . . .
    ```

* Models

    All API input and output types should be represented as _Model_ classes. 

    The convention for these classes is to have camel case properties and an special property called `_links` where controllers will load related links based on the state of the application.

    See _API Mappers_ section for information about a provided utility to map domain objects to API Models.

* Problems

    Problems are an special kind of models used to represent errors. _ApiErrorHandler Middleware_ is in charge of render them. 

    Example:

    ```csharp
    if (Exists(list.name))
    {
        throw new ApiException(new DuplicatedListNameProblem(list.name));
    }
    ```

### Linking

Making Sense Hypermedia API provides a `LinkHelper` that allows to use strongly typed code to generate links to actions, including metadata information and processing _Suitable Validators_ (See related section in under _MVC Filters_).

Example:

```csharp
public class SubscribersController : Controller
{
    // . . .
    [GetSubscriberCollectionRelation(Template = "/accounts/{accountName}/lists/{listId}/subscribers", Description = "Get subscribers of a list")]
    public async Task<SubscriberCollectionPage> GetSubscribersOfList(string accountName, int listId, int? page = null, int? per_page = null)
    {
        var pagination = new PaginationParameters(page, per_page);
        SubscriberCollectionPage result = await _service.GetSubscribersOfList(accountName, userId, listId);

        return result

            // Adds links to output model (a page of subscribers)
            .AddLinks(
                _link.ToHomeAccount(),
                _link.ToAction<ListsController>(x => x.GetList(accountName, listId)),
                _link.ToAction<SubscribersController>(x => x.ImportSubscribersToList(accountName, listId, null)),
                _link.ToAction<SubscribersController>(x => x.SubscribeToList(accountName, listId, null)),
                _link.ToAction<SubscribersController>(c => c.Unsubscribe(accountName, null)))

            // Adds page links to output model (first, prev, next, etc)
            .AddPageLinks(
                (p, pp) => _link.ToAction<SubscribersController>(x => x.GetSubscribersOfList(accountName, listId, p, pp)))

            // Adds links to each child item (each subscriber in the page)
            .AddItemsLinks(item => new[]
            {
                _link.ToAction<SubscribersController>(c => c.GetSubscriber(accountName, item.emailMd5)),
                _link.ToAction<SubscribersController>(c => c.EditSubscriber(accountName, item.emailMd5, null))
            })
            .Cast<SubscriberCollectionPage>();
    }
    // . . .
```

## ASP.NET Middlewares

### ApiErrorHandler Middleware

This middleware render all exceptions as Problem Details (See [Problem Details for HTTP APIs](https://tools.ietf.org/html/draft-ietf-appsawg-http-problem-01))

It catches:

* `Response.StatusCode == 401 (Unauthorized)` and render an `MakingSense.AspNet.HypermediaApi.Problems.UnauthorizedProblem`
* `Response.StatusCode == 403 (Forbidden)` and render a `MakingSense.AspNet.HypermediaApi.Problems.ForbiddenProblem`
* `Response.StatusCode == 404 (Not Found)` and render a `MakingSense.AspNet.HypermediaApi.Problems.RouteNotFoundProblem`
* Any `MakingSense.AspNet.Authentication.Abstractions.AuthenticationException` and render them as `MakingSense.AspNet.HypermediaApi.Problems.AuthenticationErrorProblem`
* Any `MakingSense.AspNet.HypermediaApi.ExceptionHandling.ApiException` and render the inner problem (`Problem` property)
* Any _unhandled exception_ and render them as `MakingSense.AspNet.HypermediaApi.Problems.UnexpectedProblem`

Basic Configuration example:

```csharp
public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc();

        // If you want to automatically render links to self and home resources,
        // it is required to register a LinkHelper.
        services.AddScoped<ILinkHelper, CustomLinkHelper>();
    }

    public void Configure(IApplicationBuilder app, ILoggerFactory loggerfactory)
    {
        app.UseApiErrorHandler();
        app.UseMvc();
    }
}
```

### NotFoundHandler Middleware

It is a terminal middleware, if a request comes to here it throws an `ApiException` with a `RouteNotFoundProblem`.

## MVC

### Suitable validators filters

Suitable validators are used to validate if given a set of parameters it is possible to execute an action depending on the state of the system.

They are useful to implement [HATEOAS](https://en.wikipedia.org/wiki/HATEOAS) because are invoked during link generation and also before execute a task (after parameters validation).

Initialization:

```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // . . .
        services.AddSuitableValidators(typeof(ACustomSuitableValidatorClass).GetAssembly);
        // . . .
```

Assigning to a controller action:

```csharp
public class ListsController
{
    // . . .
    [CreateListRelation(
        Template = "/accounts/{accountName}/lists",
        SuitableValidators = new[] { typeof(CreateListSuitableValidator) })]
    public async Task<CreationResult> CreateList(string accountName, [FromBody] ListModel list)
    {
        // . . .
```

Example implementation for `CreateList` method (previous example):

```csharp
    public class CreateListSuitableValidator : ISuitableValidator
    {
        readonly ICustomService _customService;

        public static readonly string arg_accountName = "accountName";

        public CreateListSuitableValidator([NotNull] customService)
        {
            _customService = customService;
        }

        public async Task<Problem> ValidateAsync(IDictionary<string, TemplateParameter> values)
        {
            if (values[arg_accountName].ForceValue)
            {
                if (await _customService.MaxListsReached((string)values[arg_accountName].ForcedValue))
                {
                    return new MaximumNumberOfListsReachedProblem();
                }
            }
            return null;
        }
    }
}
```

### Validation Filters

Validation Filters are standard MVC filters with common behavior that can be reused in your Hypermedia API project. They are executed after standard MVC validation.

Setup:

```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // . . .
        services.AddMvc(options => {
            options.Filters.Add(new PayloadValidationFilter());
            options.Filters.Add(new RequiredPayloadFilter());
        });
        // . . .
```

#### PayloadValidationFilter

If there was validation errors (`!ModelState.IsValid`), it generates and throws the `ValidationProblem` with information about validation errors.

#### RequiredPayloadFilter

It checks if expected complex types are included in the request, based in the follow criteria:

* _NotNull_ Attributes

    RequiredPayloadFilter verifies if parameters marked as `[NotNull]` are effectively not null. If not, it throws a `ValidationProblem`.

    ```csharp
    public async Task<CreationResult> CreateList(string accountName, [NotNull] [FromBody] ListModel list)
    ```

* Relation Attribute Input

    RequiredPayloadFilter verifies if the argument associated to relation input parameter has value.

    ```csharp
    public class CreateListRelation : ActionRelationAttribute
    {
        public override HttpMethod? Method => HttpMethod.POST;
        public override Type InputModel => typeof(ListModel);
        // . . .
    ```

    ```csharp
    [CreateListRelation(Template = "/accounts/{accountName}/lists")]
    public async Task<CreationResult> CreateList(string accountName, [FromBody] ListModel list)
    ```

### Formatters

#### HypermediaApiJsonInputFormatter

HypermediaApiJsonInputFormatter ignores request `content-type` header assuming that the content type is `application/json`.

#### HypermediaApiJsonOutputFormatter

HypermediaApiJsonOutputFormatter renders output JSON content indented.

### ModelBinders

#### CustomRepresentationModelBinder and CustomRepresentationModel

`CustomRepresentationModelBinder` handles binding of objects that implement `ICustomRepresentationModel` interface.

`CustomRepresentationModel` and `ICustomRepresentationModel` allow parse request content and generate response content based on custom logic.

It is specially useful for resources with only a representation, for example images or other files.

Example:

```csharp
public class Startup
{
    // . . .

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddMvc(options => 
        {
            // . . .

            options.ModelBinders.Add(new CustomRepresentationModelBinder());
        });

        // . . .
    }
}
[NoSchema]
public class CampaignContent : CustomRepresentationModel, IValidatableObject
{
    public override string ContentType => "text/html";

    public string ContentAsText { get; set; }

    public override async Task SetContentAsync(Stream stream)
    {
        using (var reader = new StreamReader(stream))
        {
            ContentAsText = await reader.ReadToEndAsync();
        }
    }

    protected override Task WriteContentAsync(HttpResponse response) => response.WriteAsync(ContentAsText);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrEmpty(ContentAsText))
        { 
            yield return new ValidationResult("CampaignContent cannot be empty.");
        }
    }
}

public class CampaignsController : Controller
{
    // . . .

    [EditCampaignContentRelation(Template = "/campaigns/{campaignId}/content")]
    public async Task<MessageModel> EditCampaignContent(int campaignId, CampaignContent content)
    {
        // Accepts PUT request with a HTML content in body
        await _campaignsApiService.EditContent(campaignId, content.ContentAsText);
    }

    [GetCampaignContentRelation(Template = "/campaigns/{campaignId}/content")]
    public async Task<CampaignContent> GetCampaignContent(int campaignId)
    {
        // Returns a HTML content in body, including `text/html` content type and link headers.
        var contentAsText = await _campaignsApiService.GetContent(campaignId);
        var result = new CampaignContent();
        result.ContentAsText = contentAsText;
        result.AddLinks(
            _link.ToAction<CampaignsController>(x => x.GetCampaignContent(campaignId)).SetSelf(),
            _link.ToAction<CampaignsController>(x => x.GetCampaign(campaignId)).AddRel<ParentRelation>(),
            _link.ToAction<CampaignsController>(x => x.EditCampaignContent(campaignId, null)),
            _link.ToHomeAccount());
        return result;
    }
```

## Utilities

### API Mappers

`ApiMapper<,>` Provides a convenient interface and utilities to implement mappers to and from API models with facilities to map pages and collections.

To register **all** API Mappers in an assembly is enough with the follow line:

```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // . . .
        services.AddApiMappers(typeof(ACustomMapperClass).GetAssembly);
        // . . .
```

Then, the mappers could be injected using the interface:

```csharp
    public class ListsController : Controller
    {
        private readonly ILogger _logger;
        private readonly IApiMapper<InternalClass, ApiOutputClass> _internalToOutputMapper;

        public ListsController(
            [NotNull] ILogger<ListsController> logger,
            [NotNull] IApiMapper<InternalClass, ApiOutputClass> internalToOutputMapper)
        {
            _logger = logger;
            _internalToOutputMapper = internalToOutputMapper;
        }

        // . . .
```
