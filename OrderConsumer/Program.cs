using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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

        var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "rabbitmq";
        var rabbitPort = int.Parse(builder.Configuration["RabbitMq:Port"] ?? "5672");


        cfg.Host($"rabbitmq://{rabbitHost}:{rabbitPort}", h =>
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

// 4️⃣ APLICA MIGRATIONS DE FORMA SEGURA
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ProcessadorDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("🔄 Verificando banco de dados...");

        // Tenta aplicar migrations - não tenta criar o banco se já existir
        db.Database.Migrate();

        logger.LogInformation("✅ Banco verificado com sucesso!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Erro ao verificar banco");

        // Se for erro de banco já existir, ignora
        if (!ex.Message.Contains("already exists"))
        {
            throw;
        }
    }
}

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
