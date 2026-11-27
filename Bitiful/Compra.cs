public class Compra
{
    private const int MaxItensPorCompra = 3;
    private const uint IdMaximo = 99999;
    private const decimal ValorTotalMaximo = 99999.99m;

    private const string NomeArquivo = "Compra.dat";
    private const int TamanhoLinha = 34;

    private static readonly string Pasta =
        Path.Combine(
            Environment.GetEnvironmentVariable("USERPROFILE") ?? "",
            "Bitiful");

    public uint Id { get; private set; }
    public DateOnly DataCompra { get; private set; }
    public string FornecedorCnpj { get; private set; }
    public decimal ValorTotal { get; private set; }

    private List<ItemCompra> itens = new();
    private bool finalizada;

    public IReadOnlyList<ItemCompra> Itens => itens;

    // Dados do fornecedor usados para mostrar a identificação.
    public struct DadosFornecedor
    {
        public string RazaoSocial;
        public DateOnly DataAbertura;
        public bool Ativo;
    }

    // Cria uma nova compra.
    public Compra(string cnpjFornecedor)
    {
        FornecedorCnpj = ValidarCnpj(cnpjFornecedor);

        ValidarFornecedor(FornecedorCnpj);

        Id = GerarProximoId();
        DataCompra = DateOnly.FromDateTime(DateTime.Today);
        ValorTotal = 0;
    }

    // Reconstrói uma compra que veio do arquivo.
    private Compra(uint id, DateOnly data, string cnpj, decimal total)
    {
        Id = id;
        DataCompra = data;
        FornecedorCnpj = cnpj;
        ValorTotal = total;
        finalizada = true;
    }

    // Retira os caracteres de formatação e valida o CNPJ.
    private static string ValidarCnpj(string cnpj)
    {
        string digitos = "";

        foreach (char caractere in cnpj)
        {
            if (char.IsDigit(caractere))
                digitos += caractere;
        }

        if (digitos.Length != 14 || !Utils.ValidarCNPJ(digitos))
            throw new ArgumentException("CNPJ inválido.");

        bool todosIguais = true;

        for (int i = 1; i < digitos.Length; i++)
        {
            if (digitos[i] != digitos[0])
            {
                todosIguais = false;
                break;
            }
        }

        if (todosIguais)
            throw new ArgumentException("CNPJ inválido.");

        return digitos;
    }

    // Confere se o fornecedor pode realizar uma compra.
    public static DadosFornecedor ValidarFornecedor(string cnpj)
    {
        cnpj = ValidarCnpj(cnpj);

        DadosFornecedor? fornecedor = ConsultarFornecedor(cnpj);

        if (fornecedor == null)
            throw new InvalidOperationException("Fornecedor não cadastrado.");

        if (!fornecedor.Value.Ativo)
            throw new InvalidOperationException("Fornecedor inativo.");

        if (FornecedorBloqueado(cnpj))
            throw new InvalidOperationException("Fornecedor bloqueado.");

        DateOnly hoje = DateOnly.FromDateTime(DateTime.Today);

        if (fornecedor.Value.DataAbertura.AddMonths(6) > hoje)
            throw new InvalidOperationException(
                "O fornecedor precisa ter pelo menos 6 meses de abertura.");

        return fornecedor.Value;
    }

    // Procura o fornecedor no arquivo Fornecedor.dat.
    private static DadosFornecedor? ConsultarFornecedor(string cnpj)
    {
        string caminho = GarantirArquivo("Fornecedor.dat");

        foreach (string linha in File.ReadAllLines(caminho))
        {
            if (linha.Length < 95)
                continue;

            if (linha.Substring(0, 14).Trim() != cnpj)
                continue;

            string razaoSocial = linha.Substring(14, 50).Trim();

            DateOnly abertura;

            if (!DateOnly.TryParseExact(
                linha.Substring(64, 10),
                "dd/MM/yyyy",
                out abertura))
            {
                throw new FormatException("Data de abertura inválida.");
            }

            bool ativo = linha.Substring(94, 1) == "A";

            return new DadosFornecedor
            {
                RazaoSocial = razaoSocial,
                DataAbertura = abertura,
                Ativo = ativo
            };
        }

        return null;
    }

    // Verifica se o CNPJ está no arquivo de bloqueados.
    private static bool FornecedorBloqueado(string cnpj)
    {
        string caminho = GarantirArquivo("Bloqueado.dat");

        foreach (string linha in File.ReadAllLines(caminho))
        {
            if (linha.Trim() == cnpj)
                return true;
        }

        return false;
    }

    // Adiciona um item à compra.
    public bool AdicionarItem(ItemCompra item)
    {
        if (finalizada || item == null)
            return false;

        if (item.Id != Id)
            return false;

        if (itens.Count >= MaxItensPorCompra)
            return false;

        if (ValorTotal + item.TotalItem > ValorTotalMaximo)
            return false;

        itens.Add(item);

        CalcularValorTotal();

        return true;
    }

    // Soma o total de todos os itens.
    public decimal CalcularValorTotal()
    {
        decimal total = 0;

        foreach (ItemCompra item in itens)
        {
            total += item.TotalItem;
        }

        if (total > ValorTotalMaximo)
            throw new InvalidOperationException(
                "O valor total da compra ultrapassou o limite.");

        ValorTotal = total;

        return ValorTotal;
    }

    // Grava a compra e seus itens nos arquivos.
    public bool FinalizarCompra()
    {
        if (finalizada)
            return false;

        if (itens.Count < 1 || itens.Count > MaxItensPorCompra)
            return false;

        CalcularValorTotal();

        if (ValorTotal <= 0 || ValorTotal > ValorTotalMaximo)
            return false;

        string caminho = GarantirArquivo(NomeArquivo);

        File.AppendAllText(
            caminho,
            ToFileString() + Environment.NewLine);

        try
        {
            ItemCompra.Salvar(itens);
        }
        catch
        {
            RemoverDoArquivo(Id);
            throw;
        }

        finalizada = true;

        return true;
    }

    // Informa quantos itens existem na compra.
    public int QuantidadeDeItens()
    {
        return itens.Count;
    }

    // Exibe os itens e o valor total da compra.
    public void ListarItens()
    {
        foreach (ItemCompra item in itens)
        {
            Console.WriteLine("--------------------------");
            Console.WriteLine($"Matéria-prima: {item.MateriaPrima}");
            Console.WriteLine($"Quantidade: {item.Quantidade}");
            Console.WriteLine($"Valor unitário: {item.ValorUnitario}");
            Console.WriteLine($"Total do item: {item.TotalItem}");
        }

        Console.WriteLine("--------------------------");
        Console.WriteLine($"Valor total da compra: {ValorTotal}");
    }

    public override string ToString()
    {
        return
            $"Id da compra: {Id}\n" +
            $"Data da compra: {DataCompra:dd/MM/yyyy}\n" +
            $"CNPJ do fornecedor: {FornecedorCnpj}\n" +
            $"Valor total: {ValorTotal}";
    }

    // Garante que o arquivo exista e retorna seu caminho.
    internal static string GarantirArquivo(string nomeArquivo)
    {
        if (!Utils.ValidarCaminhoArquivo(nomeArquivo))
            throw new IOException("Não foi possível acessar o arquivo.");

        return Path.Combine(Pasta, nomeArquivo);
    }

    // Gera o próximo ID usando o maior ID já gravado.
    private static uint GerarProximoId()
    {
        string caminho = GarantirArquivo(NomeArquivo);
        uint maiorId = 0;

        foreach (string linha in File.ReadAllLines(caminho))
        {
            if (linha.Length < 5)
                continue;

            if (uint.TryParse(linha.Substring(0, 5), out uint id))
            {
                if (id > maiorId)
                    maiorId = id;
            }
        }

        if (maiorId >= IdMaximo)
            throw new InvalidOperationException("Limite de compras atingido.");

        return maiorId + 1;
    }

    // Converte a compra em uma linha para o arquivo.
    public string ToFileString()
    {
        int valorEmCentavos = (int)(ValorTotal * 100);

        string linha =
            Id.ToString().PadLeft(5, '0') +
            DataCompra.ToString("ddMMyyyy") +
            FornecedorCnpj +
            valorEmCentavos.ToString().PadLeft(7, '0');

        if (linha.Length != TamanhoLinha)
            throw new InvalidOperationException("Registro de compra inválido.");

        return linha;
    }

    // Transforma uma linha do arquivo em uma compra.
    private static Compra FromFileString(string linha)
    {
        if (linha.Length != TamanhoLinha)
            throw new FormatException("Registro de compra inválido.");

        uint id = uint.Parse(linha.Substring(0, 5));

        DateOnly data = DateOnly.ParseExact(
            linha.Substring(5, 8), "ddMMyyyy");

        string cnpj = linha.Substring(13, 14);

        decimal total =
            int.Parse(linha.Substring(27, 7)) / 100m;

        return new Compra(id, data, cnpj, total);
    }

    // Carrega todas as compras e seus respectivos itens.
    public static List<Compra> CarregarTodas()
    {
        string caminho = GarantirArquivo(NomeArquivo);

        List<Compra> compras = new();
        List<ItemCompra> todosItens = ItemCompra.CarregarTodos();

        foreach (string linha in File.ReadAllLines(caminho))
        {
            if (string.IsNullOrWhiteSpace(linha))
                continue;

            Compra compra = FromFileString(linha);

            foreach (ItemCompra item in todosItens)
            {
                if (item.Id == compra.Id)
                    compra.itens.Add(item);
            }

            compras.Add(compra);
        }

        return compras;
    }

    // Localiza uma compra pelo ID.
    public static Compra? LocalizarPorId(uint id)
    {
        List<Compra> compras = CarregarTodas();

        foreach (Compra compra in compras)
        {
            if (compra.Id == id)
                return compra;
        }

        return null;
    }

    // Remove uma compra do arquivo.
    private static bool RemoverDoArquivo(uint id)
    {
        string caminho = GarantirArquivo(NomeArquivo);
        List<string> linhas = new();
        bool encontrou = false;

        foreach (string linha in File.ReadAllLines(caminho))
        {
            if (linha.Length >= 5 &&
                uint.TryParse(linha.Substring(0, 5), out uint idLinha) &&
                idLinha == id)
            {
                encontrou = true;
            }
            else
            {
                linhas.Add(linha);
            }
        }

        if (encontrou)
            File.WriteAllLines(caminho, linhas);

        return encontrou;
    }

    // Exclui a compra e todos os seus itens.
    public static bool ExcluirCompra(uint id)
    {
        if (!RemoverDoArquivo(id))
            return false;

        ItemCompra.ExcluirPorCompra(id);

        return true;
    }

    // Permite navegar pelos registros cadastrados.
    public static void ImprimirPorRegistro()
    {
        List<Compra> compras = CarregarTodas();

        if (compras.Count == 0)
        {
            Console.WriteLine("Nenhuma compra cadastrada.");
            return;
        }

        int indice = 0;

        while (true)
        {
            Console.Clear();

            Console.WriteLine($"Registro {indice + 1} de {compras.Count}");
            Console.WriteLine(compras[indice]);
            Console.WriteLine();

            compras[indice].ListarItens();

            Console.WriteLine();
            Console.WriteLine("1 - Próximo");
            Console.WriteLine("2 - Anterior");
            Console.WriteLine("3 - Primeiro");
            Console.WriteLine("4 - Último");
            Console.WriteLine("0 - Sair");

            string opcao = Console.ReadLine() ?? "";

            switch (opcao)
            {
                case "1":
                    if (indice < compras.Count - 1)
                        indice++;
                    break;

                case "2":
                    if (indice > 0)
                        indice--;
                    break;

                case "3":
                    indice = 0;
                    break;

                case "4":
                    indice = compras.Count - 1;
                    break;

                case "0":
                    return;
            }
        }
    }

    // Menu principal do módulo de compras.
    public static void ExecutarMenu()
    {
        while (true)
        {
            Console.WriteLine("\n=== COMPRA DE MATÉRIA-PRIMA ===");
            Console.WriteLine("1 - Cadastrar compra");
            Console.WriteLine("2 - Localizar compra");
            Console.WriteLine("3 - Excluir compra");
            Console.WriteLine("4 - Imprimir por registro");
            Console.WriteLine("0 - Voltar");
            Console.Write("Opção: ");

            string opcao = Console.ReadLine() ?? "";

            switch (opcao)
            {
                case "1":
                    CadastrarNoMenu();
                    break;

                case "2":
                    LocalizarNoMenu();
                    break;

                case "3":
                    ExcluirNoMenu();
                    break;

                case "4":
                    ImprimirPorRegistro();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine("Opção inválida.");
                    break;
            }
        }
    }

    // Realiza o cadastro de uma compra pelo console.
    private static void CadastrarNoMenu()
    {
        Console.Write("CNPJ do fornecedor: ");
        string cnpj = Console.ReadLine() ?? "";

        try
        {
            DadosFornecedor fornecedor = ValidarFornecedor(cnpj);

            Console.WriteLine($"Razão social: {fornecedor.RazaoSocial}");
            Console.WriteLine($"Data de abertura: {fornecedor.DataAbertura:dd/MM/yyyy}");

            Compra compra = new Compra(cnpj);

            while (compra.QuantidadeDeItens() < MaxItensPorCompra)
            {
                Console.Write("Código da matéria-prima (MP0000): ");
                string codigo = Console.ReadLine() ?? "";

                Console.Write("Quantidade: ");
                decimal quantidade = decimal.Parse(Console.ReadLine() ?? "0");

                Console.Write("Valor unitário: ");
                decimal valorUnitario = decimal.Parse(Console.ReadLine() ?? "0");

                ItemCompra item = new ItemCompra(
                    compra, codigo, quantidade, valorUnitario);

                if (!compra.AdicionarItem(item))
                {
                    Console.WriteLine("Não foi possível adicionar o item.");
                    break;
                }

                Console.WriteLine("Item adicionado.");

                if (compra.QuantidadeDeItens() == MaxItensPorCompra)
                    break;

                Console.Write("Adicionar outro item? (S/N): ");

                if ((Console.ReadLine() ?? "").ToUpper() != "S")
                    break;
            }

            if (compra.FinalizarCompra())
                Console.WriteLine("Compra cadastrada com sucesso.");
            else
                Console.WriteLine("A compra precisa ter de 1 a 3 itens.");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    // Localiza e exibe uma compra.
    private static void LocalizarNoMenu()
    {
        Console.Write("Informe o ID da compra: ");

        if (!uint.TryParse(Console.ReadLine(), out uint id))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        Compra? compra = LocalizarPorId(id);

        if (compra == null)
        {
            Console.WriteLine("Compra não encontrada.");
            return;
        }

        Console.WriteLine(compra);
        compra.ListarItens();
    }

    // Exclui uma compra e seus itens.
    private static void ExcluirNoMenu()
    {
        Console.Write("Informe o ID da compra: ");

        if (!uint.TryParse(Console.ReadLine(), out uint id))
        {
            Console.WriteLine("ID inválido.");
            return;
        }

        if (ExcluirCompra(id))
            Console.WriteLine("Compra excluída.");
        else
            Console.WriteLine("Compra não encontrada.");
    }
}
