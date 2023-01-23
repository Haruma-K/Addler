using System.Collections.Generic;

namespace Development.Shared.Runtime.Scripts
{
    public static class LabelNames
    {
        public const string Plastic = "plastic";
        public const string Metal = "metal";

        public static IEnumerable<string> EnumerateAll()
        {
            yield return Plastic;
            yield return Metal;
        }
    }
}
