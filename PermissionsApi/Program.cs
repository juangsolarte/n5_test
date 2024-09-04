using Microsoft.EntityFrameworkCore;
using Nest;
using PermissionsApi.command;
using PermissionsApi.Data;
using PermissionsApi.Models.Dto;
using PermissionsApi.Repositories;
using PermissionsApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Configuracion del cliente de Elasticsearch
var settings = new ConnectionSettings(new Uri("http://localhost:9200"))
    .DefaultIndex("permissions");
var client = new ElasticClient(settings);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder => builder
            .WithOrigins("http://localhost:3000") // Reemplaza con la URL de tu frontend
            .AllowAnyMethod()
            .AllowAnyHeader());
});

// Add services to the container.
builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddControllers();

builder.Services.AddScoped<IPermissionRepository, PermissionRepository>();
builder.Services.AddScoped<IPermissionTypeRepository, PermissionTypeRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Registro de los Command Handlers
builder.Services.AddScoped<ICommandHandler<CreatePermissionCommand>, CreatePermissionCommandHandler>();
builder.Services.AddScoped<ICommandHandler<UpdatePermissionCommand>, UpdatePermissionCommandHandler>();

// Registro de los Query Handlers
builder.Services.AddScoped<IQueryHandler<GetPermissionTypesQuery, IEnumerable<PermissionTypeDto>>, GetPermissionTypesQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetPermissionsQuery, IEnumerable<PermissionDto>>, GetPermissionsQueryHandler>();
builder.Services.AddScoped<IQueryHandler<GetPermissionByIdQuery, PermissionDto>, GetPermissionByIdQueryHandler>();

builder.Services.AddSingleton<KafkaProducerService>();
builder.Services.AddSingleton<IElasticClient>(new ElasticClient(settings));

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IElasticClient>(client);

var app = builder.Build();

// Usar CORS
app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();

