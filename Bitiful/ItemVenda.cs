using System.Globalization;

public class ItemVenda
{
    // Caminho da pasta e nome do arquivo usados para salvar os itens da venda.
    public const string path = @"C:/dev/Biltiful";
    public const string file = "ItemVenda.dat";
    private static string CaminhoArquivo => Path.Combine(path, file);

    // constantes de limite
    public const int TamanhoLinha = 32;
    public const int QuantidadeMaxima = 999;
    public const decimal ValorUnitarioMaximo = 999.99m;
    public const decimal TotalItemMaximo = 9999.99m;
    public const int MaxItensPorVenda = 3;

    // propriedades
    public uint IdVenda { get; }
    public string Produto { get; }
    public int Quantidade { get; }
    public decimal ValorUnitario { get; }
    public decimal TotalItem { get; }

    // Cria um item usando o Id da venda recebida. O produto e o preço vêm do cadastro,
    // e a quantidade vem da entrada do usuário; os dados ficam no objeto até SalvarItens gravá-los.
    public ItemVenda(Venda venda, string codigoProduto, int quantidade, decimal valorUnitario)
        : this(venda?.Id ?? throw new ArgumentNullException(nameof(venda)), codigoProduto, quantidade, valorUnitario)
    {
    }

    // Recria um item pelo Id da venda; recebe os campos do arquivo ou do chamador,
    // valida os valores, calcula TotalItem e guarda os dados nas propriedades do objeto.
    public ItemVenda(uint idVenda, string codigoProduto, int quantidade, decimal valorUnitario)
    {
        if (idVenda < 1 || idVenda > Venda.IdMaximo)
            throw new ArgumentOutOfRangeException(nameof(idVenda), $"O Id da venda deve estar entre 1 e {Venda.IdMaximo}.");

        if (string.IsNullOrWhiteSpace(codigoProduto) || codigoProduto.Length != 13 ||
            !codigoProduto.StartsWith("789", StringComparison.Ordinal) || !codigoProduto.All(char.IsDigit))
            throw new ArgumentException("O código de barras deve conter 13 dígitos e iniciar com 789.", nameof(codigoProduto));

        if (quantidade < 1 || quantidade > QuantidadeMaxima)
            throw new ArgumentOutOfRangeException(nameof(quantidade), $"A quantidade deve estar entre 1 e {QuantidadeMaxima}.");

        decimal valorArredondado = Math.Round(valorUnitario, 2);
        if (valorArredondado <= 0 || valorArredondado > ValorUnitarioMaximo)
            throw new ArgumentOutOfRangeException(nameof(valorUnitario), $"O valor unitário deve ser maior que zero e no máximo R$ {ValorUnitarioMaximo:N2}.");

        decimal total = Math.Round(quantidade * valorArredondado, 2);
        if (total <= 0 || total > TotalItemMaximo)
            throw new ArgumentOutOfRangeException(nameof(quantidade), $"O total do item não pode ultrapassar R$ {TotalItemMaximo:N2}.");

        IdVenda = idVenda;
        Produto = codigoProduto;
        Quantidade = quantidade;
        ValorUnitario = valorArredondado;
        TotalItem = total;
    }

    // Recebe os itens já carregados e o Id da venda; conta os itens dessa venda e
    // lança exceção ao atingir o limite, sem alterar a coleção.
    public static void ValidarLimitePorVenda(IEnumerable<ItemVenda> itensExistentes, uint idVenda)
    {
        ArgumentNullException.ThrowIfNull(itensExistentes);

        if (itensExistentes.Count(item => item.IdVenda == idVenda) >= MaxItensPorVenda)
            throw new InvalidOperationException($"A venda {idVenda:D5} já atingiu o limite de {MaxItensPorVenda} itens.");
    }

    // Converte as propriedades em uma linha fixa de 32 caracteres; o retorno é gravado em ItemVenda.dat.
    public string ToFileString()
    {
        string idStr = IdVenda.ToString("D5", CultureInfo.InvariantCulture);
        string produtoStr = Produto.PadRight(13, ' ');
        string quantidadeStr = Quantidade.ToString("D3", CultureInfo.InvariantCulture);
        string valorUnitarioStr = ((int)(ValorUnitario * 100m)).ToString("D5", CultureInfo.InvariantCulture);
        string totalStr = ((int)(TotalItem * 100m)).ToString("D6", CultureInfo.InvariantCulture);

        string linha = $"{idStr}{produtoStr}{quantidadeStr}{valorUnitarioStr}{totalStr}";
        if (linha.Length != TamanhoLinha)
            throw new InvalidOperationException($"Linha de ItemVenda inválida: esperado {TamanhoLinha} caracteres, gerado {linha.Length}.");

        return linha;
    }

