using System;

namespace DominandoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            EnsureCreatedAndDelete();
        }

        static void EnsureCreatedAndDelete()
        {
            using var db = new Data.ApplicationContext();
            //db.Database.EnsureCreated();
            db.Database.EnsureDeleted();

        }
    }
}
