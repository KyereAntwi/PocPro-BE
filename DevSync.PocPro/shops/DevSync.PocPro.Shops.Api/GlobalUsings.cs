// Global using directives

global using System.Security.Claims;
global using DevSync.PocPro.Shared.Domain.Abstractions;
global using DevSync.PocPro.Shared.Domain.Exceptions;
global using DevSync.PocPro.Shops.Api.Data;
global using DevSync.PocPro.Shops.Api.EventHandlers;
global using DevSync.PocPro.Shops.Api.Utils;
global using DevSync.PocPro.Shops.OrdersModule.Data;
global using DevSync.PocPro.Shops.OrdersModule.Domain;
global using DevSync.PocPro.Shops.PointOfSales.Data;
global using DevSync.PocPro.Shops.PointOfSales.Domains;
global using DevSync.PocPro.Shops.PrivateCustomers.Data;
global using DevSync.PocPro.Shops.PrivateCustomers.Domain;
global using DevSync.PocPro.Shops.Shard.Grpc;
global using DevSync.PocPro.Shops.Shared.Services;
global using DevSync.PocPro.Shops.StocksModule.Data;
global using DevSync.PocPro.Shops.StocksModule.DI;
global using DevSync.PocPro.Shops.StocksModule.Domains;
global using FastEndpoints;
global using MassTransit;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Design;
global using Polly;
global using Scalar.AspNetCore;
global using Order = DevSync.PocPro.Shops.OrdersModule.Domain.Order;