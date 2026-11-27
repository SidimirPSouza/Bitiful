using static System.Runtime.InteropServices.JavaScript.JSType;

public class Produto
{
    public string CodigoDeBarras { get; private set; }
    public string Nome { get; private set; }
    public decimal ValorVenda { get; private set; }
    public DateOnly DataCadastro { get; private set; }
    public DateOnly UltimaVenda { get; private set; }
    public char Situacao { get; private set; }

    //construtores
    public Produto(string nome, decimal valorVenda)
    {
        CodigoDeBarras = GerarCodigoDeBarrasAleatorio();
        Nome = nome;
        ValorVenda = valorVenda;
        DataCadastro = DateOnly.FromDateTime(DateTime.Now);
        UltimaVenda = DataCadastro;
        Situacao = 'A';

        string valorVendaString = ConverterValorVendaFormatoString(ValorVenda);

        string nomeString = ConverterNomeFormatoString(Nome);

        string linhaParaSalvar = CodigoDeBarras + nomeString + valorVendaString + DataCadastro + UltimaVenda + Situacao;

        SalvarStringNoArquivo(linhaParaSalvar);

    }
    private Produto(string codigoDeBarras, string nome, decimal valorVenda, DateOnly dataCadastro, DateOnly ultimaVenda, char situacao)
    {
        CodigoDeBarras = codigoDeBarras;
        Nome = nome;
        ValorVenda = valorVenda;
        DataCadastro = dataCadastro;
        UltimaVenda = ultimaVenda;
        Situacao = situacao;
    }

    //metodos utilizados internamente
    private static string GerarCodigoDeBarrasAleatorio()
    {
        Random random = new Random();
        string codigo = "789";
        for(int i = 0; i < 10; i++)
        {
            codigo += random.Next(1,10);
        }
        return codigo;
    }
    private void SalvarStringNoArquivo(string s)
    {
        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"Produto.dat";

        if (Utils.ValidarCaminhoArquivo(arquivo))
        {
            File.AppendAllText(caminho + arquivo, s + Environment.NewLine);
        }
    }
    private static void SalvarObjetoNoArquivo(Produto obj)
    {
        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"Produto.dat";
        File.AppendAllText(caminho + arquivo, TransformarObjEmLinhaDoArquivo(obj) + Environment.NewLine);
    }
    private static void SalvarListaNoArquivo(List<Produto> lista)
    {
        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"Produto.dat";
        using (var writer = new StreamWriter(caminho + arquivo)) { }
        foreach (var item in lista)
        {
            SalvarObjetoNoArquivo(item);
        }
    }
    private static string ConverterNomeFormatoString(string nome)
    {
        string nomeLimitado20;
        if (nome.Length > 20)
            nomeLimitado20 = nome.Substring(0, 20);
        else
            nomeLimitado20 = nome;

        string nomeFormatado = "";
        for (int i = 0; i < 20 - nomeLimitado20.Length; i++)
        {
            nomeFormatado = nomeFormatado + "*";
        }
        nomeFormatado = nomeFormatado + nomeLimitado20;
        return nomeFormatado;

    }
    private static string ConverterValorVendaFormatoString(decimal valorVenda)
    {
        bool temVirgula = false;
        string valorVendaString = valorVenda.ToString();
        for (int i = 0; i < valorVendaString.Length; i++)
        {
            if (valorVendaString[i] == ',')
            {
                temVirgula = true;
                break;
            }
        }
        if(temVirgula == false)
            valorVendaString += ",00";
        string[] valorVendaSplit = valorVendaString.Split(',');
        string antes = valorVendaSplit[0];
        string depois = valorVendaSplit[1];

        switch (antes.Length)
        {
            case 1:
                antes = $"00{antes}";
                break;
            case 2:
                antes = $"0{antes}";
                break;
            case 3:
                antes = $"{antes}";
                break;
        }

        switch (depois.Length)
        {
            case 1:
                depois = $"{depois}0";
                break;
            case 2:
                depois = $"{depois}";
                break;
        }

        return $"{antes},{depois}";
    }
    private static Produto TransformarLinhaDoArquivoEmObjeto(string linhaAtual)
    {
        string codigoDeBarras = linhaAtual.Substring(0, 13);
        int contAux = 0;

        for (int j = 0; j < 20; j++)
        {
            if (linhaAtual[j + 13] == '*')
                contAux++;
            else
                break;
        }
        string nome = linhaAtual.Substring(13 + contAux, 20 - contAux);
        decimal valorVenda = decimal.Parse(linhaAtual.Substring(33, 6));
        DateOnly dataCadastro = DateOnly.Parse(linhaAtual.Substring(39, 10));
        DateOnly ultimaVenda = DateOnly.Parse(linhaAtual.Substring(49, 10));
        char situacao = linhaAtual[linhaAtual.Length - 1];

        return new(codigoDeBarras, nome, valorVenda, dataCadastro, ultimaVenda, situacao);
    }
    private static string TransformarObjEmLinhaDoArquivo(Produto obj)
    {
        return $"{obj.CodigoDeBarras}{ConverterNomeFormatoString(obj.Nome)}{ConverterValorVendaFormatoString(obj.ValorVenda)}{obj.DataCadastro}{obj.UltimaVenda}{obj.Situacao}";
    }

