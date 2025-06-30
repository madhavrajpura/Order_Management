using Microsoft.AspNetCore.Http;
namespace BusinessLogicLayer.Helper;

public class ImageTemplate
{
    public static string GetFileName(IFormFile file, string folderPath)
    {
        if (file == null || file.Length == 0)
        {
            return null;
        }
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);

        string fileName = file.FileName;
        string filePath = Path.Combine(folderPath, fileName);

        using (FileStream? stream = new FileStream(filePath, FileMode.Create))
        {
            file.CopyTo(stream);
        }

        return fileName;
    }
}
