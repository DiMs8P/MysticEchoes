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
        //app?.Run();

        var qt = new QuadTree(new Rectangle(
            Vector2.Zero, new Vector2(40, 40)
            ),
            4
        );

        qt.Add(new Box(1, new Rectangle(new Vector2(-1, -1), new Vector2(4, 4))));
        qt.Add(new Box(2, new Rectangle(new Vector2(5, 5), new Vector2(5, 13))));
        qt.Add(new Box(3, new Rectangle(new Vector2(10, 10), new Vector2(20, 20))));
        qt.Add(new Box(4, new Rectangle(new Vector2(33, 21), new Vector2(3, 9))));
        qt.Add(new Box(5, new Rectangle(new Vector2(2, 2), new Vector2(5, 5))));
        qt.Add(new Box(6, new Rectangle(new Vector2(5, 25), new Vector2(10, 14))));
        qt.Add(new Box(7, new Rectangle(new Vector2(8, 3), new Vector2(6, 11))));

        var d = qt.MaxDepthQuery();
        var finded = qt.Query(new Rectangle(new Vector2(20, 35), new Vector2(15, 4)));
        var finded1 = qt.Query(new Rectangle(new Vector2(0, 0), new Vector2(10, 10)));
        var finded2 = qt.Query(new Rectangle(new Vector2(6, 26), new Vector2(1, 1)));
        var finded3 = qt.Query(new Rectangle(new Vector2(6, 26), new Vector2(28, 1)));
        var finded4 = qt.Query(new Rectangle(new Vector2(4, 24), new Vector2(0.5f, 0.5f)));


    }
}