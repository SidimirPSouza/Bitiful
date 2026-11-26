public class ItemCompra
{
    public int Id { get; private set; }
    public DateOnly DataCompra { get; private set; }
    public string MateriaPrima { get; private set; }
    public decimal Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal TotalItem { get; private set; }


    public ItemCompra(decimal quantidade,decimal valorUnit)
    {
        if (quantidade > 0 && quantidade < 1000)
        {
            Quantidade = quantidade;
        }
        else
        {
            Console.WriteLine("Valor invalido!!");
        }
    }


    public decimal CalcularTotal()
        {
            return TotalItem = Quantidade * ValorUnitario;
        }    
}
