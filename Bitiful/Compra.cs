public class Compra
{
    public int Id { get; private set; }
    public DateOnly DataCompra { get; private set; }
    public string CnpjFornecedor { get; private set; }
    public decimal ValorTotal { get; private set; }


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
}