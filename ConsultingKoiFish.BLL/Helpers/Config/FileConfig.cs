namespace ConsultingKoiFish.BLL.Helpers.Config;

public class FileConfig
{
    public static string PathCombine(params string[] paths)
    {
        return Path.Combine(paths.Where(x => !string.IsNullOrEmpty(x)).ToArray());
    }


    public static string UrlCombine(params string[] paths)
    {
        return String.Join("/", paths.Where(x => !string.IsNullOrEmpty(x)));
    }
}