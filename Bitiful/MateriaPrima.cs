public class MateriaPrima
{
    public int Id { get; private set; }
    public string Nome { get; set; }
    public DateOnly UltimaCompra { get; set; }
    public DateOnly DataCadastro { get; private set; }
    public char Situacao { get; set; }

    //construtores
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
            SalvarStringNoArquivo(linhaParaSalvar);
        }
        catch (Exception e) 
        {
            Console.WriteLine(e.Message);            
        }            
        
    }// ao criar a materia prima, ela ja vai gerar o caminho e o arquivo caso não exista, gerar o proximo id e salvar no arquivo automaticamente.
    private MateriaPrima(int id, string nome, DateOnly dataCadastro, DateOnly ultimaCompra, char situacao)
    {
        Id = id;
        Nome = nome;
        DataCadastro = dataCadastro;
        UltimaCompra = ultimaCompra;
        Situacao = situacao;
    }//construtor apenas para criar um objeto que recebe os atributos resgatados do arquivo(para salvar em uma lista quando necessario)
    
    //metodos utilizados internamente
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
    private void SalvarStringNoArquivo(string s)
    {
        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"MateriaPrima.dat";
        File.AppendAllText(caminho + arquivo, s + Environment.NewLine);
    }
    private static void SalvarObjetoNoArquivo(MateriaPrima obj)
    {
        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"MateriaPrima.dat";
        File.AppendAllText(caminho + arquivo, TransformarObjEmLinhaDoArquivo(obj) + Environment.NewLine);
    }
    private static void SalvarListaNoArquivo(List<MateriaPrima> lista)
    {
        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"MateriaPrima.dat";
        using (var writer = new StreamWriter(caminho+arquivo)){}
        foreach (var item in lista)
        {
            SalvarObjetoNoArquivo(item);
        }
    }
    private static string ConverterIdFormatoString(int id)
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
    private static string ConverterNomeFormatoString(string nome)
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
    private static MateriaPrima TransformarLinhaDoArquivoEmObjeto(string linhaAtual)
    {
        int id = int.Parse(linhaAtual.Substring(2, 4));
        int contAux = 0;

        for (int j = 0; j < 20; j++)
        {
            if (linhaAtual[j + 6] == '*')
                contAux++;
            else
                break;
        }
        string nome = linhaAtual.Substring(6 + contAux, 20 - contAux);
        DateOnly dataCadastro = DateOnly.Parse(linhaAtual.Substring(26, 10));
        DateOnly UltimaCompra = DateOnly.Parse(linhaAtual.Substring(36, 10));
        char situacao = linhaAtual[linhaAtual.Length - 1];

        return new(id, nome, dataCadastro, UltimaCompra, situacao);

    }//nesse metodo eu não faço nenhuma validação, pois ele é privado e ele somente é chamado por outor metodo que a principio ja fez a devida validação
    private static string TransformarObjEmLinhaDoArquivo(MateriaPrima obj)
    {
        return $"{ConverterIdFormatoString(obj.Id)}{ConverterNomeFormatoString(obj.Nome)}{obj.DataCadastro}{obj.UltimaCompra}{obj.Situacao}";
    }

    //metodos para utilizar externamente independentes
    public static List<MateriaPrima> buscarTodosRegistros()
    {
        List<MateriaPrima> lista = new();

        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"MateriaPrima.dat";
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
                return new List<MateriaPrima>();

            return lista;


        }
        else
            return new List<MateriaPrima>();
    }
    public static List<MateriaPrima> buscarRegistrosAtivos()
    {
        List<MateriaPrima> lista = new();

        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"MateriaPrima.dat";
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
                return new List<MateriaPrima>();

            return lista;


        }
        else
            return new List<MateriaPrima>();

    }
    public static List<MateriaPrima> buscarRegistrosInativos()
    {
        List<MateriaPrima> lista = new();

        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"MateriaPrima.dat";
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
                return new List<MateriaPrima>();

            return lista;


        }
        else
            return new List<MateriaPrima>();

    }
    public static List<MateriaPrima> LocalizarPorChaveInt(int idInt)
    {
        List<MateriaPrima> lista = new();

        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"MateriaPrima.dat";
        if (Utils.ValidarCaminhoArquivo(arquivo))
        {
            string[] linhas = File.ReadAllLines(caminho + arquivo);

            if (linhas.Length > 0)
            {
                foreach (string linhaAtual in linhas)
                {
                    if (idInt == int.Parse(linhaAtual.Substring(2, 4)))
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
    public static List<MateriaPrima> LocalizarPorChaveString(string IdString)
    {
        List<MateriaPrima> lista = new();

        var caminho = Environment.GetEnvironmentVariable("USERPROFILE") + $@"\Bitiful\";
        var arquivo = $"MateriaPrima.dat";
        if (Utils.ValidarCaminhoArquivo(arquivo))
        {
            string[] linhas = File.ReadAllLines(caminho + arquivo);

            if (linhas.Length > 0)
            {
                foreach (string linhaAtual in linhas)
                {
                    IdString = IdString.ToUpper();
                    string idFormat = linhaAtual.Substring(0, 6);

                    if (idFormat == IdString)
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
    public static bool EditarNomePorId(int id, string nome)
    {
        bool editou = false;

        List<MateriaPrima> lista = buscarTodosRegistros();

        foreach(MateriaPrima item in lista)
        {
            if(item.Id == id)
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
    public static bool EditarUltimaCompraPorId(int id, DateOnly dataUltimaCompra)
    {
        bool editou = false;

        List<MateriaPrima> lista = buscarTodosRegistros();

        foreach (MateriaPrima item in lista)
        {
            if (item.Id == id)
            {
                item.UltimaCompra = dataUltimaCompra;
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
    public static void InativarItem(int id)
    {
        bool editou = false;

        List<MateriaPrima> lista = buscarTodosRegistros();

        foreach (MateriaPrima item in lista)
        {
            if (item.Id == id)
            {
                item.Situacao = 'I';
                editou = true;
                break;
            }
        }

        if (editou)
            SalvarListaNoArquivo(lista);

    }
    public static void AtivarItem(int id)
    {
        bool editou = false;

        List<MateriaPrima> lista = buscarTodosRegistros();

        foreach (MateriaPrima item in lista)
        {
            if (item.Id == id)
            {
                item.Situacao = 'A';
                editou = true;
                break;
            }
        }

        if (editou)
            SalvarListaNoArquivo(lista);

    }

    //override ToString()
    public override string ToString()
    {
        return $"""

            Id: {ConverterIdFormatoString(Id)}
            Nome: {Nome}
            UltimaCompra: {UltimaCompra}
            DataCadastro: {DataCadastro}
            Situacao: {Situacao}

            """;
    }
}


