using System.Collections.Generic;
using System.Linq;

namespace LadyIDE.WPF.CanvasElements.Help
{
    public static class TypeOptionProvider
    {
        private static readonly string[] PrimitiveTypes =
        {
            "String",
            "Int",
            "Double",
            "Bool",
            "Char",
            "Object"
        };

        public static List<TypeOption> RequiredTypes(
            IEnumerable<CanvasItemBase> allItems,
            bool primitive = false,
            bool record = false,
            bool entity = false,
            bool enumElement = false,
            bool proxy = false,
            bool action = false,
            CanvasItemBase? exclude = null)
        {
            List<TypeOption> result = new();

            if (primitive)
                foreach (string type in PrimitiveTypes)
                    result.Add(new TypeOption(type, true));

            IEnumerable<CanvasItemBase> items = allItems
                .Where(item => item != exclude);

            if (entity)
                foreach (Entity item in items.OfType<Entity>())
                    result.Add(new TypeOption(item));

            if (record)
                foreach (Record item in items.OfType<Record>())
                    result.Add(new TypeOption(item));

            if (enumElement)
                foreach (EnumElement item in items.OfType<EnumElement>())
                    result.Add(new TypeOption(item));

            if (proxy)
                foreach (Proxy item in items.OfType<Proxy>())
                    result.Add(new TypeOption(item));

            if (action)
            {
                foreach (Action actionItem in allItems.OfType<Action>())
                {
                    result.Add(new TypeOption(actionItem));
                }
            }

            return result;
        }
    }
}