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
            GapDoEnsureCreated();
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
    }
}
