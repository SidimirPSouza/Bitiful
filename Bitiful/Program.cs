using System.Numerics;
using System.Runtime.Intrinsics.X86;


void MenuProducao()
{
    Producao produzindo = new Producao();
    ItemProducao _itemProducao = new ItemProducao();

    while (true)
    {
        Console.Clear();
        Console.WriteLine("""
        Digite a opção:
        1 - Cadastrar uma nova produção
        2 - Imprimir as produções
        3 - Localizar uma produção
        4 - Excluir uma produção
        5 - Vincular uma matéria-prima a uma produçao
        0 - Sair
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
                Thread.Sleep(3000);
                Console.Clear();
                break;
            case 2:
                produzindo.ImpressaoRegistro();
                break;
            case 3:
                Console.Write("Digite o ID da produção:");
                id = Console.ReadLine();
                id = id.PadLeft(5, '0');
                produzindo.Localizar(id);
                Console.ReadLine();
                Console.Clear();

                break;
            case 4:
                Console.Write("Digite o ID da produção:");
                id = Console.ReadLine();
                id = id.PadLeft(5, '0');
                produzindo.Excluir(id);
                Thread.Sleep(3000);
                Console.Clear();
                break;
            case 5:
                Console.Write("Digite o ID da produção: ");
                id = Console.ReadLine();
                id = id.PadLeft(5, '0');
                Console.Write("\nDigite o identificador da materia-prima utilizada(exemplo: MP0000): ");
                matPrima = Console.ReadLine();
                Console.Write("\nDigite a quantidade de materia prima: ");
                qtdMatPrima = Console.ReadLine();

                _itemProducao.ListarMateriaPrima(id, matPrima.ToUpper(), qtdMatPrima);
                Thread.Sleep(3000);
                Console.Clear();
                break;
            case 0:
                Environment.Exit(0);
                break;

    }
}