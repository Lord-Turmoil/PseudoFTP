using PseudoFTP.Client.Utils;

namespace PseudoFTP.Client.Test
{
    [TestClass]
    public class CompressTest
    {

        [TestMethod]
        public void WithoutIgnore()
        {
            ChangeDirectory();
            string path = CompressHelper.CompressFiles("Source");
            File.Copy(path, "archive.zip", true);
        }

        [TestMethod]
        public void WithIgnore()
        {
            ChangeDirectory();
            string path = CompressHelper.CompressFiles("Source", ".ftpignore");
            File.Copy(path, "archive.zip", true);
        }

        private void ChangeDirectory()
        {
            DirectoryInfo? directory = new DirectoryInfo(Directory.GetCurrentDirectory());
            while (directory != null && !directory.GetFiles("*.sln").Any())
            {
                directory = directory.Parent;
            }

            if (directory is null)
            {
                throw new DirectoryNotFoundException("Solution file not found.");
            }

            Directory.SetCurrentDirectory(Path.Join(directory.FullName, "Work"));
        }
    }
}