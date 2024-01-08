using SharpGL;
using SharpGL.WPF;
using System.Numerics;
using System.Windows;

namespace MysticEchoes;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    OpenGL gl;
    public MainWindow()
    {
        InitializeComponent();
    }
    public Vector2 MousePosition;

    private void GlControl_OnOpenGLInitialized(object sender, OpenGLRoutedEventArgs args)
    {
        
    }

    private void GlControl_OnResized(object sender, OpenGLRoutedEventArgs args)
    {
        OpenGL gl = args.OpenGL;
        gl.MatrixMode(OpenGL.GL_PROJECTION);

        gl.LoadIdentity();
        gl.Ortho(0.75, 1.25, 0.75, 1.25, -1, 4);

        gl.MatrixMode(OpenGL.GL_MODELVIEW);
        

    }

    private void GlControl_OnOpenGLDraw(object sender, OpenGLRoutedEventArgs args)
    {
    }

    // TODO most likely breaks when adding a camera
    private void GlControl_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
    {
        Point mousePositionP = e.GetPosition(GlControl);
        MousePosition = new Vector2((float)mousePositionP.X, (float)mousePositionP.Y);
        MousePosition.X = 2.0f * MousePosition.X / (float)ActualWidth;
        MousePosition.Y = 2.0f * ((float)((System.Windows.FrameworkElement)sender).ActualHeight - MousePosition.Y) / (float)((System.Windows.FrameworkElement)sender).ActualHeight;
    }
}