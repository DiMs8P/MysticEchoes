using System.Drawing;
using System.Drawing.Imaging;
using MysticEchoes.Core.Loaders.Assets;
using SharpGL;

namespace MysticEchoes.Core.Loaders;

public class AssetManager
{
    private const string CoreAssetsFolder = "Assets\\";
    
    private OpenGL? _gl;
    private Dictionary<AssetType, string> _texturePaths;
    private Dictionary<AssetType, uint> _loadedTextures;
    
    public AssetManager(IDataLoader dataLoader)
    {
        _texturePaths = dataLoader.LoadTexturePaths();
        _loadedTextures = new Dictionary<AssetType, uint>();
    }
    
    public void InitializeGl(OpenGL gl)
    {
        _gl = gl;
    }

    public uint GetTexture(AssetType id)
    {
        if (_loadedTextures.TryGetValue(id, out uint texture))
        {
            return texture;
        }

        if (!_texturePaths.TryGetValue(id, out string texturePath))
        {
            throw new ArgumentException($"Texture with '{id}' is not found.");
        }

        texture = LoadTexture(Path.Combine(CoreAssetsFolder, texturePath));
        
        _loadedTextures.Add(id, texture);
        return texture;
    }
    
    private uint LoadTexture(string path)
    {
        if (_gl is null)
        {
            throw new ApplicationException("OpenGl is null");
        }
        
        Bitmap bitmap = new Bitmap(path);
        PixelFormat pixelFormat = bitmap.PixelFormat;
        bool isRgba = pixelFormat == PixelFormat.Format32bppArgb ||
                      pixelFormat == PixelFormat.Format32bppPArgb ||
                      pixelFormat == PixelFormat.Format64bppArgb ||
                      pixelFormat == PixelFormat.Format64bppPArgb;

        return isRgba ? 
            LoadTexture_Internal(bitmap, pixelFormat, 4, OpenGL.GL_BGRA) : 
            LoadTexture_Internal(bitmap, pixelFormat, 3, OpenGL.GL_BGR);
    }
    
    private uint LoadTexture_Internal(Bitmap bitmap, PixelFormat pixelFormat, uint internalFormat, uint format)
    {
        uint[] textureId = new uint[1];
        _gl.GenTextures(1, textureId);
        
        IntPtr pixels = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height),
            ImageLockMode.ReadOnly, pixelFormat).Scan0;
        
        if (pixels != IntPtr.Zero)
        {
            _gl.BindTexture(OpenGL.GL_TEXTURE_2D, textureId[0]);
            _gl.TexImage2D(OpenGL.GL_TEXTURE_2D, 0, internalFormat, bitmap.Width, bitmap.Height, 0, format,
                OpenGL.GL_UNSIGNED_BYTE, pixels);
            SetTextureParameters();
        }
        else
        {
            throw new ArgumentException("Can't load texture from given path");
        }
        
        bitmap.UnlockBits(new BitmapData());
        return textureId[0];
    }

    private void SetTextureParameters()
    {
        _gl.GenerateMipmapEXT(OpenGL.GL_TEXTURE_2D);

        _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
        _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);
        _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_LINEAR_MIPMAP_LINEAR);
        _gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_LINEAR);
    }
}