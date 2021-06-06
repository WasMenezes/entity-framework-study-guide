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
            FiltroGlobal();
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
