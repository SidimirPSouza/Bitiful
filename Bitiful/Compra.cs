public class Compra
{
    public int Id { get; set; }
    public DateOnly DataCompra { get; set; }
    public string CnpjFornecedor { get; set; }
    public decimal ValorTotal { get; set; }


    //Construtor que toda vez que um objeto é criado ele ja pega a data
    public Compra()
    {
        DataCompra = DateOnly.FromDateTime(DateTime.Today);
    }


    private List<ItemCompra> listaDeCompra = new();

    public bool AdicionarItem(ItemCompra itemCompra)
    {
        if (itemCompra == null || listaDeCompra.Count >= 3)
        {
            return false;
        }

        listaDeCompra.Add(itemCompra);

        return true;
    }

    public IReadOnlyList<ItemCompra> ObterItens()
    {
        return listaDeCompra.AsReadOnly();
    }

 
}