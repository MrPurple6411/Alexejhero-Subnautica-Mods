using System.Reflection;

namespace AlexejheroYTB.Common
{
    public class PathHelper
    {
        public static string GetDLLPath() => Assembly.GetCallingAssembly().Location;

        public string Path;

        public PathHelper(string path) => Path = path;
        public PathHelper Append(string pathToAppend)
        {
            PathHelper newPathHelper = new PathHelper(Path);
            newPathHelper.Path = System.IO.Path.Combine(newPathHelper.Path, pathToAppend);
            return newPathHelper;
        }

        public static PathHelper operator +(PathHelper currentPath, string pathToAppend) => currentPath.Append(pathToAppend);
        public static implicit operator PathHelper(string s) => new PathHelper(s);
        public static implicit operator string(PathHelper p) => p.Path;
    }
}
