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

    //metodos para utilizar externamente independentes

    //override ToString()

}

