using System.IO;
using System.Reflection;

namespace Cipa.Application.Helpers
{
    public static class FileSystemHelpers
    {
        /// <summary>
        /// Verifica se já existe um arquivo com o nome <param name="filename"></param> 
        /// no caminho <param name="path"></param>. Se exisitir,
        /// vai adicionando um indíce ao final do nome do
        /// arquivo, até chegar a um nome único, e retorna o caminho do arquivo
        /// concatenado com o nome
        /// </summary>
        /// <param name="path"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static string GetRelativeFileName(string path, string filename)
        {
            int count = 1;
            string file = Path.Combine(path, filename);
            string fileName = Path.GetFileNameWithoutExtension(file);
            string extension = Path.GetExtension(file);
            while (File.Exists(GetAbsolutePath(file)))
            {
                string tempFileName = string.Format("{0} ({1}){2}", fileName, count++, extension.ToString());
                file = Path.Combine(path, tempFileName);
            }
            return file;
        }

        public static string GetAbsolutePath(string relativePath)
        {
            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), relativePath);
            return path;
        }
    }
}