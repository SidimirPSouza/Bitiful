public class ItemCompra
{
    public int Id { get; set; }
    public DateOnly DataCompra { get; set; }
    public string MateriaPrima { get; set; }
    public decimal Quantidade { get; set; }
    public decimal ValorUnitario { get; set; }
    public decimal TotalItem { get; set; }


    public decimal CalcularTotal()
    {
        return TotalItem = Quantidade * ValorUnitario;
    }    
}
