

public class Venda
{
    // Caminho da pasta e nome do arquivo usados para salvar os registros de venda.
    public static readonly string path = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Bitiful\";
    public const string file = "Venda.dat";
    private static string CaminhoArquivo => Utils.ValidarCaminhoArquivo(file)
        ?? throw new IOException($"Não foi possível validar o caminho do arquivo {file}.");

    // constantes de limite
    public const int TamanhoLinha = 31;
    public const uint IdMaximo = 99999;
    public const decimal ValorTotalMaximo = 99999.99m;

    // incrementador de id
    private static uint _proximoId = 1;

    // propriedades
    public uint Id { get; }
    public DateOnly DataVenda { get; }
    public string Cliente { get; private set; }
    public decimal ValorTotal { get; private set; }

    // Função fornecida pelo módulo Cliente para localizar os dados pelo CPF.
    // A classe Cliente pode atribuir aqui seu método de busca antes de iniciar o menu.
    public static Func<string, (string Nome, DateOnly DataNascimento, char Situacao)?>? ConsultarCliente { get; set; }

    // Função fornecida pelo módulo Produto para localizar nome, preço e situação pelo código de barras.
    // A classe Produto pode atribuir aqui seu método de busca antes de iniciar o menu.
    public static Func<string, (string Nome, decimal ValorVenda, char Situacao)?>? ConsultarProduto { get; set; }

    // Função fornecida pelo módulo Produto para listar todos os registros cadastrados.
    public static Func<IEnumerable<(string CodigoBarras, string Nome, decimal ValorVenda, DateOnly UltimaVenda, DateOnly DataCadastro, char Situacao)>>? ListarProdutos { get; set; }

    // Atualiza o Id a partir dos registros existentes antes de criar uma venda.
    private static uint GerarProximoId()
    {
        if (_proximoId > IdMaximo)
            throw new InvalidOperationException($"Limite máximo de {IdMaximo} vendas atingido.");

        return _proximoId++;
    }

