using SharpGL.WPF;
using System.Windows;

namespace MysticEchoes;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void GlControl_OnOpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
    {

    }

    private void GlControl_OnResized(object sender, OpenGLRoutedEventArgs args)
    {

    }

    private void GlControl_OnOpenGLDraw(object sender, OpenGLRoutedEventArgs args)
    {


    }
}