

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Sunctum.Domain.Logic.DisplayType
{
    public abstract class DisplayType
    {
        public static readonly IDisplayType SideBySide = new DisplayTypeSideBySide();
        public static readonly IDisplayType Details = new DisplayTypeDetails();

        public static IDisplayType GetReferenceByName(string propertyName)
        {
            var displayTypes = typeof(DisplayType).GetFields(BindingFlags.Static | BindingFlags.Public);
            return displayTypes.Single(x => x.Name == propertyName).GetValue(typeof(IDisplayType)) as IDisplayType;
        }

        public static string GetPropertyName(IDisplayType displayType)
        {
            var name = displayType.GetType().Name;

            var fields = typeof(DisplayType).GetFields(BindingFlags.Static | BindingFlags.Public);
            return fields.Single(x => ("DisplayType" + x.Name) == name).Name;
        }
    }
}
