public class Venda
{
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

    // função que automatiza o incrementador de id
    private static uint GerarProximoId()
    {
        if (_proximoId > IdMaximo)
            throw new InvalidOperationException($"Limite máximo de {IdMaximo} vendas atingido.");

        return _proximoId++;
    }
    // metodo temporario, feito com ia para validar, aguardando classe utils
    private static void ValidarCpf(string cliente)
    {
        if (string.IsNullOrWhiteSpace(cliente) || cliente.Length != 11 || !cliente.All(char.IsDigit) || cliente.Distinct().Count() == 1)
            throw new ArgumentException("O CPF deve conter 11 dígitos válidos.", nameof(cliente));

        int soma = 0;
        for (int i = 0; i < 9; i++)
            soma += (cliente[i] - '0') * (10 - i);

        int primeiroDigito = soma % 11 < 2 ? 0 : 11 - soma % 11;
        soma = 0;
        for (int i = 0; i < 9; i++)
            soma += (cliente[i] - '0') * (11 - i);
        soma += primeiroDigito * 2;

        int segundoDigito = soma % 11 < 2 ? 0 : 11 - soma % 11;
        if (cliente[9] - '0' != primeiroDigito || cliente[10] - '0' != segundoDigito)
            throw new ArgumentException("O CPF informado não é válido.", nameof(cliente));
    }

    // gera o Id e a data atual para uma nova venda
    public Venda(string cliente)
    {
        ValidarCpf(cliente);

        Id = GerarProximoId();
        DataVenda = DateOnly.FromDateTime(DateTime.Now);
        Cliente = cliente;
        ValorTotal = 0m;
    }

    // recria uma venda na memoria já existente mantendo Id e DataVenda originais
    private Venda(uint id, DateOnly dataVenda, string cliente, decimal valorTotal)
    {
        ValidarCpf(cliente);

        Id = id;
        DataVenda = dataVenda;
        Cliente = cliente;
        ValorTotal = ValidarValorTotal(valorTotal);
    }

    // valida e formata
    private static decimal ValidarValorTotal(decimal valorTotal)
    {
        decimal arredondado = Math.Round(valorTotal, 2);
        if (arredondado <= 0 || arredondado > ValorTotalMaximo)
            throw new ArgumentOutOfRangeException(nameof(valorTotal), $"O ValorTotal (R$ {arredondado:N2}) deve ser maior que zero e no máximo R$ {ValorTotalMaximo:N2}.");

        return arredondado;
    }

    // define o valor total após os itens da venda serem definidos
    public void DefinirValorTotal(decimal valorTotal)
    {
        ValorTotal = ValidarValorTotal(valorTotal);
    }

    // edita os dados de uma venda pelo Id
    public static bool EditarVenda(uint id, string novoCliente, decimal novoValorTotal, string caminhoArquivo = "Venda.dat")
    {
        if (!File.Exists(caminhoArquivo))
            return false;

        var vendas = new List<Venda>();
        foreach (string linha in File.ReadAllLines(caminhoArquivo))
        {
            if (!string.IsNullOrWhiteSpace(linha))
                vendas.Add(FromFileString(linha));
        }

        var venda = vendas.FirstOrDefault(v => v.Id == id);
        if (venda is null)
            return false;

        ValidarCpf(novoCliente);
        venda.Cliente = novoCliente;
        venda.DefinirValorTotal(novoValorTotal);

        using var sw = new StreamWriter(caminhoArquivo, append: false);
        foreach (var v in vendas)
            sw.WriteLine(v.ToFileString());

        return true;
    }

