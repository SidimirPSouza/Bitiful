Producao produzindo = new Producao();

while(true){
    Console.WriteLine("""
        Digite a opção:
        1 - cadastrar uma nova produção
        2 - imprimir todas as produções
        3 - localizar uma producão
        4 - excluir uma produção
        0 - sair
        """);

    int opcao = int.Parse(Console.ReadLine());

    switch (opcao)
    {
        case 1:
            Console.Write("Digite o código de barras(13 digitos): ");
            string barcode = Console.ReadLine();
            Console.Write("\nDigite a quantidade: ");
            string qtd = Console.ReadLine();
            produzindo.Cadastrar(barcode, qtd);
            break;
        case 2:
            produzindo.ImpressaoRegistro();
            break;
        case 3:
            Console.Write("digite o id da produção:");
            string id = Console.ReadLine();
            var impressao = new Producao();
            id = id.PadLeft(5, '0');
            Console.WriteLine("Testando:" + id);
            impressao = impressao.Localizar(id);
            Console.WriteLine($"ID:{int.Parse(impressao.Id)}\tData de produção:{impressao.DataProducao}\tProduto:{impressao.Produto}\tQuantidade: {decimal.Parse(impressao.Quantidade)}");
            break;
        case 4:
            Console.Write("digite o id da produção:");
            string _id = Console.ReadLine();
            _id = _id.PadLeft(5, '0');
            produzindo.Excluir(_id);
            break;
        case 0:
            Environment.Exit(0);
            break;

    }
}