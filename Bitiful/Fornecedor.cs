public class Fornecedor
{
    public string Cnpj { get; private set; }
    public string RazaoSocial { get; private set; }
    public DateOnly DataAbertura { get; private set; }
    public DateOnly DataUltimaCompra { get; private set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly DataCadastro { get; private set; } = DateOnly.FromDateTime(DateTime.Today);
    //public ESituacao Situacao { get; private set} = ativo; 

    public Fornecedor(string cnpj, string rs, DateOnly dataAbertura)
    {
        Cnpj = cnpj;
        RazaoSocial = validarTamanhoRS(rs);
        DataAbertura = ValidarDataAbertura(dataAbertura);
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
        return $"{Cnpj.PadRight(14,' ')}" +
            $"{RazaoSocial.PadRight(50,' ')}" +
            $"{DataAbertura}{DataUltimaCompra}" +
            $"{DataCadastro}";
    }
}