// Global using directives

global using System.Net;
global using System.Text;
global using System.Text.Json;
global using DevSync.PocPro.Shared.Domain.Dtos;
global using DevSync.PocPro.Shared.Domain.Utils;
global using DevSync.PocPro.Shops.ProductsQueryModule.Features.V1.GetProducts;
global using DevSync.PocPro.Shops.ProductsQueryModule.Interfaces;
global using DevSync.PocPro.Shops.ProductsQueryModule.Models;
global using DevSync.PocPro.Shops.ProductsQueryModule.Services;
global using DevSync.PocPro.Shops.Shared.Dtos;
global using DevSync.PocPro.Shops.Shared.Events;
global using FastEndpoints;
global using FluentResults;
global using Marten;
global using Microsoft.AspNetCore.Http;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Hosting;
global using Microsoft.Extensions.Logging;
global using RabbitMQ.Client;
global using RabbitMQ.Client.Events;