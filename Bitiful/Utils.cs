using System.Text.RegularExpressions;

internal static class Utils
{
    internal static bool ValidarCaminhoArquivo(string caminho)
    {
        if (string.IsNullOrWhiteSpace(caminho))
        {
            Console.WriteLine("O caminho do arquivo não deve estar vazio");
            return false;
        }
        else
        {
            return File.Exists(caminho);
        }
    }
}

