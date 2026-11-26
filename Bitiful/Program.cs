using System.Runtime.Intrinsics.X86;

Producao produzindo = new Producao();
ItemProducao _itemProducao = new ItemProducao();

while(true){
    Console.WriteLine("""
        Digite a opção:
        1 - cadastrar uma nova produção
        2 - imprimir todas as produções
        3 - localizar uma producão
        4 - excluir uma produção
        5 - cadastrar uma materia prima na produção
        0 - sair
        """);

    int opcao = int.Parse(Console.ReadLine());

    string barcode = string.Empty;
    string qtd = string.Empty;
    string matPrima = string.Empty;
    string qtdMatPrima = string.Empty;
    string id = string.Empty;

    switch (opcao)
    {

        case 1:
            Console.Write("Digite o código de barras(13 digitos): ");
            barcode = Console.ReadLine();
            Console.Write("\nDigite a quantidade: ");
            qtd = Console.ReadLine();


            produzindo.Cadastrar(barcode, qtd);
            break;
        case 2:
            produzindo.ImpressaoRegistro();
            break;
        case 3:
            Console.Write("digite o id da produção:");
            id = Console.ReadLine();
            id = id.PadLeft(5, '0');
            produzindo.Localizar(id);
            
            break;
        case 4:
            Console.Write("digite o id da produção:");
            id = Console.ReadLine();
            id = id.PadLeft(5, '0');
            produzindo.Excluir(id);
            break;
        case 5:
            Console.Write("Digite o ID da produção");
            id = Console.ReadLine();
            id = id.PadLeft(5, '0');
            Console.Write("\nDigite o identificador da materia-prima utilizada(exemplo: MP0000): ");
            matPrima = Console.ReadLine();
            Console.Write("\nDigite a quantidade de materia prima: ");
            qtdMatPrima = Console.ReadLine();

            _itemProducao.ListarMateriaPrima(id, matPrima.ToUpper(), qtdMatPrima);
            break;
        case 0:
            Environment.Exit(0);
            break;

    }
}
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