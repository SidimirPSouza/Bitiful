public class Compra
{
    public int Id { get; set; }
    public DateOnly DataCompra { get; set; }
    public string CnpjFornecedor { get; set; }
    public decimal ValorTotal { get; set; }

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