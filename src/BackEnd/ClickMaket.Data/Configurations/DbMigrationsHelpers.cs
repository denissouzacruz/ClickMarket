using ClickMarket.Data.Context;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ClickMarket.Data.Configurations;

public static class DbMigrationsHelpers
{
    public static async Task EnsureSeedData(WebApplication serviceScope)
    {
        var services = serviceScope.Services.CreateScope().ServiceProvider;
        await EnsureSeedData(services);
    }

    public static async Task EnsureSeedData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var env = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        var contextID = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var contextClick = scope.ServiceProvider.GetRequiredService<ClickDbContext>();

        if (env.IsDevelopment())
        {

            await contextID.Database.MigrateAsync();
            await contextClick.Database.MigrateAsync();

            await EnsureSeedProducts(contextID, contextClick, scope.ServiceProvider);
        }
    }

    private static async Task EnsureSeedProducts(ApplicationDbContext identityDb, ClickDbContext clickDb, IServiceProvider serviceProvider)
    {
        //Realiza a carga inicial dos dados
        if (clickDb.Categorias.Any())
            return;

        var roleManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();

        if (!await roleManager.RoleExistsAsync("Administrador"))
        {
            await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole("Administrador"));
        }
        if (!await roleManager.RoleExistsAsync("Vendedor"))
        {
            await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole("Vendedor"));
        }
        if (!await roleManager.RoleExistsAsync("Cliente"))
        {
            await roleManager.CreateAsync(new Microsoft.AspNetCore.Identity.IdentityRole("Cliente"));
        }

        //Adiciona os usuários iniciais
        //Administrador, Vendedor e Cliente
        await AdicionarAdministrador(identityDb, serviceProvider);
        var idVendedor = await AdicionarVendedor(identityDb, clickDb, serviceProvider);
        await AdicionarCliente(identityDb, clickDb, serviceProvider);

        var categoriaPapelaria = Guid.NewGuid();
        var categoriaInformatica = Guid.NewGuid();

        clickDb.Categorias.AddRange(
            new Business.Models.Categoria()
            {
                Id = categoriaPapelaria,
                Nome = "Papelaria",
                Descricao = "Materiais Acadêmicos"
            },
            new Business.Models.Categoria()
            {
                Id = categoriaInformatica,
                Nome = "Informática",
                Descricao = "Artigos eletrônicos e informática em geral"
            }
        );
        await clickDb.SaveChangesAsync();

        if (clickDb.Produtos.Any())
            return;

        clickDb.Produtos.AddRange(
            new Business.Models.Produto()
            {
                Id = Guid.NewGuid(),
                Nome = "Livro A Cabana",
                Descricao = "Livro A Cabana / Literatura Estrangeira",
                CategoriaId = categoriaPapelaria,
                VendedorId = idVendedor,
                Imagem = "5be38508-8799-4625-8c68-77100b36c7fc-Acabana.jpg",
                Valor = 30,
                QuantidadeEstoque = 245,
                Ativo = true
            },
            new Business.Models.Produto()
            {
                Id = Guid.NewGuid(),
                Nome = "Teclado Logi",
                Descricao = "Teclado Logi, sem bluetooth",
                CategoriaId = categoriaInformatica,
                VendedorId = idVendedor,
                Imagem = "6afb183a-744b-4f99-b394-6180f9d3b1dd-teclado_logi.png",
                Valor = 122,
                QuantidadeEstoque = 198,
                Ativo = true
            }
        );

        await clickDb.SaveChangesAsync();
    }

    public async static Task AdicionarAdministrador(ApplicationDbContext identityDb, IServiceProvider serviceProvider)
    {
        var idUsuario = Guid.NewGuid();
        await identityDb.Users.AddAsync(new Microsoft.AspNetCore.Identity.IdentityUser
        {
            Id = idUsuario.ToString(),
            UserName = "admin@admin.com",
            NormalizedUserName = "ADMIN@ADMIN.COM",
            Email = "admin@admin.com",
            NormalizedEmail = "ADMIN@ADMIN.COM",
            AccessFailedCount = 0,
            LockoutEnabled = false,
            PasswordHash = "AQAAAAIAAYagAAAAEA8BzmHCVEcOD+VNHR7Z91SjCRm9Zc4yodRPaowNC98ttq1IuwawRlqBzwUPidXCnw==",
            TwoFactorEnabled = false,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        });

        await identityDb.SaveChangesAsync();

        //set role Administrador for user
        var userManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Microsoft.AspNetCore.Identity.IdentityUser>>();
        var user = await userManager.FindByIdAsync(idUsuario.ToString());
        if (user != null)
        {
            await userManager.AddToRoleAsync(user, "Administrador");
        }
    }

    public static async Task<Guid> AdicionarVendedor(ApplicationDbContext identityDb, ClickDbContext clickDb, IServiceProvider serviceProvider)
    {
        var idUsuario = Guid.NewGuid();
        await identityDb.Users.AddAsync(new Microsoft.AspNetCore.Identity.IdentityUser
        {
            Id = idUsuario.ToString(),
            UserName = "vendedor@vendedor.com",
            NormalizedUserName = "VENDEDOR@VENDEDOR.COM",
            Email = "vendedor@vendedor.com",
            NormalizedEmail = "VENDEDOR@VENDEDOR.COM",
            AccessFailedCount = 0,
            LockoutEnabled = false,
            PasswordHash = "AQAAAAIAAYagAAAAEA8BzmHCVEcOD+VNHR7Z91SjCRm9Zc4yodRPaowNC98ttq1IuwawRlqBzwUPidXCnw==",
            TwoFactorEnabled = false,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        });

        await identityDb.SaveChangesAsync();

        //set role Administrador for user
        var userManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Microsoft.AspNetCore.Identity.IdentityUser>>();
        var user = await userManager.FindByIdAsync(idUsuario.ToString());
        if (user != null)
        {
            await userManager.AddToRoleAsync(user, "Vendedor");
        }

        clickDb.Vendedores.Add(new Business.Models.Vendedor() { Id = idUsuario, Nome = "Vendedor", Email = "vendedor@vendedor.com", Ativo = true });
        await clickDb.SaveChangesAsync();

        return idUsuario;
    }

    public static async Task AdicionarCliente(ApplicationDbContext identityDb, ClickDbContext clickDb, IServiceProvider serviceProvider)
    {
        var idUsuario = Guid.NewGuid();
        await identityDb.Users.AddAsync(new Microsoft.AspNetCore.Identity.IdentityUser
        {
            Id = idUsuario.ToString(),
            UserName = "cliente@cliente.com",
            NormalizedUserName = "CLIENTE@CLIENTE.COM",
            Email = "cliente@cliente.com",
            NormalizedEmail = "CLIENTE@CLIENTE.COM",
            AccessFailedCount = 0,
            LockoutEnabled = false,
            PasswordHash = "AQAAAAIAAYagAAAAEA8BzmHCVEcOD+VNHR7Z91SjCRm9Zc4yodRPaowNC98ttq1IuwawRlqBzwUPidXCnw==",
            TwoFactorEnabled = false,
            ConcurrencyStamp = Guid.NewGuid().ToString(),
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        });

        await identityDb.SaveChangesAsync();

        //set role Administrador for user
        var userManager = serviceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<Microsoft.AspNetCore.Identity.IdentityUser>>();
        var user = await userManager.FindByIdAsync(idUsuario.ToString());
        if (user != null)
        {
            await userManager.AddToRoleAsync(user, "Cliente");
        }

        clickDb.Clientes.Add(new Business.Models.Cliente() { Id = idUsuario, Nome = "Cliente", Email = "cliente@cliente.com", Ativo = true });
        await clickDb.SaveChangesAsync();
    }
}