    // Lê os registros de ItemVenda.path/ItemVenda.file e devolve os itens em uma lista;
    // se o arquivo não existir, devolve uma lista vazia.
    public static List<ItemVenda> CarregarTodos()
    {
        var itens = new List<ItemVenda>();
        if (!File.Exists(CaminhoArquivo))
            return itens;

        foreach (string linha in File.ReadAllLines(CaminhoArquivo))
        {
            if (!string.IsNullOrWhiteSpace(linha))
                itens.Add(FromFileString(linha));
        }

        return itens;
    }

    // Recebe o Id da venda, carrega ItemVenda.path/ItemVenda.file e devolve seus itens.
    public static List<ItemVenda> CarregarPorVenda(uint idVenda)
    {
        return CarregarTodos().Where(item => item.IdVenda == idVenda).ToList();
    }

    // Recebe os novos itens, valida o limite por venda e grava a lista em ItemVenda.path/ItemVenda.file.
    public static void SalvarItens(IEnumerable<ItemVenda> novosItens)
    {
        ArgumentNullException.ThrowIfNull(novosItens);

        var itensNovos = novosItens.ToList();
        if (itensNovos.Count == 0)
            return;

        var todosItens = CarregarTodos();
        foreach (ItemVenda? item in itensNovos)
        {
            ArgumentNullException.ThrowIfNull(item);
            ValidarLimitePorVenda(todosItens, item.IdVenda);
            todosItens.Add(item);
        }

        Directory.CreateDirectory(path);

        File.WriteAllLines(CaminhoArquivo, todosItens.Select(item => item.ToFileString()));
    }

    // Recebe o Id da venda, remove seus itens e grava os registros restantes em ItemVenda.path/ItemVenda.file.
    public static void ExcluirPorVenda(uint idVenda)
    {
        if (!File.Exists(CaminhoArquivo))
            return;

        var itensRestantes = CarregarTodos()
            .Where(item => item.IdVenda != idVenda)
            .Select(item => item.ToFileString());

        File.WriteAllLines(CaminhoArquivo, itensRestantes);
    }

    // Recebe uma linha de 32 caracteres lida de ItemVenda.dat, separa os campos
    // e devolve um objeto ItemVenda reconstruído em memória.
    public static ItemVenda FromFileString(string linha)
    {
        if (string.IsNullOrEmpty(linha) || linha.Length != TamanhoLinha)
            throw new FormatException($"Registro de ItemVenda inválido. Esperado {TamanhoLinha} caracteres, encontrado {linha?.Length ?? 0}.");

        uint idVenda = uint.Parse(linha.Substring(0, 5), CultureInfo.InvariantCulture);
        string produto = linha.Substring(5, 13).TrimEnd();
        int quantidade = int.Parse(linha.Substring(18, 3), CultureInfo.InvariantCulture);
        decimal valorUnitario = int.Parse(linha.Substring(21, 5), CultureInfo.InvariantCulture) / 100m;
        int totalItemSalvo = int.Parse(linha.Substring(26, 6), CultureInfo.InvariantCulture);

        var item = new ItemVenda(idVenda, produto, quantidade, valorUnitario);
        int totalItemCalculado = (int)(item.TotalItem * 100m);
        if (totalItemSalvo != totalItemCalculado)
            throw new FormatException("O total do item salvo não corresponde à quantidade e ao valor unitário.");

        return item;
    }

    // Devolve os dados do item formatados para exibição no console.
    public override string ToString()
    {
        return $"Id da Venda   : {IdVenda:D5}\n" +
               $"Produto       : {Produto}\n" +
               $"Quantidade    : {Quantidade}\n" +
               $"Valor Unitário: R$ {ValorUnitario:N2}\n" +
               $"Total do Item : R$ {TotalItem:N2}";
    }
}
