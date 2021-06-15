using System.Collections.Generic;
using Helper;
using UnityEngine.UI;

namespace Plotter
{
    public static class PlotterTypes
    {
        public static List<Dropdown.OptionData> GetOptionList()
        {
            List<Dropdown.OptionData> optionList = new List<Dropdown.OptionData>();

            optionList.Add(new Dropdown.OptionData(StringRepresentation.Get(TypePlotter.TypeCubePlotter)));
            optionList.Add(new Dropdown.OptionData(StringRepresentation.Get(TypePlotter.TypeQuadPlotter)));
            optionList.Add(new Dropdown.OptionData(StringRepresentation.Get(TypePlotter.TypeTexturePlotter)));

            return optionList;
        }

        public static TypePlotter? GetTypeFromIndex(int typePlotterIndex)
        {
            switch (typePlotterIndex)
            {
                case 0:
                    return TypePlotter.TypeCubePlotter;
                case 1:
                    return TypePlotter.TypeQuadPlotter;
                case 2:
                    return TypePlotter.TypeTexturePlotter;
                default:
                    return null;
            }
        }

        public static int? GetOptionIndex(AbstractPlotter typePlotter)
        {
            switch (typePlotter)
            {
                case CubePlotter cubePlotter:
                    return 0;
                case QuadPlotter quadPlotter:
                    return 1;
                case TexturePlotter texturePlotter:
                    return 2;
                default:
                    return null;
            }
        }
    }
}