    //metodos para utilizar externamente independentes
    public static List<Produto> buscarTodosRegistros()
    {
        List<Produto> lista = new();

        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"Produto.dat";
        if (Utils.ValidarCaminhoArquivo(arquivo))
        {
            string[] linhas = File.ReadAllLines(caminho + arquivo);

            if (linhas.Length > 0)
            {
                foreach (string linhaAtual in linhas)
                {
                    lista.Add(TransformarLinhaDoArquivoEmObjeto(linhaAtual));
                }

            }
            else
                return new List<Produto>();

            return lista;


        }
        else
            return new List<Produto>();
    }
    public static List<Produto> buscarRegistrosAtivos()
    {
        List<Produto> lista = new();

        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"Produto.dat";
        if (Utils.ValidarCaminhoArquivo(arquivo))
        {
            string[] linhas = File.ReadAllLines(caminho + arquivo);

            if (linhas.Length > 0)
            {
                foreach (string linhaAtual in linhas)
                {
                    char situacao = linhaAtual[linhaAtual.Length - 1];
                    if (situacao == 'A')
                    {
                        lista.Add(TransformarLinhaDoArquivoEmObjeto(linhaAtual));
                    }

                }

            }
            else
                return new List<Produto>();

            return lista;


        }
        else
            return new List<Produto>();

    }
    public static List<Produto> buscarRegistrosInativos()
    {
        List<Produto> lista = new();

        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"Produto.dat";
        if (Utils.ValidarCaminhoArquivo(arquivo))
        {
            string[] linhas = File.ReadAllLines(caminho + arquivo);

            if (linhas.Length > 0)
            {
                foreach (string linhaAtual in linhas)
                {
                    char situacao = linhaAtual[linhaAtual.Length - 1];
                    if (situacao == 'I')
                    {
                        lista.Add(TransformarLinhaDoArquivoEmObjeto(linhaAtual));
                    }
                }

            }
            else
                return new List<Produto>();

            return lista;


        }
        else
            return new List<Produto>();

    }
    public static List<Produto> LocalizarPorCodigoDeBarras(string codigoDeBarrasString)
    {
        List<Produto> lista = new();

        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"Produto.dat";
        if (Utils.ValidarCaminhoArquivo(arquivo))
        {
            string[] linhas = File.ReadAllLines(caminho + arquivo);

            if (linhas.Length > 0)
            {
                foreach (string linhaAtual in linhas)
                {
                    codigoDeBarrasString = codigoDeBarrasString.ToUpper();
                    string idFormat = linhaAtual.Substring(0, 13);

                    if (idFormat == codigoDeBarrasString)
                    {
                        lista.Add(TransformarLinhaDoArquivoEmObjeto(linhaAtual));
                        break;
                    }

                }

            }
            else
                return lista;

            return lista;


        }
        else
            return lista;


    }
    public static bool EditarNomePorCodigoDeBarras(string codigoDeBarras, string nome)
    {
        bool editou = false;

        List<Produto> lista = buscarTodosRegistros();

        foreach (Produto item in lista)
        {
            if (item.CodigoDeBarras == codigoDeBarras)
            {
                item.Nome = nome;
                editou = true;
                break;
            }
        }

        if (editou)
        {
            SalvarListaNoArquivo(lista);
            return true;
        }
        else
            return false;

    }

    //override ToString()
    public override string ToString()
    {
        return $"""

            Id: {CodigoDeBarras}
            Nome: {Nome}
            Valor de venda: R${ValorVenda}
            Ultima venda: {UltimaVenda}
            Data de cadastro: {DataCadastro}
            Situacao: {Situacao}

            """;
    }

}

