# Sistema de Processamento de Pedidos com Microserviços e Mensageria

⚙️ COMPONENTES PRINCIPAIS

1️⃣ Serviço A - CreateOrder (API de Pedidos)
Tecnologias: ASP.NET Core 8, Entity Framework Core, MassTransit

Função: Receber pedidos via REST API

Diferenciais:

✅ Implementa Outbox Pattern para garantir consistência

✅ Transações atômicas com SQL Server

✅ Resposta rápida ao cliente (< 50ms)

2️⃣ Serviço B - OrderProcessor (Processador)
Tecnologias: .NET Worker Service, MassTransit, Entity Framework Core

Função: Processar pedidos em background

Diferenciais:

✅ Processamento assíncrono e paralelo

✅ Retry automático em falhas

✅ Logging completo de operações

3️⃣ Message Broker - RabbitMQ
Função: Transporte assíncrono de mensagens

Diferenciais:

✅ Garantia de entrega

✅ Persistência de mensagens

✅ Dead Letter Queue para falhas

4️⃣ Bancos de Dados - SQL Server
Serviço A: PedidosDb - Armazena pedidos criados

Serviço B: ProcessadorDb - Armazena logs de processamento

Diferenciais:

✅ Cada serviço com seu próprio banco (isolamento)

✅ Migrations com Entity Framework

✅ Índices otimizados para performance



