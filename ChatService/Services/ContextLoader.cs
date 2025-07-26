using System.Text;

namespace ChatService.Services
{
    public static class ContextLoader
    {
        public static string LoadCombinedContext()
        {
            string contextDir = Path.Combine(Directory.GetCurrentDirectory(), "Context");
            if (!Directory.Exists(contextDir)) return "";

            var files = Directory.GetFiles(contextDir, "*.txt");
            var sb = new StringBuilder();

            foreach (var file in files)
            {
                var fileName = Path.GetFileName(file);
                sb.AppendLine($"--- {fileName} ---");
                sb.AppendLine(File.ReadAllText(file));
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }

}
