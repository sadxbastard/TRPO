using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace WpfApp1
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IMemory, RAMMemory>();
            //serviceCollection.AddSingleton<IMemory, FileMemory>();
            //serviceCollection.AddSingleton<IMemory, DbMemory>();

            serviceCollection.AddSingleton<MainViewModel>();
            Provider = serviceCollection.BuildServiceProvider();
        }
        public static ServiceProvider Provider { get; private set; }
    }
}
