using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqldiff
{
    public class Program
    {
        static void Main(string[] args)
        {
            /*
            if (args != null && args.Length < 2)
            {
                Console.WriteLine("Please input sql file full path to compare diff with the previous version...");
            }

            if (args != null && args.Length == 2)
            {
                try
                {
                    var factory = new Factory(args[0]);
                    factory.Run();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            */

            var previous = @"D:\Project\Arthor\sqldiff\ins_smp_SYSTEM_EVENT.sqlx";
            var current = @"D:\Project\Arthor\sqldiff\ins_smp_SYSTEM_EVENT.sqlx";
            try
            {
                var factory = new Factory(previous, current);
                factory.Compare();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
