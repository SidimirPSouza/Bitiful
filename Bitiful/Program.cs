try
{
    ItemCompra item = new ItemCompra(-5, 20);

    Console.WriteLine($"Quantidade: {item.Quantidade}");
    Console.WriteLine($"Valor unitário: {item.ValorUnitario:C}");
    Console.WriteLine($"Total: {item.TotalItem:C}");
}
catch (ArgumentOutOfRangeException ex)
{
    Console.WriteLine($"Erro nos dados: {ex.Message}");
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"Erro na operação: {ex.Message}");
}

Compra compra = new Compra("12345678000199");

ItemCompra item1 = new ItemCompra(2, 10);
ItemCompra item2 = new ItemCompra(3, 15);
ItemCompra item3 = new ItemCompra(1, 30);

// Adicionando os itens.
Console.WriteLine(compra.AdicionarItem(item1));
Console.WriteLine(compra.AdicionarItem(item2));
Console.WriteLine(compra.AdicionarItem(item3));

// Tentativa de adicionar o quarto item.
ItemCompra item4 = new ItemCompra(1, 50);

Console.WriteLine(compra.AdicionarItem(item4));

// Finalizando a compra.
if (compra.FinalizarCompra())
{
    Console.WriteLine("Compra finalizada com sucesso!");
    compra.ListarItens();
}
else
{
    Console.WriteLine("Não foi possível finalizar a compra");
}