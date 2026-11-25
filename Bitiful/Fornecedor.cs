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

    public static List<Fornecedor> CarregarFornecedor(string filePath)
    {
        var fornecedores = new List<Fornecedor>();
        try
        {
            if (Utils.ValidarCaminhoArquivo(filePath))
                foreach (var fornecedor in File.ReadAllLines(filePath))
                {
                    string cnpj = fornecedor.Substring(0, 14).Trim();
                    string razaoSocial = fornecedor.Substring(14, 50).Trim();
                    DateOnly dataAbertura = DateOnly.ParseExact(fornecedor.Substring(64, 10), "dd/MM/yyyy");
                    DateOnly dataUltimaCompra = DateOnly.ParseExact(fornecedor.Substring(74, 10), "dd/MM/yyyy");
                    DateOnly dataCadastro = DateOnly.ParseExact(fornecedor.Substring(84, 10), "dd/MM/yyyy");

                    var novoFornecedor = new Fornecedor(cnpj, razaoSocial, dataAbertura, dataUltimaCompra, dataCadastro);
                    fornecedores.Add(novoFornecedor);

                }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocorreu um erro ao carregar os fornecedores: {e.Message}");
        }
        return fornecedores;
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