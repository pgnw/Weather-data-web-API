
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using Weather_data_web_api.Repositories;
using Weather_data_web_api.Services;
using Weather_data_web_api.Settings;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("apiKey", new OpenApiSecurityScheme
    {
        Description = @"Api key to manage user acsess.
                        Enter your provided key in the box below",
        Name = "apiKey",
        In = ParameterLocation.Query,
        Type = SecuritySchemeType.ApiKey
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            // Add a new security scheme to get users to input their api key.
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "apiKey"
                },

                Name = "apiKey",
                In = ParameterLocation.Query
            },
            new List<string>()
        }
    });

    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "Weather data web api.xml"));
});


// Sets up the MongoConnection class from Services and configure it to hold the connection details from appsettings.json.
builder.Services.Configure<MongoConnection>(builder.Configuration.GetSection("MongoConnection"));
//Adds the connection builder to the services collection of the dependency injection.
builder.Services.AddScoped<MongoConnectionBuilder>();
//Adds the WeatherDataRepository class to the services container.
builder.Services.AddScoped<IWeatherDataRepository, WeatherDataRepository>();
builder.Services.AddScoped<IUserDataRepository, UserRepository>();

// Add google to the cors whitelist for GET, POST, PUT and DELETE methods.
builder.Services.AddCors(options =>
{
    options.AddPolicy("GooglePolicy", p =>
    {
        p.WithOrigins("https://www.google.com", "https://www.google.com.au");
        p.AllowAnyHeader();
        p.WithMethods("GET", "POST", "PUT", "DELETE");

    });
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
// Allow the WeatherDataFilter class to be serialized.
var objectSerializer = new ObjectSerializer(type => ObjectSerializer.DefaultAllowedTypes(type) || type.FullName.StartsWith("Weather_data_web_api.Models.Filter")); BsonSerializer.RegisterSerializer(objectSerializer);

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();

app.Run();
