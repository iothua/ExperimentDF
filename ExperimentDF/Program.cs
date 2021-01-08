using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace ExperimentDF
{
    class Program
    {
        private static IContainer Container { get; set; }

        static void Main(string[] args)
        { 
            
        }

        public static void WriteDate()
        {
            using (var scope = Container.BeginLifetimeScope())
            { 
                
            }
        }
    }
}
