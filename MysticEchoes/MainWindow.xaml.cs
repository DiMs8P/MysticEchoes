using SharpGL.WPF;
using System.Numerics;
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
    public Vector2 _mousePosition;

    private void GlControl_OnOpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
    {

    }

    private void GlControl_OnResized(object sender, OpenGLRoutedEventArgs args)
    {

    }

    private void GlControl_OnOpenGLDraw(object sender, OpenGLRoutedEventArgs args)
    {


    }

    private void GlControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        Point mousePosition = e.GetPosition(GlControl);
        _mousePosition = new Vector2((float)mousePosition.X, (float)mousePosition.Y);
    }
}