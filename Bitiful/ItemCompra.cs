public class ItemCompra
{
    public int Id { get; private set; }
    public DateOnly DataCompra { get; private set; }
    public string MateriaPrima { get; private set; }
    public decimal Quantidade { get; private set; }
    public decimal ValorUnitario { get; private set; }
    public decimal TotalItem { get; private set; }

    public ItemCompra(decimal quantidade, decimal valorUnit)
    {
        // Verifica se a quantidade é maior que zero e não ultrapassa 999,99.
        // Também verifica se possui no máximo duas casas decimais.
        if (quantidade <= 0 || quantidade > 999.99m ||
            quantidade != decimal.Round(quantidade, 2))
        {
            throw new ArgumentOutOfRangeException(
                nameof(quantidade),
                "A quantidade deve ser maior que zero, até 999,99"
            );
        }

        // Verifica se o valor unitário respeita os limites estabelecidos.
        if (valorUnit <= 0 || valorUnit > 999.99m ||
            valorUnit != decimal.Round(valorUnit, 2))
        {
            throw new ArgumentOutOfRangeException(
                nameof(valorUnit),
                "O valor unitário deve ser maior que zero, até 999,99"
            );
        }

        Quantidade = quantidade;
        ValorUnitario = valorUnit;

        // Calcula o total e verifica seu limite.
        CalcularTotal();
    }

    public decimal CalcularTotal()
    {
        decimal total = Quantidade * ValorUnitario;

        if (total > 9999.99m)
        {
            throw new InvalidOperationException(
                "O total do item não pode ultrapassar R$ 9.999,99."
            );
        }

        TotalItem = total;

        return TotalItem;
    }
}
