using System.Collections.Generic;

namespace Development.Shared.Runtime.Scripts
{
    public static class Addresses
    {
        private const string SpherePrefix = "Sphere_";
        private const string PlasticPrefix = "Plastic_";
        private const string MetalPrefix = "Metal_";

        public const string SphereRedPlastic = SpherePrefix + PlasticPrefix + "Red";
        public const string SphereGreenPlastic = SpherePrefix + PlasticPrefix + "Green";
        public const string SphereBluePlastic = SpherePrefix + PlasticPrefix + "Blue";
        public const string SphereYellowPlastic = SpherePrefix + PlasticPrefix + "Yellow";
        public const string SphereCyanPlastic = SpherePrefix + PlasticPrefix + "Cyan";

        public const string SphereRedMetal = SpherePrefix + MetalPrefix + "Red";
        public const string SphereGreenMetal = SpherePrefix + MetalPrefix + "Green";
        public const string SphereBlueMetal = SpherePrefix + MetalPrefix + "Blue";
        public const string SphereYellowMetal = SpherePrefix + MetalPrefix + "Yellow";
        public const string SphereCyanMetal = SpherePrefix + MetalPrefix + "Cyan";

        public static IEnumerable<string> EnumerateSpherePlastic()
        {
            yield return SphereRedPlastic;
            yield return SphereGreenPlastic;
            yield return SphereBluePlastic;
            yield return SphereYellowPlastic;
            yield return SphereCyanPlastic;
        }

        public static IEnumerable<string> EnumerateSphereMetal()
        {
            yield return SphereRedMetal;
            yield return SphereGreenMetal;
            yield return SphereBlueMetal;
            yield return SphereYellowMetal;
            yield return SphereCyanMetal;
        }

        public static IEnumerable<string> EnumerateSphere()
        {
            foreach (var value in EnumerateSpherePlastic())
                yield return value;

            foreach (var value in EnumerateSphereMetal())
                yield return value;
        }

        public static IEnumerable<string> EnumerateAll()
        {
            foreach (var value in EnumerateSphere())
                yield return value;
        }
    }
}
