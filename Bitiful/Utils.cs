using System.Text.RegularExpressions;

internal static class Utils
{
    internal static bool ValidarCaminhoArquivo(string arquivo)
    {
        try
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
                if (!File.Exists(caminhoArquivo))
                {
                    using (File.Create(caminhoArquivo))
                    {

                    }

                }
            }
            return true;
        }catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return false;
        }
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
                    return true;
            }
        }
        return false;
    }

    //internal static bool ValidaCPFCadastrado(List<Cliente> clientes, string cpf)
    //{

    //    Cliente? cliente = clientes.Find(c => c.Cpf == cpf);

    //    if (cliente is null) // se o cliente for nulo
    //    {
    //        return true; //retorna true se o cliente não está cadastrado
    //    }
    //    else
    //        return false; //retorna false se ele já está cadastrado
    //}



    internal static bool ValidarCNPJ(string cnpj)
    {
        //cnpj = Regex.Replace(cnpj, "[^a-zA-Z]", "");
        cnpj = Regex.Replace(cnpj, "[^0-9]", "");

        if (cnpj.Length == 14)
        {
            List<int> pesos = new List<int> { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;

            for (int i = 0; i < pesos.Count; i++)
            {
                int num = cnpj[i] - '0';
                soma = soma + (num * pesos[i]);
            }
            soma = soma % 11;
            soma = soma < 2 ? 0 : 11 - soma;
            if (cnpj[12] == soma.ToString()[0])
            {
                var newPesos = pesos.Prepend(6).ToList();
                soma = 0;
                for (int i = 0; i < newPesos.Count; i++)
                {
                    int num = cnpj[i] - '0';
                    soma = soma + (num * newPesos[i]);
                }
                soma = soma % 11;
                soma = soma < 2 ? 0 : 11 - soma;
                if (cnpj[13] == soma.ToString()[0])
                    return true;
            }
        }
        return false;
    }

    internal static ESituacao ValidaSituacao(ESituacao situacao, char status)
    {
        if (status == 'A')
            return situacao = (ESituacao)status;
        else
            return situacao = (ESituacao)'I';
    }

    //internal static bool ValidarData(string cpf)

    internal static char CreateMenu(List<string> options)
    {
        foreach (string item in options)
            Console.Write(item);
        try
        {
            Console.Write("Informe a opção desejada: ");
            return char.TryParse(Console.ReadLine(), out char option) ? option : '\0';
        }
        catch
        {
            return '\0';
        }
    }

    internal static void ShowInformations<T>(List<T> models)
    {
        if (models.Count.Equals(0))
        {
            Console.WriteLine("Nenhum contato encontrado!");
            Console.ReadKey();
            return;
        }

        int index = 0;
        bool showing = true;

        while (showing)
        {
            var options = new List<string> { "1 - Primeiro ", "2 - Anterior ", "3 - Próximo ", "4 - Último ", "0 - Sair\n" };

            Console.Clear();
            Console.WriteLine($"Item {index + 1} de {models.Count}");
            Console.WriteLine(models[index] + "\n");

            switch (Utils.CreateMenu(options))
            {
                case '1':
                    index = 0;
                    break;

                case '2':
                    if (index > 0) index--;
                    break;

                case '3':
                    if (index < models.Count - 1) index++;
                    break;

                case '4':
                    index = models.Count - 1;
                    break;

                case '0':
                    showing = false;
                    break;

                default:
                    Console.WriteLine("Opção inválida!");
                    break;
            }
        }
    }
    //internal ProcurarNaLista
}

