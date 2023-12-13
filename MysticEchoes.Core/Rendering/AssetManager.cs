using System.Drawing;
using System.Drawing.Imaging;
using MysticEchoes.Core.Loaders;
using SharpGL;

namespace MysticEchoes.Core.Rendering;

public class AssetManager
{
    private const string CoreAssetsFolder = "Assets\\";
    
    private OpenGL _gl;
    private Dictionary<string, string> _texturePaths;
    private Dictionary<string, uint> _loadedTextures;
    
    public AssetManager(IDataLoader dataLoader)
    {
        _texturePaths = dataLoader.LoadTexturePaths();
        _loadedTextures = new Dictionary<string, uint>();
    }
    
    public void InitializeGl(OpenGL gl)
    {
        _gl = gl;
    }

    public uint LoadTexture(string id)
    {
        if (_loadedTextures.TryGetValue(id, out uint texture))
        {
            return texture;
        }

        if (!_texturePaths.TryGetValue(id, out string texturePath))
        {
            throw new ArgumentException($"Texture with '{id}' is not found.");
        }

        texture = LoadTexture_Internal(Path.Combine(CoreAssetsFolder, texturePath));
        
        _loadedTextures.Add(id, texture);
        return texture;
    }
    
    private uint LoadTexture_Internal(string path)
    {
        uint loadedTexture = 0;
        Bitmap bitmap = new Bitmap(path);

        switch (bitmap.PixelFormat)
        {
            case PixelFormat.Format32bppArgb:
            case PixelFormat.Format32bppPArgb:
            case PixelFormat.Format64bppArgb:
            case PixelFormat.Format64bppPArgb:
                loadedTexture = LoadRgbaTexture(bitmap);
                break;
            
            default:
                loadedTexture = LoadRgbTexture(bitmap);
                break;
        }

        return loadedTexture;
    }

    private uint LoadRgbTexture(Bitmap bitmap)
    {
        uint[] textureId = new uint[1];
        _gl.GenTextures(1, textureId);
        
        IntPtr pixels = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format24bppRgb).Scan0;
        
        if (pixels != IntPtr.Zero)
        {
            _gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId[0]);
            _gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, 3, bitmap.Width, bitmap.Height, 0, OpenGL.GL_BGR,
                OpenGL.GL_UNSIGNED_BYTE, pixels);
            _gl.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);

            _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
            _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
            _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
        }
        else
        {
            throw new ArgumentException("Can't load texture from given path");
        }
        
        bitmap.UnlockBits(new BitmapData());
        return textureId[0];
    }

    private uint LoadRgbaTexture(Bitmap bitmap)
    {
        uint[] textureId = new uint[1];
        _gl.GenTextures(1, textureId);
        
        IntPtr pixels = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb).Scan0;
        
        if (pixels != IntPtr.Zero)
        {
            _gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId[0]);
            _gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, 4, bitmap.Width, bitmap.Height, 0, OpenGL.GL_BGRA,
                OpenGL.GL_UNSIGNED_BYTE, pixels);
            _gl.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);

            _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
            _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
            _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
        }
        else
        {
            throw new ArgumentException("Can't load texture from given path");
        }
        
        bitmap.UnlockBits(new BitmapData());
        return textureId[0];
    }
}