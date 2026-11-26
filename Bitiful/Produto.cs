public class Produto
{
    public string CodigoDeBarras { get; private set; }
    public string Nome { get; private set; }
    public decimal ValorVenda { get; private set; }
    public DateOnly UltimaVenda { get; private set; }
    public DateOnly DataCadastro { get; private set; }
    public char Situacao { get; private set; }

}

