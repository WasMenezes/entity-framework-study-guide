using Microsoft.EntityFrameworkCore;
using SistemaPedidos.Domain;
using SistemaPedidos.ValueObjects;
using System;
using System.Linq;

namespace SistemaPedidos
{
    class Program
    {
        static void Main(string[] args)
        {
            //InserirDados();
            //InserirDadosEmMassa();
            ConsultarDados();
        }

        private static void InserirDados()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = TipoProduto.Mercadoria,
                Ativo = true
            };
            using var db = new Data.ApplicationContext();
            // db.Produtos.Add(produto);
            // db.Set<Produto>().Add(produto);
            // db.Entry(produto).State = EntityState.Added
            db.Add(produto);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total Registro(s): {registros}");
        }

        private static void InserirDadosEmMassa()
        {
            var produto = new Produto
            {
                Descricao = "Produto Teste",
                CodigoBarras = "1234567891231",
                Valor = 10m,
                TipoProduto = TipoProduto.Mercadoria,
                Ativo = true
            };

            var cliente = new Cliente
            {
                Nome = "Waschington Luiz",
                CEP = "11111111",
                Cidade = "Colatina",
                Estado = "ES",
                Telefone = "999999999"
            };

            using var db = new Data.ApplicationContext();

            db.AddRange(produto, cliente);

            var registros = db.SaveChanges();
            Console.WriteLine($"Total Registro(s): {registros}");
        }

        private static void ConsultarDados()
        {
            using var db = new Data.ApplicationContext();
            //var consultaPorSintaxe = (from c in db.Clientes where c.Id > 0 select c).ToList();
            var consultaPorMetodo = db.Clientes
                .Where(p => p.Id > 0)
                .OrderBy(p=>p.Id)
                .ToList();

            foreach(var cliente in consultaPorMetodo)
            {
                Console.WriteLine($"Consultando Cliente: {cliente.Id}");
                ///db.Clientes.Find(cliente.Id);
                db.Clientes.FirstOrDefault(p => p.Id == cliente.Id);
            }
        }
    }
}
