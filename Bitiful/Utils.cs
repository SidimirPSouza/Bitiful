using System.Text.RegularExpressions;

internal static class Utils
{
    internal static bool ValidarCaminhoArquivo(string arquivo)
    {
        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        if (!Directory.Exists(caminho))
            Directory.CreateDirectory(caminho);

        var caminhoArquivo = caminho + arquivo;
        if (string.IsNullOrWhiteSpace(caminhoArquivo))
        {
            Console.WriteLine("O caminho do arquivo não deve estar vazio");
            return false;
        }
        else
        {
            if(!Directory.Exists(caminhoArquivo))
            {
                File.Create(caminhoArquivo);
            }
        }
        return true;
    }

    internal static bool ValidarCPF(string cpf)
    {
        var cpfFormatado = Regex.Replace(cpf, "[^0-9]", "");

        if (cpfFormatado.Count() == 11)
        {
            var soma = 0;
            int mult = 10;
            for (int i = 0; i < 9; i++, mult--)
            {
                int num = cpfFormatado[i] - '0';
                soma = soma + (num * mult);
            }
            soma = (soma * 10) % 11;
            soma = soma == 10 ? 0 : soma;

            if (cpfFormatado[9] == soma.ToString()[0])
            {
                soma = 0;
                mult = 11;
                for (int i = 0; i < 10; i++, mult--)
                {
                    int num = cpfFormatado[i] - '0';
                    soma = soma + (num * mult);
                }
                soma = (soma * 10) % 11;
                soma = soma == 10 ? 0 : soma;
                if (cpfFormatado[10] == soma.ToString()[0])
                {
                    return true;
                }
            }
            return false;
        }

        return true;
    }
}

