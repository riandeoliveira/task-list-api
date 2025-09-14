using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using SwaggerThemes;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace TodoList.Extensions;

public static class ApiDocExtensions
{
    public static WebApplicationBuilder ConfigureApiDoc(this WebApplicationBuilder builder)
    {
        var apiInfo = new OpenApiInfo { Title = "Todo List API", Version = "v1" };

        builder
            .Services.AddEndpointsApiExplorer()
            .AddSwaggerGen(options =>
            {
                options.OperationFilter<AddHeaderParameter>();
                options.SwaggerDoc("v1", apiInfo);
                options.ExampleFilters();
            });

        var assembly = Assembly.Load("TodoList");

        builder.Services.AddSwaggerExamplesFromAssemblies(assembly);

        return builder;
    }

    public static WebApplication UseApiDoc(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(Theme.UniversalDark);
        }

        return app;
    }
}

public class AddHeaderParameter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= [];

        var enums = new List<IOpenApiAny>
        {
            new OpenApiString("en-US"),
            new OpenApiString("pt-BR"),
        };

        var schema = new OpenApiSchema { Type = "String", Enum = enums };

        var parameter = new OpenApiParameter
        {
            Name = "Accept-Language",
            Description = "The natural language and locale that the client prefers.",
            In = ParameterLocation.Header,
            Schema = schema,
        };

        operation.Parameters.Add(parameter);
    }
}
