public class Compra
{
    public int Id { get; private set; }
    public DateOnly DataCompra { get; private set; }
    public string CnpjFornecedor { get; private set; }
    public decimal ValorTotal { get; private set; }

    // Lista privada que armazena os itens da compra.
    private List<ItemCompra> listaDeCompra = new();

    public Compra(string cnpjFornecedor)
    {
        if (string.IsNullOrWhiteSpace(cnpjFornecedor))
        {
            throw new ArgumentException("O CNPJ do fornecedor é obrigatório.");
        }

        CnpjFornecedor = cnpjFornecedor;
        DataCompra = DateOnly.FromDateTime(DateTime.Today);
    }

    // Adiciona um item somente se ele respeitar as regras da compra.
    public bool AdicionarItem(ItemCompra itemCompra)
    {
        if (itemCompra == null)
        {
            return false;
        }

        // Impede adicionar mais de três itens.
        if (listaDeCompra.Count >= 3)
        {
            return false;
        }

        // Calcula quanto ficará o total caso o item seja adicionado.
        decimal novoTotal = ValorTotal + itemCompra.TotalItem;

        // Impede ultrapassar o limite de R$ 99.999,99.
        if (novoTotal > 99999.99m)
        {
            return false;
        }

        // Só adiciona depois de validar todas as condições.
        listaDeCompra.Add(itemCompra);

        // Atualiza o valor total da compra.
        ValorTotal = novoTotal;

        return true;
    }

    // Calcula o valor total de todos os itens.
    public decimal CalcularValorTotal()
    {
        decimal total = 0;

        foreach (ItemCompra item in listaDeCompra)
        {
            total += item.TotalItem;
        }

        if (total > 99999.99m)
        {
            throw new InvalidOperationException(
                "O valor total da compra não pode ultrapassar R$ 99.999,99."
            );
        }

        ValorTotal = total;

        return ValorTotal;
    }

    // Verifica se a compra atende às regras para ser finalizada.
    public bool FinalizarCompra()
    {
        // A compra precisa ter pelo menos um item.
        if (listaDeCompra.Count < 1)
        {
            return false;
        }

        // A compra não pode possuir mais de três itens.
        if (listaDeCompra.Count > 3)
        {
            return false;
        }

        // Recalcula o total para garantir que esteja atualizado.
        CalcularValorTotal();

        // Confirma que o valor total está dentro do limite.
        if (ValorTotal <= 0 || ValorTotal > 99999.99m)
        {
            return false;
        }

        return true;
    }

    // Exibe todos os itens cadastrados na compra.
    public void ListarItens()
    {
        if (listaDeCompra.Count == 0)
        {
            Console.WriteLine("A compra ainda não possui itens.");
            return;
        }

        foreach (ItemCompra item in listaDeCompra)
        {
            Console.WriteLine("----------------------------");
            Console.WriteLine($"Matéria-prima: {item.MateriaPrima}");
            Console.WriteLine($"Quantidade: {item.Quantidade}");
            Console.WriteLine($"Valor unitário: {item.ValorUnitario:C}");
            Console.WriteLine($"Total do item: {item.TotalItem:C}");
        }

        Console.WriteLine("----------------------------");
        Console.WriteLine($"Valor total da compra: {ValorTotal:C}");
    }

    // Retorna a quantidade de itens cadastrados.
    public int QuantidadeDeItens()
    {

        return listaDeCompra.Count;
    }
}
