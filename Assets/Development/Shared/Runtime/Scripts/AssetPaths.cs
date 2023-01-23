using System.Collections.Generic;

namespace Development.Shared.Runtime.Scripts
{
    public sealed class AssetPaths
    {
        public const string BaseFolder = "Assets/Development/Shared/Runtime";
        public const string PrefabsFolder = BaseFolder + "/Prefabs";

        public const string SphereRedPlastic = PrefabsFolder + "/" + Addresses.SphereRedPlastic + ".prefab";
        public const string SphereGreenPlastic = PrefabsFolder + "/" + Addresses.SphereGreenPlastic + ".prefab";
        public const string SphereBluePlastic = PrefabsFolder + "/" + Addresses.SphereBluePlastic + ".prefab";
        public const string SphereYellowPlastic = PrefabsFolder + "/" + Addresses.SphereYellowPlastic + ".prefab";
        public const string SphereCyanPlastic = PrefabsFolder + "/" + Addresses.SphereCyanPlastic + ".prefab";

        public const string SphereRedMetal = PrefabsFolder + "/" + Addresses.SphereRedMetal + ".prefab";
        public const string SphereGreenMetal = PrefabsFolder + "/" + Addresses.SphereGreenMetal + ".prefab";
        public const string SphereBlueMetal = PrefabsFolder + "/" + Addresses.SphereBlueMetal + ".prefab";
        public const string SphereYellowMetal = PrefabsFolder + "/" + Addresses.SphereYellowMetal + ".prefab";
        public const string SphereCyanMetal = PrefabsFolder + "/" + Addresses.SphereCyanMetal + ".prefab";

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
