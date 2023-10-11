using System.Windows;

namespace MysticEchoes;

public class App : Application
{
    readonly MainWindow mainWindow;

    public App(MainWindow mainWindow)
    {
        this.mainWindow = mainWindow;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        mainWindow.Show();
        base.OnStartup(e);

    }
}