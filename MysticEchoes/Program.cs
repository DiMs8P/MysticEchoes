using System.Numerics;
using System.Windows.Media.Animation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MysticEchoes.Core;
using MysticEchoes.Core.Base.Geometry;
using MysticEchoes.Core.Collisions;

namespace MysticEchoes;

public class Program
{
    [STAThread]
    public static void Main()
    {
        // создаем хост приложения
        var host = Host.CreateDefaultBuilder()
            // внедряем сервисы
            .ConfigureServices(services =>
            {
                services.AddSingleton<App>();
                services.AddSingleton<MainWindow>();
                services.AddGame();
            })
            .Build();
        // получаем сервис - объект класса App
        var app = host.Services.GetService<App>();
        // запускаем приложения
        try
        {
            app?.Run();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}