    // método desenvolvido com auxílio de IA
    // valida o cliente e cria uma venda; o valor será definido depois dos itens
    public static Venda? CadastrarVenda(
        string cpfCliente,
        string nomeCliente,
        DateOnly dataNascimento,
        char situacao,
        bool clienteInadimplente,
        string caminhoVenda = "Venda.dat")
    {
        ValidarCpf(cpfCliente);

        if (situacao != 'A')
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
        if (!string.Equals(Console.ReadLine()?.Trim(), "S", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Cadastro cancelado.");
            return null;
        }

        CarregarTodas(caminhoVenda);
        return new Venda(cpfCliente);
    }

    // passa da memoria para arquivo
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

    // carrega todas as vendas salvas em Venda.dat
    public static List<Venda> CarregarTodas(string caminhoArquivo = "Venda.dat")
    {
        var vendas = new List<Venda>();
        if (!File.Exists(caminhoArquivo))
            return vendas;

        foreach (string linha in File.ReadAllLines(caminhoArquivo))
        {
            if (!string.IsNullOrWhiteSpace(linha))
                vendas.Add(FromFileString(linha));
        }

        return vendas;
    }

    // grava a lista completa de vendas em Venda.dat
    
    public static void SalvarTodas(IEnumerable<Venda> vendas, string caminhoArquivo = "Venda.dat")
    {
        ArgumentNullException.ThrowIfNull(vendas);

        var novasLinhas = vendas.Select(venda => venda.ToFileString()).ToList();
        var linhasSalvas = File.Exists(caminhoArquivo)
            ? File.ReadAllLines(caminhoArquivo).Where(linha => !string.IsNullOrWhiteSpace(linha)).ToList()
            : new List<string>();

        if (linhasSalvas.SequenceEqual(novasLinhas))
            return;

        // uso assistido de IA*
        bool somenteNovasVendas = linhasSalvas.Count <= novasLinhas.Count &&
            linhasSalvas.SequenceEqual(novasLinhas.Take(linhasSalvas.Count));

        string? pasta = Path.GetDirectoryName(caminhoArquivo);
        if (!string.IsNullOrEmpty(pasta))
            Directory.CreateDirectory(pasta);

        if (somenteNovasVendas)
        {
            using var sw = new StreamWriter(caminhoArquivo, append: true);
            foreach (string linha in novasLinhas.Skip(linhasSalvas.Count))
                sw.WriteLine(linha);
            return;
        }

        // edições ou exclusões exigem atualizar os registros já salvos
        File.WriteAllLines(caminhoArquivo, novasLinhas);
    }

    // localiza e exibe uma venda pelo Id
    public static void LocalizarPorId(uint id, string caminhoArquivo = "Venda.dat")
    {
        var venda = CarregarTodas(caminhoArquivo).FirstOrDefault(v => v.Id == id);
        if (venda is null)
        {
            Console.WriteLine("Venda não encontrada.");
            return;
        }

        Console.WriteLine(venda);
    }

    // exclui uma venda pelo Id
    public static bool ExcluirVenda(uint id, string caminhoArquivo = "Venda.dat")
    {
        var vendas = CarregarTodas(caminhoArquivo);
        var venda = vendas.FirstOrDefault(v => v.Id == id);
        if (venda is null)
            return false;

        vendas.Remove(venda);
        SalvarTodas(vendas, caminhoArquivo);
        return true;
    }

    // navega pelos registros de Venda.dat
    public static void ImprimirPorRegistro(string caminhoArquivo = "Venda.dat")
    {
        var vendas = CarregarTodas(caminhoArquivo);
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
            switch (Console.ReadLine()?.Trim())
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

    public static bool MaiorIdade(DateOnly dataNascimento, DateOnly dataReferencia)
    {
        int idade = dataReferencia.Year - dataNascimento.Year;
        if (dataNascimento > dataReferencia.AddYears(-idade))
            idade--;

        return idade >= 18;
    }

    // reconstroi do texto pra memoria
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

    public override string ToString()
    {
        return $"Id da Venda : {Id:D5}\n" +
               $"Data da Venda: {DataVenda:dd/MM/yyyy}\n" +
               $"CPF Cliente : {Cliente}\n" +
               $"Valor Total : R$ {ValorTotal:N2}";
    }
}
