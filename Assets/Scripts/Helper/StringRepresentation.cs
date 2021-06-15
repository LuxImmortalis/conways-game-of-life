using Plotter;

namespace Helper
{
    public static class StringRepresentation
    {
        public static string Get(object translatable)
        {
            switch (translatable)
            {
                case TypePlotter.TypeCubePlotter:
                    return "Plot Cubes";
                case TypePlotter.TypeTexturePlotter:
                    return "Plot to Texture";
                case TypePlotter.TypeQuadPlotter:
                    return "Plot Quads";
                default:
                    return "";
            }
        }
    }
}