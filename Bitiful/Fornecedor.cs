public class Fornecedor
{
    public string Cnpj { get; private set; }
    public string RazaoSocial { get; private set; }
    public DateOnly DataAbertura { get; private set; }
    public DateOnly DataUltimaCompra { get; private set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly DataCadastro { get; private set; } = DateOnly.FromDateTime(DateTime.Today);

    //public ESituacao Situacao { get; private set} = ativo; 

    // CONTRUTOR PARA CADASTRAR PELA PRIMEIRA VEZ O FORNECEDOR
    public Fornecedor(string cnpj, string rs, DateOnly dataAbertura)
    {
        Cnpj = cnpj;
        RazaoSocial = validarTamanhoRS(rs);
        DataAbertura = ValidarDataAbertura(dataAbertura);
    }

    //CONSTRUTOR PARA PEGAR O FORNECEDOR DO ARQUIVO
    public Fornecedor(string cnpj, string rs, DateOnly dataAbertura, DateOnly dataUltimaCompra, DateOnly dataCadastro)
    {
        Cnpj = cnpj;
        RazaoSocial = validarTamanhoRS(rs);
        DataAbertura = ValidarDataAbertura(dataAbertura);
        DataUltimaCompra = dataUltimaCompra;
        DataCadastro = dataCadastro;
    }

    private string validarTamanhoRS(string razaoSocial)
    {
        return razaoSocial.Length > 50 ?
        throw new ArgumentException("Razao social tem mais que 50 caracteres!") :
        razaoSocial;
    }

    private DateOnly ValidarDataAbertura(DateOnly dataAbertura)
    {
        return dataAbertura < DateOnly.FromDateTime(DateTime.Today) ?
            dataAbertura :
            throw new ArgumentException("A data de abertura nao pode ser hoje ou futura");
    }

    public void AlterarDataAbertura(DateOnly novaData)
    {
        DataAbertura = ValidarDataAbertura(novaData);
    }

    public void AlterarRazaoSocial(string razaoSocial)
    {
        RazaoSocial = validarTamanhoRS(razaoSocial);
    }

    //PEGAR FORNECEDOR DO ARQUIVO e ENVIAR PARA UMA LISTA
    public static List<Fornecedor> CarregarFornecedores(string CaminhoArquivo)
    {
        var fornecedores = new List<Fornecedor>();
        try
        {
            if (Utils.ValidarCaminhoArquivo(CaminhoArquivo))
                foreach (var fornecedor in File.ReadAllLines(CaminhoArquivo))
                {
                    //pegando os dados do arquivo
                    string cnpj = fornecedor.Substring(0, 14).Trim();
                    string razaoSocial = fornecedor.Substring(14, 50).Trim();
                    DateOnly dataAbertura = DateOnly.ParseExact(fornecedor.Substring(64, 10), "dd/MM/yyyy");
                    DateOnly dataUltimaCompra = DateOnly.ParseExact(fornecedor.Substring(74, 10), "dd/MM/yyyy");
                    DateOnly dataCadastro = DateOnly.ParseExact(fornecedor.Substring(84, 10), "dd/MM/yyyy");
                    
                    //criando um objeto Fornecedor com os dados do arquivo
                    var novoFornecedor = new Fornecedor(cnpj, razaoSocial, dataAbertura, dataUltimaCompra, dataCadastro);
                    //adicionando na lista o novo objeto
                    fornecedores.Add(novoFornecedor);

                }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocorreu um erro ao carregar os fornecedores: {e.Message}");
        }
        //retorando a lista de fornecedores do arquivo
        return fornecedores;
    }

    //SALVAR FORNECEDOR DENTRO DO ARQUIVO
    public static void SalvarFornecedores(List<Fornecedor> fornecedores, string CaminhoArquivo)
    {
        try
        {
            var diretorio = Path.GetDirectoryName(CaminhoArquivo);

            if (!Directory.Exists(diretorio))
                Directory.CreateDirectory(diretorio);

            var sw = new StreamWriter(CaminhoArquivo);

            //passando pela lista de fornecedores e escrevendo em cada linha no arquivo
            foreach (var fornecedor in fornecedores)
            {
                sw.WriteLine(fornecedor.ToFileString());
            }
            sw.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ao salvar o fornecedor houve um erro: {e.Message}");
        }
    }


    public override string ToString()
    {
        return $"Cnpj: {Cnpj.ToString()}" +
            $"\nRazao Social: {RazaoSocial}" +
            $"\nData Abertura: {DataAbertura}" +
            $"\nData Ultima Compra: {DataUltimaCompra}" +
            $"\nData Cadastro: {DataCadastro}";
    }

    public string ToFileString()
    {
        return $"{Cnpj.PadRight(14, ' ')}" +
            $"{RazaoSocial.PadRight(50, ' ')}" +
            $"{DataAbertura:dd/MM/yyyy}" +
            $"{DataUltimaCompra:dd/MM/yyyy}" +
            $"{DataCadastro:dd/MM/yyyy}";
    }
}