using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;

namespace DominandoEFCore
{
    class Program
    {

        static void Main(string[] args)
        {
            DivisaoDeConsulta();
        }

        static void FiltroGlobal()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToList();

            foreach(var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}\t Excluido: {departamento.Excluido}");
            }
        }

        static void IgnoreFiltroGlobal()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos.IgnoreQueryFilters().Where(p => p.Id > 0).ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}\t Excluido: {departamento.Excluido}");
            }
        }

        static void ConsultaProjetada()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
                .Where(p => p.Id > 0)
                .Select(p => new { p.Descricao, Funcionarios = p.Funcionarios.Select(f => f.Nome) })
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"\t Nome: {funcionario}");
                }
            }
        }
        static void ConsultaParametrizada()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var id = new SqlParameter
            {
                Value = 1,
                SqlDbType = System.Data.SqlDbType.Int
            };
            var departamentos = db.Departamentos
                .FromSqlRaw("SELECT * FROM Departamentos WHERE Id > {0}", id)
                .Where(p => !p.Excluido)
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }
        static void ConsultaInterpolada()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var id = 1;
            var departamentos = db.Departamentos
                .FromSqlInterpolated($"SELECT * FROM Departamentos WHERE Id > {id}")
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }
        static void ConsultaComTag()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
                .TagWith(@"Estou enviando um comentário para o servidor
                Segundo comentário
                Terceiro comentário")
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
            }
        }
        static void EntendendoConsulta1NN1()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var funcionarios = db.Funcionarios
                .Include(p => p.Departamento)
                .ToList();

            foreach (var funcionario in funcionarios)
            {
                Console.WriteLine($"Nome: {funcionario.Nome} / Descricao Dep: {funcionario.Departamento.Descricao}");
            }

            /*
            var departamentos = db.Departamentos
                .Include(p => p.Funcionarios)
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");

                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"Nome: {funcionario.Nome}");
                }
            }
            */
        }
        static void DivisaoDeConsulta()
        {
            using var db = new Data.ApplicationContext();
            Setup(db);

            var departamentos = db.Departamentos
                .Include(p => p.Funcionarios)
                //.AsSplitQuery()
                .AsSingleQuery()
                .Where(p => p.Id < 3)
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine($"Descrição: {departamento.Descricao}");
                foreach (var funcionario in departamento.Funcionarios)
                {
                    Console.WriteLine($"Nome: {funcionario.Nome}");
                }
            }
        }
        static void Setup(Data.ApplicationContext db)
        {
            if (db.Database.EnsureCreated())
            {
                db.Departamentos.AddRange(
                    new Domain.Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<Domain.Funcionario>
                        {
                            new Domain.Funcionario
                            {
                                Nome ="Rafael Almeida",
                                CPF ="88888888811",
                                RG = "2100062"
                            }
                        },
                        Excluido = true
                    },
                    new Domain.Departamento
                    {
                        Ativo = true,
                        Descricao = "Departamento 02",
                        Funcionarios = new System.Collections.Generic.List<Domain.Funcionario>
                        {
                            new Domain.Funcionario
                            {
                                Nome ="Bruno Brito",
                                CPF ="99999999911",
                                RG = "3100062"
                            },
                            new Domain.Funcionario
                            {
                                Nome ="Eduardo Pires",
                                CPF ="77777777711",
                                RG = "1100062"
                            }
                        }
                    });
                db.SaveChanges();
                db.ChangeTracker.Clear(); 
            }
        }
    }
}
