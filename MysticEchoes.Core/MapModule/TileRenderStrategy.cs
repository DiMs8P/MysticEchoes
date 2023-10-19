// using MazeGeneration;
// using MysticEchoes.Core.Base;
// using MysticEchoes.Core.Rendering;
// using SharpGL;
//
// namespace MysticEchoes.Core.MapModule;
//
// public class TileRenderStrategy : RenderStrategy
// {
//     private readonly Entity _entity;
//
//     public TileRenderStrategy(Entity entity)
//     {
//         _entity = entity;
//     }
//
//     public override void DoRender()
//     {
//         var tile = _entity.GetComponent<TileComponent>();
//         var rect = tile.Rect;
//
//         GL.Begin(OpenGL.GL_TRIANGLE_FAN);
//
//         var color = tile.Type switch
//         {
//             CellType.Empty => new[] { 0.1d, 0.1d, 0.1d },
//             CellType.FragmentBound => new[] { 0.1d, 0.1d, 0.1d },
//             CellType.Hall => new[] { 0.8d, 0.8d, 0.1d },
//             CellType.Wall => new[] { 0.1d, 0.1d, 0.8d },
//             _ => throw new ArgumentOutOfRangeException()
//         };
//
//         GL.Color(color);
//         GL.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y);
//         GL.Vertex(rect.LeftBottom.X, rect.LeftBottom.Y + rect.Size.Height);
//         GL.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y + rect.Size.Height);
//         GL.Vertex(rect.LeftBottom.X + rect.Size.Width, rect.LeftBottom.Y);
//         GL.End();
//
//     }
// }