    // Recebe o CPF do cliente, valida e cria a venda com novo Id e data atual.
    public Venda(string cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente) || cliente.Length != 11 || !Utils.ValidarCPF(cliente))
            throw new ArgumentException("O CPF deve conter 11 dígitos válidos.", nameof(cliente));

        _ = CarregarTodas();
        Id = GerarProximoId();
        DataVenda = DateOnly.FromDateTime(DateTime.Now);
        Cliente = cliente;
        ValorTotal = 0m;
    }

    // Reconstrói uma venda que veio do arquivo, mantendo Id e data originais;
    // valida o CPF e o valor total antes de guardar os campos no objeto.
    private Venda(uint id, DateOnly dataVenda, string cliente, decimal valorTotal)
    {
        if (string.IsNullOrWhiteSpace(cliente) || cliente.Length != 11 || !Utils.ValidarCPF(cliente))
            throw new ArgumentException("O CPF deve conter 11 dígitos válidos.", nameof(cliente));

        Id = id;
        DataVenda = dataVenda;
        Cliente = cliente;
        ValorTotal = ValidarValorTotal(valorTotal);
    }

    // Recebe um valor em reais, arredonda para centavos, valida os limites e o devolve formatado.
    private static decimal ValidarValorTotal(decimal valorTotal)
    {
        decimal arredondado = Math.Round(valorTotal, 2);
        if (arredondado <= 0 || arredondado > ValorTotalMaximo)
            throw new ArgumentOutOfRangeException(nameof(valorTotal), $"O ValorTotal (R$ {arredondado:N2}) deve ser maior que zero e no máximo R$ {ValorTotalMaximo:N2}.");

        return arredondado;
    }

    // Recebe os itens desta venda, valida a quantidade e os Ids, soma os totais e atualiza ValorTotal.
    public void DefinirValorTotal(IEnumerable<ItemVenda> itens)
    {
        ArgumentNullException.ThrowIfNull(itens);
        var itensDaVenda = itens.ToList();

        if (itensDaVenda.Count < 1 || itensDaVenda.Count > ItemVenda.MaxItensPorVenda)
            throw new ArgumentException($"A venda deve possuir de 1 a {ItemVenda.MaxItensPorVenda} itens.", nameof(itens));

        if (itensDaVenda.Any(item => item is null || item.IdVenda != Id))
            throw new ArgumentException("Todos os itens devem pertencer a esta venda.", nameof(itens));

        ValorTotal = ValidarValorTotal(itensDaVenda.Sum(item => item.TotalItem));
    }

    // Recebe os dados do cliente; valida situação, inadimplência, maioridade e confirmação.
    // Se aprovado, sincroniza o Id com Venda.path/Venda.file e devolve a venda em memória.
    public static Venda? CadastrarVenda(
        string cpfCliente,
        string nomeCliente,
        DateOnly dataNascimento,
        char situacao,
        bool clienteInadimplente)
    {
        if (string.IsNullOrWhiteSpace(cpfCliente) || cpfCliente.Length != 11 || !Utils.ValidarCPF(cpfCliente))
            throw new ArgumentException("O CPF deve conter 11 dígitos válidos.", nameof(cpfCliente));

        if (situacao != (char)ESituacao.Ativo)
        {
            Console.WriteLine("Cliente inativo. Venda não permitida.");
            return null;
        }

        if (clienteInadimplente)
        {
            Console.WriteLine("Cliente consta em Risco.dat. Venda não permitida.");
            return null;
        }

        if (!MaiorIdade(dataNascimento, DateOnly.FromDateTime(DateTime.Now)))
        {
            Console.WriteLine("Cliente menor de 18 anos. Venda não permitida.");
            return null;
        }

        Console.WriteLine($"Cliente: {nomeCliente} | Nascimento: {dataNascimento:dd/MM/yyyy}");
        Console.Write("Confirmar cliente? (S/N): ");
        if (!string.Equals(LerEntrada()?.Trim(), "S", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Cadastro cancelado.");
            return null;
        }

        return new Venda(cpfCliente);
    }

    // Converte a venda em uma linha de 31 caracteres para gravação em Venda.dat;
    // exige que o total já tenha sido definido.
    public string ToFileString()
    {
        if (ValorTotal <= 0)
            throw new InvalidOperationException("A venda só pode ser gravada após definir o ValorTotal.");

        string idStr = Id.ToString().PadLeft(5, '0');
        string dataStr = DataVenda.ToString("ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
        string cpfStr = Cliente.PadLeft(11, '0');
        long valorCentavos = (long)Math.Round(ValorTotal * 100m);
        string valorStr = valorCentavos.ToString().PadLeft(7, '0');

        string linha = $"{idStr}{dataStr}{cpfStr}{valorStr}";

        if (linha.Length != TamanhoLinha)
            throw new InvalidOperationException($"Linha de Venda inválida: esperado {TamanhoLinha} caracteres, gerado {linha.Length}.");

        return linha;
    }

    // Lê os registros de Venda.path/Venda.file e devolve as vendas em uma lista;
    // devolve lista vazia se o arquivo ainda não existir.
    public static List<Venda> CarregarTodas()
    {
        var vendas = new List<Venda>();
        if (!File.Exists(CaminhoArquivo))
            return vendas;

        foreach (string linha in File.ReadAllLines(CaminhoArquivo))
        {
            if (!string.IsNullOrWhiteSpace(linha))
                vendas.Add(FromFileString(linha));
        }

        return vendas;
    }

    // Recebe a lista atualizada, converte as vendas em registros e grava em Venda.path/Venda.file.
    // Acrescenta novos registros ou regrava o arquivo após edições/exclusões.
    public static void SalvarTodas(IEnumerable<Venda> vendas)
    {
        ArgumentNullException.ThrowIfNull(vendas);

        var novasLinhas = vendas.Select(venda => venda.ToFileString()).ToList();
        var linhasSalvas = File.Exists(CaminhoArquivo)
            ? File.ReadAllLines(CaminhoArquivo).Where(linha => !string.IsNullOrWhiteSpace(linha)).ToList()
            : new List<string>();

        if (linhasSalvas.SequenceEqual(novasLinhas))
            return;

        
        bool somenteNovasVendas = linhasSalvas.Count <= novasLinhas.Count &&
            linhasSalvas.SequenceEqual(novasLinhas.Take(linhasSalvas.Count));

        Directory.CreateDirectory(path);

        if (somenteNovasVendas)
        {
            using var sw = new StreamWriter(CaminhoArquivo, append: true);
            foreach (string linha in novasLinhas.Skip(linhasSalvas.Count))
                sw.WriteLine(linha);
            return;
        }

        // edições ou exclusões exigem atualizar os registros já salvos
        File.WriteAllLines(CaminhoArquivo, novasLinhas);
    }

    // Recebe o Id, procura em Venda.path/Venda.file e exibe a venda no console.
    public static void LocalizarPorId(uint id)
    {
        var venda = CarregarTodas().FirstOrDefault(v => v.Id == id);
        if (venda is null)
        {
            Console.WriteLine("Venda não encontrada.");
            return;
        }

        ExibirVenda(venda);
    }

    // Recebe o Id, remove a venda de Venda.path/Venda.file e seus itens em ItemVenda.path/ItemVenda.file.
    public static bool ExcluirVenda(uint id)
    {
        var vendas = CarregarTodas();
        var venda = vendas.FirstOrDefault(v => v.Id == id);
        if (venda is null)
            return false;

        vendas.Remove(venda);
        SalvarTodas(vendas);
        ItemVenda.ExcluirPorVenda(id);
        return true;
    }

    // Carrega Venda.path/Venda.file e permite navegar pelos registros no console.
    public static void ImprimirPorRegistro()
    {
        var vendas = CarregarTodas();
        if (vendas.Count == 0)
        {
            Console.WriteLine("Nenhuma venda cadastrada.");
            return;
        }

        int index = 0;

        while (true)
        {
            Console.Clear();
            var venda = vendas[index];

            Console.WriteLine($"Registro {index + 1} de {vendas.Count}\n");
            Console.WriteLine(venda);

            Console.WriteLine("\n1 - Próximo | 2 - Anterior | 3 - Primeiro | 4 - Último | 0 - Sair");
            Console.Write("Opção: ");
            switch (LerEntrada()?.Trim())
            {
                case "1":
                    if (index < vendas.Count - 1) index++;
                    break;
                case "2":
                    if (index > 0) index--;
                    break;
                case "3":
                    index = 0;
                    break;
                case "4":
                    index = vendas.Count - 1;
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Opção inválida!");
                    Thread.Sleep(700);
                    break;
            }
        }
    }

    // Recebe a data de nascimento e uma data de referência; devolve true se a pessoa tem 18 anos ou mais.
    public static bool MaiorIdade(DateOnly dataNascimento, DateOnly dataReferencia)
    {
        int idade = dataReferencia.Year - dataNascimento.Year;
        if (dataNascimento > dataReferencia.AddYears(-idade))
            idade--;

        return idade >= 18;
    }

    // Recebe uma linha de Venda.dat, separa e valida os campos, atualiza o próximo Id
    // e devolve a venda reconstruída em memória.
    public static Venda FromFileString(string linha)
    {
        if (string.IsNullOrEmpty(linha) || linha.Length != TamanhoLinha)
            throw new FormatException($"Registro de Venda inválido. Esperado {TamanhoLinha} caracteres, encontrado {linha?.Length ?? 0}.");

        uint id = uint.Parse(linha.Substring(0, 5));
        DateOnly dataVenda = DateOnly.ParseExact(linha.Substring(5, 8), "ddMMyyyy", System.Globalization.CultureInfo.InvariantCulture);
        string cpf = linha.Substring(13, 11);
        decimal valorTotal = decimal.Parse(linha.Substring(24, 7)) / 100m;

        if (id >= _proximoId)
            _proximoId = id + 1;

        return new Venda(id, dataVenda, cpf, valorTotal);
    }

    // Carrega todas as vendas e exibe cada uma com nome, nascimento e itens.
    private static void ExibirLista()
    {
        Console.Clear();
        var vendas = CarregarTodas();
        if (vendas.Count == 0)
        {
            Console.WriteLine("Nenhuma venda cadastrada.");
            return;
        }

        foreach (Venda venda in vendas)
        {
            Console.WriteLine($"\n--- Venda {venda.Id:D5} ---");
            ExibirVenda(venda);
        }
    }

    // Consulta o cadastro de Produto e exibe todos os produtos cadastrados, vendidos ou não.
    private static void ExibirListaProdutos()
    {
        Console.Clear();
        if (ListarProdutos is null)
        {
            Console.WriteLine("A listagem do cadastro de Produto ainda não foi conectada.");
            return;
        }

        var produtos = ListarProdutos().ToList();
        if (produtos.Count == 0)
        {
            Console.WriteLine("Nenhum produto cadastrado.");
            return;
        }

        foreach (var produto in produtos)
        {
            Console.WriteLine($"\nCódigo de Barras: {produto.CodigoBarras}");
            Console.WriteLine($"Nome: {produto.Nome}");
            Console.WriteLine($"Valor de Venda: R$ {produto.ValorVenda:N2}");
            Console.WriteLine($"Última Venda: {produto.UltimaVenda:dd/MM/yyyy}");
            Console.WriteLine($"Data de Cadastro: {produto.DataCadastro:dd/MM/yyyy}");
            Console.WriteLine($"Situação: {produto.Situacao}");
        }
    }

    // Exibe a venda, consulta Cliente pelo CPF e lista os itens associados.
    private static void ExibirVenda(Venda venda)
    {
        Console.WriteLine(venda);
        if (ConsultarCliente is null)
        {
            Console.WriteLine("Nome e nascimento: consulta ao cadastro de Cliente não conectada.");
        }
        else
        {
            var cliente = ConsultarCliente(venda.Cliente);
            if (cliente is null)
                Console.WriteLine("Nome e nascimento: cliente não encontrado no cadastro atual.");
            else
                Console.WriteLine($"Nome do Cliente: {cliente.Value.Nome}\nData de Nascimento: {cliente.Value.DataNascimento:dd/MM/yyyy}");
        }

        Console.WriteLine("Itens:");
        var itens = ItemVenda.CarregarPorVenda(venda.Id);
        if (itens.Count == 0)
        {
            Console.WriteLine("Sem itens cadastrados.");
            return;
        }

        foreach (ItemVenda item in itens)
        {
            var produto = ConsultarProduto?.Invoke(item.Produto);
            string nomeProduto = produto?.Nome ?? "Produto não encontrado no cadastro atual";
            Console.WriteLine($"{nomeProduto} ({item.Produto}) | Qtd: {item.Quantidade} | Unit.: R$ {item.ValorUnitario:N2} | Total: R$ {item.TotalItem:N2}");
        }
    }

    // Lê Risco.dat, que contém um CPF por linha, e verifica se o cliente está inadimplente.
    private static bool ClienteEstaInadimplente(string cpf)
    {
        string caminhoRisco = Path.Combine(path, "Risco.dat");
        if (!File.Exists(caminhoRisco))
            return false;

        return File.ReadAllLines(caminhoRisco)
            .Any(linha => string.Equals(linha.Trim(), cpf, StringComparison.Ordinal));
    }

    // Aguarda uma tecla antes de retornar ao menu principal.
    private static void PausarMenu()
    {
        Console.WriteLine("\nPressione Enter para continuar...");
        try
        {
            LerEntrada();
        }
        catch (OperationCanceledException)
        {
            // /cancel já retorna naturalmente ao menu principal nesta pausa.
        }
    }

    // Lê uma entrada do console e cancela a operação atual quando o usuário digita /cancel.
    private static string? LerEntrada()
    {
        string? entrada = Console.ReadLine();
        if (string.Equals(entrada?.Trim(), "/cancel", StringComparison.OrdinalIgnoreCase))
            throw new OperationCanceledException();

        return entrada;
    }

    // Devolve os dados da venda formatados para exibição no console.
    public override string ToString()
    {
        return $"Id da Venda : {Id:D5}\n" +
               $"Data da Venda: {DataVenda:dd/MM/yyyy}\n" +
               $"CPF Cliente : {Cliente}\n" +
               $"Valor Total : R$ {ValorTotal:N2}";
    }


    //
    // MENU E FUNÇOES DENTRO DA CLASS - AGUARDANDO DEFINITIVO
    //


    // Prepara os caminhos dos arquivos e controla a navegação do módulo de vendas.
    public static void ExecutarMenu()
    {
        try
        {
            Directory.CreateDirectory(path);
            Directory.CreateDirectory(ItemVenda.path);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro ao preparar a pasta de dados: {ex.Message}");
            return;
        }

        while (true)
        {
            Console.Clear();
            Console.WriteLine("=== MÓDULO DE VENDAS ===");
            Console.WriteLine($"Pasta de dados: {path}");
            Console.WriteLine("1 - Cadastrar venda");
            Console.WriteLine("2 - Localizar venda");
            Console.WriteLine("3 - Excluir venda e itens");
            Console.WriteLine("4 - Exibir lista de vendas");
            Console.WriteLine("5 - Exibir lista de produtos cadastrados");
            Console.WriteLine("0 - Sair");
            Console.Write("Opção: ");

            try
            {
                switch (LerEntrada()?.Trim())
                {
                    case "1":
                        CadastrarVendaNoMenu();
                        PausarMenu();
                        break;
                    case "2":
                        LocalizarVendaNoMenu();
                        PausarMenu();
                        break;
                    case "3":
                        ExcluirVendaNoMenu();
                        PausarMenu();
                        break;
                    case "4":
                        ExibirLista();
                        PausarMenu();
                        break;
                    case "5":
                        ExibirListaProdutos();
                        PausarMenu();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Opção inválida.");
                        PausarMenu();
                        break;
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Operação cancelada. Voltando ao menu principal.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
                PausarMenu();
            }
        }
    }

    // Pede o CPF, busca o cliente pelo método conectado, exibe nome e nascimento,
    // confirma o cliente, recebe de 1 a 3 itens e grava a venda e os itens.
    private static void CadastrarVendaNoMenu()
    {
        Console.Clear();
        if (ConsultarCliente is null)
        {
            Console.WriteLine("A consulta ao cadastro de Cliente ainda não foi conectada.");
            Console.WriteLine("Atribua o método de busca de Cliente a Venda.ConsultarCliente.");
            return;
        }

        if (ConsultarProduto is null)
        {
            Console.WriteLine("A consulta ao cadastro de Produto ainda não foi conectada.");
            Console.WriteLine("Atribua o método de busca de Produto a Venda.ConsultarProduto.");
            return;
        }

        string cpf;
        (string Nome, DateOnly DataNascimento, char Situacao)? cliente;
        while (true)
        {
            cpf = LerCpfValido();
            cliente = ConsultarCliente(cpf);
            if (cliente is not null)
                break;

            Console.WriteLine("Cliente não encontrado. Confira o CPF e tente novamente.");
        }

        bool clienteInadimplente = ClienteEstaInadimplente(cpf);
        Venda? venda = CadastrarVenda(
            cpf,
            cliente.Value.Nome,
            cliente.Value.DataNascimento,
            cliente.Value.Situacao,
            clienteInadimplente);

        if (venda is null)
            return;

        var itens = new List<ItemVenda>();
        while (itens.Count < ItemVenda.MaxItensPorVenda)
        {
            Console.WriteLine($"\nItem {itens.Count + 1} de {ItemVenda.MaxItensPorVenda}");
            var produto = LerProdutoAtivo();
            ItemVenda item;

            while (true)
            {
                int quantidade = LerQuantidadeValida();
                try
                {
                    item = new ItemVenda(venda, produto.Codigo, quantidade, produto.ValorVenda);
                    break;
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    Console.WriteLine($"Não foi possível calcular o item: {ex.Message}");
                    Console.WriteLine("Informe novamente a quantidade.");
                }
            }

            itens.Add(item);
            Console.WriteLine($"Total do item: R$ {item.TotalItem:N2}");

            if (itens.Count == ItemVenda.MaxItensPorVenda)
                break;

            Console.Write("Adicionar outro item? (S/N): ");
            if (!string.Equals(LerEntrada()?.Trim(), "S", StringComparison.OrdinalIgnoreCase))
                break;
        }

        venda.DefinirValorTotal(itens);
        var vendas = CarregarTodas();
        vendas.Add(venda);
        SalvarTodas(vendas);
        ItemVenda.SalvarItens(itens);
        Console.WriteLine($"\nVenda {venda.Id:D5} salva. Total: R$ {venda.ValorTotal:N2}");
    }

    // Repete a leitura até receber um CPF de 11 dígitos validado pelo Utils.
    private static string LerCpfValido()
    {
        while (true)
        {
            Console.Write("CPF do cliente (11 dígitos): ");
            string cpf = (LerEntrada() ?? string.Empty).Trim();
            if (cpf.Length == 11 && Utils.ValidarCPF(cpf))
                return cpf;

            Console.WriteLine("CPF inválido. Digite os 11 dígitos corretamente.");
        }
    }

    // Repete a entrada até localizar um produto ativo pelo código EAN-13 e obter seu nome e preço.
    private static (string Codigo, string Nome, decimal ValorVenda) LerProdutoAtivo()
    {
        while (true)
        {
            Console.Write("Código do produto (EAN-13 iniciando com 789): ");
            string codigo = (LerEntrada() ?? string.Empty).Trim();
            if (codigo.Length != 13 || !codigo.StartsWith("789", StringComparison.Ordinal) || !codigo.All(char.IsDigit))
            {
                Console.WriteLine("Código inválido. Informe 13 dígitos começando com 789.");
                continue;
            }

            var produto = ConsultarProduto?.Invoke(codigo);
            if (produto is null)
            {
                Console.WriteLine("Produto não encontrado. Digite outro código.");
                continue;
            }

            if (produto.Value.Situacao != (char)ESituacao.Ativo)
            {
                Console.WriteLine("Produto inativo. Escolha outro produto.");
                continue;
            }

            if (produto.Value.ValorVenda <= 0 || produto.Value.ValorVenda > ItemVenda.ValorUnitarioMaximo)
            {
                Console.WriteLine($"O preço do produto deve ser maior que zero e no máximo R$ {ItemVenda.ValorUnitarioMaximo:N2}.");
                continue;
            }

            Console.WriteLine($"Produto: {produto.Value.Nome} | Valor unitário: R$ {produto.Value.ValorVenda:N2}");
            return (codigo, produto.Value.Nome, produto.Value.ValorVenda);
        }
    }

    // Repete a entrada até receber uma quantidade entre 1 e 999.
    private static int LerQuantidadeValida()
    {
        while (true)
        {
            Console.Write($"Quantidade (1 a {ItemVenda.QuantidadeMaxima}): ");
            if (int.TryParse(LerEntrada(), out int quantidade) && quantidade >= 1 && quantidade <= ItemVenda.QuantidadeMaxima)
                return quantidade;

            Console.WriteLine("Quantidade inválida.");
        }
    }

    // Procura o Id informado em Venda.dat e mostra os dados do cliente e os itens relacionados.
    private static void LocalizarVendaNoMenu()
    {
        Console.Clear();
        Console.Write("Id da venda: ");
        if (!uint.TryParse(LerEntrada(), out uint id))
        {
            Console.WriteLine("Id inválido.");
            return;
        }

        LocalizarPorId(id);
    }

    // Remove a venda pelo Id e apaga também os itens associados.
    private static void ExcluirVendaNoMenu()
    {
        Console.Clear();
        Console.Write("Id da venda para excluir: ");
        if (!uint.TryParse(LerEntrada(), out uint id))
        {
            Console.WriteLine("Id inválido.");
            return;
        }

        bool excluida = ExcluirVenda(id);
        Console.WriteLine(excluida ? "Venda e itens excluídos." : "Venda não encontrada.");
    }

}
