using System.Windows.Media;
using System.Windows.Media.Media3D;
namespace ERP.Helpers
{
    public static class MeshHelper
    {
        /// <summary>
        /// 创建一个立体柱子（长方体）
        /// </summary>
        /// <param name="x">柱子在 X 方向的位置</param>
        /// <param name="z">柱子在 Z 方向的位置</param>
        /// <param name="width">柱子的宽度</param>
        /// <param name="depth">柱子的深度</param>
        /// <param name="height">柱子的高度</param>
        public static MeshGeometry3D CreateBar(double x,  double height)
        {
            var mesh = new MeshGeometry3D();

            // 每个面单独定义 4 个顶点，共 6 个面 × 4 = 24 个顶点
            mesh.Positions = new Point3DCollection
    {
        // 底面 (Y=0)
        new Point3D(-1+x,        -1, -1),
        new Point3D(1+x,  -1, -1),
        new Point3D(1+x,  -1, 1),
        new Point3D(-1+x,  -1, 1),

        // 顶面 (Y=height)
        new Point3D(-1+x,        height, -1),
        new Point3D(1+x,  height, -1),
        new Point3D(1+x,  height, 1),
        new Point3D(-1+x,        height, 1),

        // 前面 (Z=z)
        new Point3D(-1+x,        -1, -1),
        new Point3D(1+x,  -1, -1),
        new Point3D(1+x,  height, -1),
        new Point3D(-1+x,        height, -1),

        // 后面 (Z=z+depth)
        new Point3D(-1+x,  -1, 1),
        new Point3D(1+x,        -1, 1),
        new Point3D(1+x,        height, 1),
        new Point3D(-1+x,  height, 1),

        // 左面 (X=x)
        new Point3D(-1+x,        -1, -1),
        new Point3D(-1+x,        -1, 1),
        new Point3D(-1+x,        height, 1),
        new Point3D(-1+x,        height, -1),

        // 右面 (X=x+width)
        new Point3D(1+x,  -1, -1),
        new Point3D(1+x,  -1, 1),
        new Point3D(1+x,  height, 1),
        new Point3D(1+x,  height, -1)
    };



            // 每个面两个三角形，共 12 个三角形
            mesh.TriangleIndices = new Int32Collection
        {
            // 底面
            0,2,1, 0,3,2,
            // 顶面
            4,6,5, 4,7,6,
            // 前面
            8,9,10, 8,10,11,
            // 后面
            12,13,14, 12,14,15,
            // 左面
            16,17,18, 16,18,19,
            // 右面
            20,21,22, 20,22,23
        };

            return mesh;

        }
    }
    }