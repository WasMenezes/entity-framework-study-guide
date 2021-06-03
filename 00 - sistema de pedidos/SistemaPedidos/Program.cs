using SistemaPedidos.Domain;
using SistemaPedidos.ValueObjects;
using System;

namespace SistemaPedidos
{
    class Program
    {
        static void Main(string[] args)
        {
            //InserirDados();
            InserirDadosEmMassa();
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
    }
}
