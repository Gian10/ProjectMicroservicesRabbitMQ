using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Data;
using Order.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CONFIGURA SQL SERVER
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));



// 2️⃣ CONFIGURA MASSTRANSIT
builder.Services.AddMassTransit(x =>
{
    x.UsingRabbitMq((context, cfg) =>
    {
        // 🔥 LÊ DO APPSETTINGS OU VARIÁVEL DE AMBIENTE
        //var rabbitHost = builder.Configuration["RabbitMq:Host"] ?? "localhost";
        //var rabbitPort = int.Parse(builder.Configuration["RabbitMq:Port"] ?? "5672");
        // 🔥 TEMPORARIAMENTE - FIXO PARA TESTE!
        var rabbitHost = "rabbitmq";
        var rabbitPort = 5672;  // ← FIXO EM 5672!

        Console.WriteLine($"🔌 Conectando ao RabbitMQ em {rabbitHost}:{rabbitPort}");


        // SINTAXE CORRETA do MassTransit
        cfg.Host($"rabbitmq://{rabbitHost}:{rabbitPort}", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });
        // 🔥 ADICIONAR RETRY NA CONEXÃO
        cfg.UseMessageRetry(r => r.Interval(5, TimeSpan.FromSeconds(5)));

        cfg.ConfigureEndpoints(context);
    });
});

builder.Services.Configure<MassTransitHostOptions>(options =>
{
    options.WaitUntilStarted = true;
});

// Outbox Publisher
builder.Services.AddHostedService<OutboxPublisher>();


var app = builder.Build();

// 🔥 ADICIONAR ISSO - CRIA/ATUALIZA BANCO AUTOMATICAMENTE!
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        logger.LogInformation("🔄 Aplicando migrations...");
        db.Database.Migrate();
        logger.LogInformation("✅ Migrations aplicadas com sucesso!");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "❌ Erro ao aplicar migrations");
        throw;
    }
}



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
