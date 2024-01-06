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
    public Vector2 mousePosition;

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
        Point mousePositionP = e.GetPosition(GlControl);
        mousePosition = new Vector2((float)mousePositionP.X, (float)mousePositionP.Y);
        mousePosition.X = 2.0f * mousePosition.X / (float)ActualWidth;
        mousePosition.Y = 2.0f * ((float)((System.Windows.FrameworkElement)sender).ActualHeight - mousePosition.Y) / (float)((System.Windows.FrameworkElement)sender).ActualHeight;
    }
}