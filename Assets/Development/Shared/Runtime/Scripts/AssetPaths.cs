using System.Collections.Generic;

namespace Development.Shared.Runtime.Scripts
{
    public sealed class AssetPaths
    {
        private const string PrefabExtension = ".prefab";
        
        public const string BaseFolder = "Assets/Development/Shared/Runtime";
        public const string PrefabsFolder = BaseFolder + "/Prefabs";

        public const string SphereRedPlastic = PrefabsFolder + "/" + Addresses.SphereRedPlastic + PrefabExtension;
        public const string SphereGreenPlastic = PrefabsFolder + "/" + Addresses.SphereGreenPlastic + PrefabExtension;
        public const string SphereBluePlastic = PrefabsFolder + "/" + Addresses.SphereBluePlastic + PrefabExtension;
        public const string SphereYellowPlastic = PrefabsFolder + "/" + Addresses.SphereYellowPlastic + PrefabExtension;
        public const string SphereCyanPlastic = PrefabsFolder + "/" + Addresses.SphereCyanPlastic + PrefabExtension;

        public const string SphereRedMetal = PrefabsFolder + "/" + Addresses.SphereRedMetal + PrefabExtension;
        public const string SphereGreenMetal = PrefabsFolder + "/" + Addresses.SphereGreenMetal + PrefabExtension;
        public const string SphereBlueMetal = PrefabsFolder + "/" + Addresses.SphereBlueMetal + PrefabExtension;
        public const string SphereYellowMetal = PrefabsFolder + "/" + Addresses.SphereYellowMetal + PrefabExtension;
        public const string SphereCyanMetal = PrefabsFolder + "/" + Addresses.SphereCyanMetal + PrefabExtension;

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
