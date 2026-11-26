using static System.Runtime.InteropServices.JavaScript.JSType;

public class Produto
{
    public string CodigoDeBarras { get; private set; }
    public string Nome { get; private set; }
    public decimal ValorVenda { get; private set; }
    public DateOnly UltimaVenda { get; private set; }
    public DateOnly DataCadastro { get; private set; }
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

    //metodos para utilizar externamente independentes

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

