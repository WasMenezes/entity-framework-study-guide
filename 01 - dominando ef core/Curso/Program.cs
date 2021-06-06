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
            CarregamentoLento();
        }

        static void HealthCheckBancoDeDados()
        {
            using var db = new Data.ApplicationContext();
            var canConnect = db.Database.CanConnect();

            if (canConnect)
            {
                Console.WriteLine("Posso me conectar");
            }
            else
            {
                Console.WriteLine("Não posso me conectar");
            }

            /*
            try
            {
                //1
                var connection = db.Database.GetDbConnection();
                connection.Open();

                //2
                db.Departamentos.Any();
                
                Console.WriteLine("Posso me conectar");
            }
            catch(Exception)
            {
                Console.WriteLine("Não posso me conectar");
            }
            */
        }
        static void EnsureCreatedAndDelete()
        {
            using var db = new Data.ApplicationContext();
            //db.Database.EnsureCreated();
            db.Database.EnsureDeleted();
        }

        static void GapDoEnsureCreated()
        {
            using var db1 = new Data.ApplicationContext();
            using var db2 = new Data.ApplicationContextCidade();

            db1.Database.EnsureCreated();
            db2.Database.EnsureCreated();

            var databaseCreator = db2.GetService<IRelationalDatabaseCreator>();
            databaseCreator.CreateTables();
        }

        static int _count;
        static void GerenciarEstadoDaConexao(bool gerenciarEstadoConexao)
        {
            using var db = new Data.ApplicationContext();
            var time = System.Diagnostics.Stopwatch.StartNew();

            var conexao = db.Database.GetDbConnection();
            conexao.StateChange += (_, __) => ++_count;

            if(gerenciarEstadoConexao)
            {
                conexao.Open();
            }

            for (var i = 0; i < 200; i++)
            {
                db.Departamentos.AsNoTracking().Any();
            }

            time.Stop();
            var mensagem = $"Tempo: {time.Elapsed.ToString()}, {gerenciarEstadoConexao}, Contador: {_count}";

            Console.WriteLine(mensagem);
        }
        static void ExecuteSQL()
        {
            using var db = new Data.ApplicationContext();
            //Primeira Opção
            using(var cmd = db.Database.GetDbConnection().CreateCommand())
            {
                cmd.CommandText = "SELECT 1";
                cmd.ExecuteNonQuery();
            }

            //Segunda Opção
            var descricao = "Teste";
            db.Database.ExecuteSqlRaw("update departamentos set descricao={0} where id=1", descricao);

            //Terceira Opção 
            db.Database.ExecuteSqlInterpolated($"update departamentos set descricao={descricao} where id=1");
        }
        static void SqlInjection()
        {
            using var db = new Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            db.Departamentos.AddRange(
                new Domain.Departamento { Descricao = "Departamento 01" },
                new Domain.Departamento { Descricao = "Departamento 02" });
            db.SaveChanges();

            var descricao = "Teste ' or 1 = '1";
            //Jeito errado, permite o SQL Injection
            db.Database.ExecuteSqlRaw($"update departamentos set descricao='AtaqueSqlInjection' where descricao='{descricao}'");
            
            //Jeito certo, não permite o SQL Injection
            db.Database.ExecuteSqlRaw("update departamentos set descricao='AtaqueSqlInjection' where descricao={0}", descricao);


            foreach (var departamento in db.Departamentos.AsNoTracking())
            {
                Console.WriteLine($"Id {departamento.Id}, Descricao: {departamento.Descricao}");
            }
        }

        static void MigracoesPendentes()
        {
            using var db = new Data.ApplicationContext();

            var migracoesPendentes = db.Database.GetPendingMigrations();

            Console.WriteLine($"TOtal: {migracoesPendentes.Count()}");

            foreach(var migracao in migracoesPendentes)
            {
                Console.WriteLine($"Migracao: {migracao}");
            }
        }
         
        static void AplicatMigracaoEmTempodeExecucao()
        {
            using var db = new Data.ApplicationContext();
            db.Database.Migrate();
        }

        static void TodasMigracoes()
        {
            using var db = new Data.ApplicationContext();
            var migracoes = db.Database.GetMigrations();
            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach(var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        static void MigracoesJaAplicadas()
        {
            using var db = new Data.ApplicationContext();
            var migracoes = db.Database.GetAppliedMigrations();
            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        static void MigracoesNaoAplicadas()
        {
            using var db = new Data.ApplicationContext();
            var migracoes = db.Database.GetPendingMigrations();
            Console.WriteLine($"Total: {migracoes.Count()}");

            foreach (var migracao in migracoes)
            {
                Console.WriteLine($"Migração: {migracao}");
            }
        }

        static void ScriptGeralDoBancoDeDados()
        {
            using var db = new Data.ApplicationContext();
            var script = db.Database.GenerateCreateScript();

            Console.WriteLine(script);
        }

        static void CarregamentoAdiantado()
        {
            using var db = new Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            var departamentos = db
                .Departamentos
                .Include(p => p.Funcionarios);

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if(departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }

            }
        }

        static void CarregamentoExplicito()
        {
            using var db = new Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            var departamentos = db
                .Departamentos
                .ToList();

            //Ou usar o MultipleActiveResultSets=true na string de conexão

            foreach (var departamento in departamentos)
            {
                if(departamento.Id == 2)
                {
                    //db.Entry(departamento).Collection(p => p.Funcionarios).Load();
                    db.Entry(departamento).Collection(p => p.Funcionarios).Query().Where(p=>p.Id > 2).ToList();
                }
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

        static void SetupTiposCarregamentos(Data.ApplicationContext db)
        {
            if(!db.Departamentos.Any())
            {
                db.Departamentos.AddRange(
                    new Domain.Departamento
                    {
                        Descricao = "Departamento 01",
                        Funcionarios = new System.Collections.Generic.List<Domain.Funcionario>
                        {
                            new Domain.Funcionario
                            {
                                Nome = "Waschington Luiz",
                                CPF = "99999999911",
                                RG = "2100062"
                            }
                        }
                    },
                    new Domain.Departamento
                    {
                        Descricao = "Departamento 02",
                        Funcionarios = new System.Collections.Generic.List<Domain.Funcionario>
                        {
                            new Domain.Funcionario
                            {
                                Nome= "Other Was",
                                CPF = "12341234911",
                                RG = "3100062"
                            },
                            new Domain.Funcionario
                            {
                                Nome= "Eduardo Pires",
                                CPF = "13913234911",
                                RG = "4100062"
                            }
                        }
                    });
                db.SaveChanges();
                db.ChangeTracker.Clear();
            }
        }
        static void CarregamentoLento()
        {
            using var db = new Data.ApplicationContext();
            SetupTiposCarregamentos(db);

            //db.ChangeTracker.LazyLoadingEnabled = false;
            
            var departamentos = db
                .Departamentos
                .ToList();

            foreach (var departamento in departamentos)
            {
                Console.WriteLine("------------------------------------------------");
                Console.WriteLine($"Departamento: {departamento.Descricao}");

                if (departamento.Funcionarios?.Any() ?? false)
                {
                    foreach (var funcionario in departamento.Funcionarios)
                    {
                        Console.WriteLine($"\tFuncionario: {funcionario.Nome}");
                    }
                }
                else
                {
                    Console.WriteLine($"\tNenhum funcionario encontrado!");
                }
            }
        }

    }
}
