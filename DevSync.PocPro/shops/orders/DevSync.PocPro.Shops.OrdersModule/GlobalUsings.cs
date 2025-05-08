// Global using directives

global using System.Collections.ObjectModel;
global using System.Security.Claims;
global using DevSync.PocPro.Shared.Domain.Abstractions;
global using DevSync.PocPro.Shared.Domain.Dtos;
global using DevSync.PocPro.Shared.Domain.Enums;
global using DevSync.PocPro.Shared.Domain.Events;
global using DevSync.PocPro.Shared.Domain.Exceptions;
global using DevSync.PocPro.Shared.Domain.ValueObjects;
global using DevSync.PocPro.Shops.OrdersModule.Data;
global using DevSync.PocPro.Shops.OrdersModule.Domain;
global using DevSync.PocPro.Shops.OrdersModule.Domain.Enums;
global using DevSync.PocPro.Shops.OrdersModule.Domain.Interfaces;
global using DevSync.PocPro.Shops.OrdersModule.Domain.Services;
global using DevSync.PocPro.Shops.OrdersModule.Domain.ValueObjects;
global using DevSync.PocPro.Shops.OrdersModule.Features.Orders.GetOrder;
global using DevSync.PocPro.Shops.PointOfSalesModule.Features.Grpc;
global using DevSync.PocPro.Shops.Shared.Services;
global using DevSync.PocPro.Shops.StocksModule.Features.Grpc;
global using FastEndpoints;
global using FluentResults;
global using MassTransit;
global using Microsoft.AspNetCore.Http;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.EntityFrameworkCore.Metadata.Builders;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Order = DevSync.PocPro.Shops.OrdersModule.Domain.Order;