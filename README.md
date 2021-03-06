# Summary
munERP (muncinsky Enterprise resource planning) is an experimental project implementing basic ERP system based on microservice architecture using design like DDD and CQRS. Purpose is to show common concepts to other developers or colleagues, but core of the project is mature enough to be used even in the production.

Project is composed of services each representing one Bounded context from DDD's perspective (Accounting, Sales,...) and client applications. Currently there is only one client application, which is for win desktop and is developed in WPF. Another technologies will be added soon.

# Main concepts
- **Plugin architecture** - Project is developed with emphasis of possibility of trying and quick replacement of various technologies (even IOC container), therefore implementations are strictly separated from abstractions, and they are loaded dynamically during runtime from *.Plugin.* modules.
- **Common messaging interface** - Introduced common message bus interface that can be implemented by various message channels (currently EasyNetQ/RabbitMQ, ASP Web.API, in process channel). Benefit is that we can quickly map any message to a particular channel that we need and preserve the same cross-cutting concerns (logging, caching,...) handled by message filters.

# Getting started
Project can be built inside of Docker container. Depending on type of your Docker host use:
 - either [build.dev.docker.linux.bat](build/build.dev.docker.linux.bat)
 - or [build.dev.docker.win.bat](build/build.dev.docker.win.bat)
 
 Have in mind that build running inside of linux container doesn't build windows desktop client.
 
 To run project inside of container use:
 - either [run.dev.docker.linux.bat](build/run.dev.docker.linux.bat)
 - or [run.dev.docker.win.bat](build/run.dev.docker.win.bat)
 
 This will start services as well as depending infrastructure like Sql server or RabbitMQ broker.
