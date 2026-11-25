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
            if(!File.Exists(caminhoArquivo))
            {
                using (File.Create(caminhoArquivo)){}
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
        }
        return false;
    }



    //private string ValidarCNPJ(string cnpj)

    //{
    //    cnpj = LimparCnpj(cnpj);

    //    if (cnpj.Length == 14)
    //    {
    //        int[] pesos = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
    //        int soma = 0;
    //        int resto = 0;
    //        int primeiroDigito = 0;
    //        int segundoDigito = 0;

    //        for (int i = 0; i < pesos.Length; i++)

    //        {
    //            soma += int.Parse(cnpj[i].ToString()) * pesos[i];
    //        }

    //        resto = soma % 11;

    //        if (resto < 2)

    //        {
    //            primeiroDigito = 0;
    //        }

    //        else
    //        {

    //        }
    //    }
    //}

    //internal static bool ValidarData(string cpf)
}

