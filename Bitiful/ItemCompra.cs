public class ItemCompra
{
    private const decimal QuantidadeMaxima = 999.99m;
    private const decimal ValorUnitarioMaximo = 999.99m;
    private const decimal TotalItemMaximo = 9999.99m;

    private const string NomeArquivo = "ItemCompra.dat";
    private const int TamanhoLinha = 35;

    public uint Id { get; private set; }
    public DateOnly DataCompra { get; private set; }
    public string MateriaPrima { get; private set; }
    public decimal Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal TotalItem { get; private set; }

    // Cria um item ligado à compra informada.
    public ItemCompra(Compra compra, string materiaPrima,
        decimal quantidade, decimal valorUnitario)
    {
        if (compra == null)
            throw new ArgumentNullException(nameof(compra));

        Id = compra.Id;
        DataCompra = compra.DataCompra;

        MateriaPrima = ValidarMateriaPrima(materiaPrima);

        if (quantidade <= 0 || quantidade > QuantidadeMaxima)
            throw new ArgumentOutOfRangeException(nameof(quantidade));

        if (valorUnitario <= 0 || valorUnitario > ValorUnitarioMaximo)
            throw new ArgumentOutOfRangeException(nameof(valorUnitario));

        Quantidade = Math.Round(quantidade, 2);
        ValorUnitario = Math.Round(valorUnitario, 2);

        CalcularTotal();
    }

    // Reconstrói um item que foi carregado do arquivo.
    private ItemCompra(uint id, DateOnly dataCompra, string materiaPrima,
        decimal quantidade, decimal valorUnitario, decimal totalItem)
    {
        Id = id;
        DataCompra = dataCompra;
        MateriaPrima = ValidarFormatoMateriaPrima(materiaPrima);
        Quantidade = quantidade;
        ValorUnitario = valorUnitario;
        TotalItem = totalItem;
    }

    // Confere o formato MP0000.
    public static string ValidarMateriaPrima(string codigo)
    {
        return ValidarFormatoMateriaPrima(codigo);
    }

    private static string ValidarFormatoMateriaPrima(string codigo)
    {
        if (string.IsNullOrWhiteSpace(codigo))
            throw new ArgumentException("Informe a matéria-prima.");

        codigo = codigo.Trim().ToUpper();

        if (codigo.Length != 6 || codigo[0] != 'M' || codigo[1] != 'P')
            throw new ArgumentException("O código deve seguir o formato MP0000.");

        for (int i = 2; i < 6; i++)
        {
            if (!char.IsDigit(codigo[i]))
                throw new ArgumentException("Os quatro últimos caracteres devem ser números.");
        }

        return codigo;
    }

    // Calcula quantidade multiplicada pelo valor unitário.
    private void CalcularTotal()
    {
        TotalItem = Math.Round(Quantidade * ValorUnitario, 2);

        if (TotalItem <= 0 || TotalItem > TotalItemMaximo)
            throw new ArgumentOutOfRangeException(nameof(TotalItem));
    }

    // Transforma o item em uma linha de tamanho fixo.
    public string ToFileString()
    {
        string linha =
            Id.ToString().PadLeft(5, '0') +
            DataCompra.ToString("ddMMyyyy") +
            MateriaPrima +
            ((int)(Quantidade * 100)).ToString().PadLeft(5, '0') +
            ((int)(ValorUnitario * 100)).ToString().PadLeft(5, '0') +
            ((int)(TotalItem * 100)).ToString().PadLeft(6, '0');

        if (linha.Length != TamanhoLinha)
            throw new InvalidOperationException("Tamanho inválido do registro de item.");

        return linha;
    }

    // Transforma uma linha do arquivo em um objeto ItemCompra.
    private static ItemCompra FromFileString(string linha)
    {
        if (linha.Length != TamanhoLinha)
            throw new FormatException("Registro de item inválido.");

        uint id = uint.Parse(linha.Substring(0, 5));

        DateOnly data = DateOnly.ParseExact(
            linha.Substring(5, 8), "ddMMyyyy");

        string materiaPrima = linha.Substring(13, 6);

        decimal quantidade =
            int.Parse(linha.Substring(19, 5)) / 100m;

        decimal valorUnitario =
            int.Parse(linha.Substring(24, 5)) / 100m;

        decimal totalItem =
            int.Parse(linha.Substring(29, 6)) / 100m;

        return new ItemCompra(id, data, materiaPrima,
            quantidade, valorUnitario, totalItem);
    }

    // Salva os itens da compra no arquivo.
    internal static void Salvar(List<ItemCompra> itens)
    {
        string caminho = Compra.GarantirArquivo(NomeArquivo);

        foreach (ItemCompra item in itens)
        {
            File.AppendAllText(
                caminho,
                item.ToFileString() + Environment.NewLine);
        }
    }

    // Carrega todos os itens cadastrados.
    public static List<ItemCompra> CarregarTodos()
    {
        string caminho = Compra.GarantirArquivo(NomeArquivo);
        List<ItemCompra> itens = new();

        foreach (string linha in File.ReadAllLines(caminho))
        {
            if (!string.IsNullOrWhiteSpace(linha))
                itens.Add(FromFileString(linha));
        }

        return itens;
    }

    // Retorna os itens pertencentes a uma compra.
    public static List<ItemCompra> CarregarPorCompra(uint idCompra)
    {
        List<ItemCompra> todos = CarregarTodos();
        List<ItemCompra> encontrados = new();

        foreach (ItemCompra item in todos)
        {
            if (item.Id == idCompra)
                encontrados.Add(item);
        }

        return encontrados;
    }

    // Exclui todos os itens de uma compra.
    internal static void ExcluirPorCompra(uint idCompra)
    {
        string caminho = Compra.GarantirArquivo(NomeArquivo);
        List<string> linhas = new();

        foreach (string linha in File.ReadAllLines(caminho))
        {
            if (linha.Length == TamanhoLinha)
            {
                uint id = uint.Parse(linha.Substring(0, 5));

                if (id != idCompra)
                    linhas.Add(linha);
            }
        }

        File.WriteAllLines(caminho, linhas);
    }
}
