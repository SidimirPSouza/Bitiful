using System.Runtime.Intrinsics.X86;

using System.Globalization;

public class Producao
{
    public string Id { get; private set; }
    public DateOnly DataProducao { get; set; }
    public string Produto { get; set; }
    public string Quantidade { get; set; }
    public string Diretorio { get; private set; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Bitiful") + Path.DirectorySeparatorChar;
    public string Arquivo { get; private set; } = "Producao.dat";
    public ItemProducao ItemProd { get; set; }
    public List<Producao> Producoes = new List<Producao>();
    public bool DiretorioExistente() //Checando se o diretorio existe e criando ele
    {
        if (Directory.Exists(Diretorio))
        {
            return true;

        }
        else
        {
            try
            {
                Directory.CreateDirectory(Diretorio);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
    public bool ArquivoExistente() //checando se o arquivo existe e criando ele
    {
        if (DiretorioExistente())
        {
            if (File.Exists(Diretorio + Arquivo))
                return true;
            else
            {
                try
                {
                    using (File.Create(Diretorio + Arquivo)) { }
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }
        }
        else
        {
            Console.WriteLine("O diretorio não existe e não conseguiu ser criado");
            return false;
        }

    }
    public void CarregarArquivo()//carregando a lista com o arquivo
    {
        Producoes.Clear();
        if (!ArquivoExistente())
            return;

        foreach (var item in File.ReadAllLines(Diretorio + Arquivo))
        {
            if (string.IsNullOrWhiteSpace(item))
                continue;
            if (item.Length != 34)
                throw new FormatException("Registro de produção inválido: esperado 34 caracteres.");

            Producao aux = new Producao();
            aux.Id = SepararPorTamanho(0, 5, item);
            aux.DataProducao = DateOnly.Parse(SepararPorTamanho(5, 10, item), CultureInfo.InvariantCulture);
            aux.Produto = SepararPorTamanho(15, 13, item).Trim();
            aux.Quantidade = FormatarQuantidade(SepararPorTamanho(28, 6, item));
            Producoes.Add(aux);
        }
    }
    public void SalvarArquivo()//salvando a lista no arquivo
    {
        if (ArquivoExistente())
        {
            var sw = new StreamWriter(Diretorio + Arquivo);

            foreach(var item in Producoes)
            {
                sw.WriteLine(item.Id + item.DataProducao + item.Produto + item.Quantidade);
            }
            sw.Close();
        }
    }
    public void Cadastrar(string produto, string quantidade)
    {
        if (produto.Length != 13 || !produto.All(char.IsDigit))//o codigo de barras tem que ter 13 caracteres
        {
            Console.WriteLine("Código de barras inválido");
            return;
        }

        if (!ValidarQuantidade(quantidade))//a quantidade tem que ser no máximo 999,99   
                return;

        CarregarArquivo();
        Producao aux = new Producao();
        ItemProducao auxItemProd = new ItemProducao();

        if (ArquivoExistente())
        {
            
            if(Producoes.Find(p => p.Produto == produto) != null)
            {
                Producao aux2 = Producoes.Find(p => p.Produto == produto);
                string novaQuantidade = FormatarQuantidade((decimal.Parse(Producoes[Producoes.IndexOf(aux2)].Quantidade) +
                decimal.Parse(FormatarQuantidade(quantidade))).ToString());
                if (!ValidarQuantidade(novaQuantidade)) return;
                Producoes[Producoes.IndexOf(aux2)].Quantidade = novaQuantidade;
                Console.WriteLine(Producoes.IndexOf(aux2));
                SalvarArquivo();
                return;
            }
            aux.DataProducao = DateOnly.FromDateTime(DateTime.Today);
            aux.Produto = produto;
            aux.Quantidade = FormatarQuantidade(ParseQuantidade(quantidade).ToString(CultureInfo.InvariantCulture));

            //se a lista estiver vazia, o id vai ser 1, se não segue sendo autoincrementado, preenchendo os espaços vazios com 0
            if (Producoes.Count > 0) aux.Id = (int.Parse(Producoes.Last().Id) + 1).ToString().PadLeft(5,'0');
            else aux.Id = 1.ToString().PadLeft(5,'0');

            Producoes.Add(aux);
            Console.WriteLine("Cadastrado com sucesso!");

        }
        SalvarArquivo();
    }

    // Valida todos os produtos solicitados e baixa as produções mais antigas primeiro.
    // Nenhuma baixa é persistida se um produto não tiver estoque suficiente.
    public void ConsumirProdutos(IEnumerable<(string Codigo, int Quantidade)> consumos)
    {
        ArgumentNullException.ThrowIfNull(consumos);
        var solicitacoes = consumos.ToList();
        if (solicitacoes.Count == 0)
            throw new ArgumentException("Informe ao menos um produto para consumir.", nameof(consumos));

        var totalPorProduto = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var (codigo, quantidade) in solicitacoes)
        {
            if (string.IsNullOrWhiteSpace(codigo) || codigo.Length != 13 || !codigo.All(char.IsDigit))
                throw new ArgumentException("O código do produto deve conter 13 dígitos.", nameof(consumos));
            if (quantidade < 1 || quantidade > ItemVenda.QuantidadeMaxima)
                throw new ArgumentOutOfRangeException(nameof(consumos), $"A quantidade por produto deve estar entre 1 e {ItemVenda.QuantidadeMaxima}.");

            totalPorProduto[codigo] = totalPorProduto.GetValueOrDefault(codigo) + quantidade;
        }

        CarregarArquivo();
        var saldos = Producoes.Select(producao => ParseQuantidade(producao.Quantidade)).ToArray();

        foreach (var (codigo, quantidade) in totalPorProduto)
        {
            decimal disponivel = Producoes
                .Select((producao, indice) => (producao, indice))
                .Where(item => item.producao.Produto == codigo)
                .Sum(item => saldos[item.indice]);

            if (disponivel < quantidade)
                throw new InvalidOperationException($"Estoque insuficiente para o produto {codigo}. Disponível: {disponivel:N2}; solicitado: {quantidade}.");
        }

        foreach (var (codigo, quantidade) in totalPorProduto)
        {
            decimal restante = quantidade;
            for (int indice = 0; indice < Producoes.Count && restante > 0; indice++)
            {
                if (Producoes[indice].Produto != codigo || saldos[indice] <= 0)
                    continue;

                decimal baixa = Math.Min(saldos[indice], restante);
                saldos[indice] -= baixa;
                restante -= baixa;
                Producoes[indice].Quantidade = FormatarQuantidade(saldos[indice].ToString(CultureInfo.InvariantCulture));
            }
        }

        SalvarArquivo();
    }

    public void ValidarEstoqueProdutos(IEnumerable<(string Codigo, int Quantidade)> consumos)
    {
        ArgumentNullException.ThrowIfNull(consumos);
        var totalPorProduto = new Dictionary<string, int>(StringComparer.Ordinal);
        foreach (var (codigo, quantidade) in consumos)
        {
            if (string.IsNullOrWhiteSpace(codigo) || codigo.Length != 13 || !codigo.All(char.IsDigit))
                throw new ArgumentException("O código do produto deve conter 13 dígitos.", nameof(consumos));
            if (quantidade < 1 || quantidade > ItemVenda.QuantidadeMaxima)
                throw new ArgumentOutOfRangeException(nameof(consumos), $"A quantidade por produto deve estar entre 1 e {ItemVenda.QuantidadeMaxima}.");
            totalPorProduto[codigo] = totalPorProduto.GetValueOrDefault(codigo) + quantidade;
        }
        if (totalPorProduto.Count == 0)
            throw new ArgumentException("Informe ao menos um produto para consumir.", nameof(consumos));

        CarregarArquivo();
        foreach (var (codigo, quantidade) in totalPorProduto)
        {
            decimal disponivel = Producoes
                .Where(producao => producao.Produto == codigo)
                .Sum(producao => ParseQuantidade(producao.Quantidade));
            if (disponivel < quantidade)
                throw new InvalidOperationException($"Estoque insuficiente para o produto {codigo}. Disponível: {disponivel:N2}; solicitado: {quantidade}.");
        }
    }

    public Producao? Localizar(string id)
    {
        
        CarregarArquivo();
        
        if (Producoes.Count > 0)//se a lista não estiver vazia, retorno um objeto baseado no id que foi procurado
        {
            Producao _producao = Producoes.FirstOrDefault(p => p.Id.Equals(id));
            ItemProducao auxItemProd = new ItemProducao();
            if (_producao == null)
            {
                Console.WriteLine("Produção não localizada, cheque o ID e tente novamente");    
                return null;
            }
            Console.WriteLine($"ID:{int.Parse(_producao.Id)}\tData de produção:{_producao.DataProducao}\tProduto:{_producao.Produto}\tQuantidade: {decimal.Parse(_producao.Quantidade)}");
            auxItemProd.ImprimirMateriaPrima(_producao.Id);

            return _producao;
        }
        else
        {
            Console.WriteLine("Não há produções a serem localizadas");
            return null;
        }
    }
    public void Excluir(string id)
    {
        CarregarArquivo();
        if (Producoes.Count > 0) //mesmo principio da função Localizar, só que dessa vez para excluir
        {
            var item = Producoes.FirstOrDefault(p => p.Id.Equals(id));
            if(item == null)
            {
                Console.WriteLine("Produção não encontrada, cheque o ID e tente novamente");
                return;
            }
            Producoes.Remove(item);
            Console.WriteLine("Excluido com sucesso!");
        } else Console.WriteLine("Não tem produções a serem localizadas");
        SalvarArquivo();
    }
    public void ImpressaoRegistro()
    {
        CarregarArquivo();
        int index = 0;
        int opcao = 0;
        ItemProducao auxItemProd = new ItemProducao();
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"Item {index + 1} de {Producoes.Count}");
            Console.WriteLine($"\nID:{int.Parse(Producoes[index].Id)}\tData de produção:{Producoes[index].DataProducao}\tProduto:{Producoes[index].Produto}\tQuantidade: {decimal.Parse(Producoes[index].Quantidade)}");
            auxItemProd.ImprimirMateriaPrima(Producoes[index].Id);
            Console.WriteLine("\n1 - Próximo\t2 - Anterior\t3 - Primeiro\t4 - Último\t5 - Ir para\t0 - Sair");
            Console.WriteLine("Digite a opção: ");
            try
            {
                opcao = int.Parse(Console.ReadLine());
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                continue;
            }
            if (opcao > 5 || opcao < 0) continue;
            switch (opcao)
            {
                case 1:
                    if (index + 1 < Producoes.Count) index++;
                    else index = 0;
                    break;
                case 2:
                    if (index - 1 >= 0) index--;
                    else index = Producoes.Count-1;
                    break;
                case 3:
                    index = 0;
                    break;
                case 4:
                    index = Producoes.Count - 1;
                    break;
                case 5:
                IrPara:
                    int outraOpcao = 0;
                    Console.WriteLine("Digite a posicão que deseja ir");
                    try
                    {
                        outraOpcao = int.Parse(Console.ReadLine());
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.Message);
                        goto IrPara;
                    }
                    if (outraOpcao < 0 || outraOpcao > Producoes.Count)
                    {
                        Console.WriteLine("Posição fora do alcance");
                        goto IrPara;
                    }
                    index = outraOpcao - 1;
                    break;
                case 0:
                    return;             
            }
            
        }
    }


    public void DecrementarQuantidade(string produto, decimal quantidade)
    {
        CarregarArquivo();
        if (ValidarQuantidade(produto, quantidade.ToString()))
        {
            Producao aux = Producoes.Find(p => p.Produto == produto);
            aux.Quantidade = (decimal.Parse(aux.Quantidade) - quantidade).ToString();
            aux.Quantidade = FormatarQuantidade(aux.Quantidade);
        }
        SalvarArquivo();
    }
    public bool ValidarQuantidade(string produto, string quantidade)
    {
        Producao aux = Producoes.Find(p => p.Produto == produto);
        if(decimal.Parse(aux.Quantidade) - decimal.Parse(quantidade) < 0)
        {
            Console.WriteLine("Não tem produto o suficiente");
            return false;
        }
        return true;
    }
    
    string SepararPorTamanho(int inicio, int tamanho, string aSerSeparada)//percorro o trecho da string que foi definido pelo inicio e tamanho, e retorno o trecho especifico da string
    {
        
        string aux = string.Empty;
        int auxTamanho = tamanho + inicio;
        for (int i = inicio; i < auxTamanho; i++)
        {
           
            aux += aSerSeparada[i];

        }
        return aux;
    }
    bool ValidarQuantidade(string quantidade)//valido a quantidade baseando no limite de 999,99
    {
        if (decimal.Parse(quantidade) < 0) return false;
        string[] aux = quantidade.Split(",");
        if (decimal.Parse(quantidade) >= 1000)
        {
            Console.WriteLine("Quantidade inválida");
            return false;
        }

        if (aux.Contains(","))
        {
            decimal valor = ParseQuantidade(quantidade);
            return valor > 0 && valor <= 999.99m;
        }
        catch (FormatException)
        {
            return false;
        }
    }

    private static decimal ParseQuantidade(string quantidade)
    {
        string normalizada = quantidade.Trim().Replace(',', '.');
        if (!decimal.TryParse(normalizada, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal valor))
            throw new FormatException($"Quantidade inválida: '{quantidade}'.");
        return valor;
    }

    string FormatarQuantidade(string quantidade)//formato a quantidade para que sempre tenha 5 digitos(6 contando com a vírgula)
    {
        
        string[] aux = quantidade.Split(",");
        if (quantidade.Contains(','))
        {
            aux[1] = aux[1].PadRight(2, '0');
            aux[0] = aux[0].PadLeft(3, '0');
            return $"{aux[0]},{aux[1]}";
        }
        else
        {
            aux[0] = aux[0].PadLeft(3, '0');
            return $"{aux[0]},00";
        }

    }

}
