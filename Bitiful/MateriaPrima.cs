public class MateriaPrima
{
    private int Id { get; set; }
    public string Nome { get; set; }
    public DateOnly UltimaCompra { get; set; }
    private DateOnly DataCadastro { get; set; }
    public char Situacao { get; set; }

    public MateriaPrima(string nome)
    {
        try
        {
            Id = GerarNovoId();
            Nome = nome;
            UltimaCompra = DateOnly.FromDateTime(DateTime.Now);
            DataCadastro = DateOnly.FromDateTime(DateTime.Now);
            Situacao = 'A';

            string idString = ConverterIdFormatoString(Id);
            string nomeString = ConverterNomeFormatoString(Nome);

            string linhaParaSalvar = idString + nomeString + DataCadastro + UltimaCompra + Situacao;
            Save(linhaParaSalvar);
        }
        catch (Exception e) 
        {
            Console.WriteLine(e.Message);            
        }            
        
    }// ao criar a materia prima, ela ja vai gerar o caminho e o arquivo caso não exista, gerar o proximo id e salvar no arquivo automaticamente.
    public MateriaPrima(int id, string nome, DateOnly ultimaCompra, DateOnly dataCadastro, char situacao)
    {
        Id = id;
        Nome = nome;
        UltimaCompra = ultimaCompra;
        DataCadastro = dataCadastro;
        Situacao = situacao;
    }//construtor apenas para criar um objeto que recebe os atributos resgatados do arquivo(para salvar em uma lista quando necessario)
    private int GerarNovoId()
    {
        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"MateriaPrima.dat";

        if (Utils.ValidarCaminhoArquivo(arquivo))
        {
            
            int ultimoId = 0;

            string[] linhas = File.ReadAllLines(caminho + arquivo);

            if (linhas.Length > 0)
            {
                string ultimaLinha = linhas[linhas.Length - 1];
                ultimoId = int.Parse(ultimaLinha.Substring(2, 4));
                return ultimoId + 1;
            }
            else
                return 1;

        }
        else
            return 1;

    }
    private void Save(string s)
    {
        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"MateriaPrima.dat";
        File.AppendAllText(caminho + arquivo, s + Environment.NewLine);
    }
    private string ConverterIdFormatoString(int id)
    {
        switch (id)
        {
            case < 10:
                return $"MP000{id}";
            case < 100:
                return $"MP00{id}";
            case < 1000:
                return $"MP0{id}";
            case < 10000:
                return $"MP{id}";
            default:
                return $"MP0000{id}";
        }

    }
    private string ConverterNomeFormatoString(string nome)
    {
        string nomeLimitado20;
        if (nome.Length > 20)
            nomeLimitado20 = nome.Substring(0, 20);
        else
            nomeLimitado20 = nome;

        string nomeFormatado = "";
        for(int i = 0; i < 20 - nomeLimitado20.Length; i++)
        {
            nomeFormatado = nomeFormatado + "*";
        }
        nomeFormatado = nomeFormatado + nomeLimitado20;
        return nomeFormatado;       

    }


}


