public class Compra
{
    public int Id { get; set; }
    public DateOnly DataCompra { get; set; }
    public string CnpjFornecedor { get; set; }
    public decimal ValorTotal { get; set; }

    private List<ItemCompra> listaDeCompra = new();
    
    public bool AdicionarItem(ItemCompra itemCompra)
    {
        if(listaDeCompra.Count < 3)
        {
            listaDeCompra.Add(itemCompra);

            foreach (var item in listaDeCompra)
            {
                item.CalcularTotal();
            }
            return true;
        }
        else
        {
            return false;
        }
    }
}
