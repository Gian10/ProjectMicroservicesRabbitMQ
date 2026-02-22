using MassTransit;
using Microsoft.EntityFrameworkCore;
using OrderConsumer.Consumers;
using OrderConsumer.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

// CONFIGURA BANCO DE DADOS
builder.Services.AddDbContext<ProcessadorDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// CONFIGURA MASSTRANSIT
builder.Services.AddMassTransit(x =>
{
    // Registra o consumer
    x.AddConsumer<OrderCreatedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("localhost", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        cfg.ReceiveEndpoint("fila-pedidos-processador", e =>
        {
            e.ConfigureConsumer<OrderCreatedConsumer>(context);

            // Configurações de resiliência
            e.UseMessageRetry(r => r
                .Interval(3, TimeSpan.FromSeconds(5))
                .Handle<Exception>());

            e.PrefetchCount = 10;
        });
    });
});

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
   // app.UseSwagger();
    //app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
