using System.Runtime.Intrinsics.X86;

public class Producao
{
    public string Id { get; private set; }
    public DateOnly DataProducao { get; set; }
    public string Produto { get; set; }
    public string Quantidade { get; set; }
    public string Diretorio { get; private set; } = @"C:\dev\Biltiful\";
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
                    File.Create(Diretorio + Arquivo);
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
        var sr = new StreamReader(Diretorio + Arquivo);
        if (sr.Read() == -1)//se o arquivo estiver vazio eu não carrego nada
        {
            sr.Close();
            return;
        }
        if (ArquivoExistente())
        {
            foreach (var item in File.ReadAllLines(Diretorio + Arquivo))
            {
                Producao aux = new Producao();
                aux.Id = SepararPorTamanho(0, 5, item);
                aux.DataProducao = DateOnly.Parse(SepararPorTamanho(5, 10, item));
                aux.Produto = SepararPorTamanho(15, 13, item);
                string auxiliar = SepararPorTamanho(28, 6, item);
                aux.Quantidade = FormatarQuantidade(auxiliar);
                Producoes.Add(aux);

            }
        }
        sr.Close();
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
        if (produto.Length != 13)//o codigo de barras tem que ter 13 caracteres
        {
            Console.WriteLine("Código de barras inválido");
            return;
        }
        if (!ValidarQuantidade(quantidade))//a quantidade tem que ser no máximo 999,99
        {
            Console.WriteLine("Quantidade inválida, tem que ser no máximo 999,99");
            return;
        }
        CarregarArquivo();
        Producao aux = new Producao();
        ItemProducao auxItemProd = new ItemProducao();
        if (ArquivoExistente())
        {
            aux.DataProducao = DateOnly.FromDateTime(DateTime.Today);
            aux.Produto = produto;
            aux.Quantidade = FormatarQuantidade(quantidade);

            //se a lista estiver vazia, o id vai ser 1, se não segue sendo autoincrementado, preenchendo os espaços vazios com 0
            if (Producoes.Count > 0) aux.Id = (int.Parse(Producoes.Last().Id) + 1).ToString().PadLeft(5,'0');
            else aux.Id = 1.ToString().PadLeft(5,'0');

            

            Producoes.Add(aux);
            Console.WriteLine("Cadastrado com sucesso!");
        }
        SalvarArquivo();
    }
    public Producao? Localizar(string id)
    {
        
        CarregarArquivo();

        if (Producoes.Count > 0)//se a lista não estiver vazia, retorno um objeto baseado no id que foi procurado
        {
            Producao _producao = Producoes.FirstOrDefault(p => p.Id.Equals(id));
            ItemProducao auxItemProd = new ItemProducao();

            Console.WriteLine($"ID:{int.Parse(_producao.Id)}\tData de produção:{_producao.DataProducao}\tProduto:{_producao.Produto}\tQuantidade: {decimal.Parse(_producao.Quantidade)}");
            auxItemProd.ImprimirMateriaPrima(_producao.Id);

            return _producao;
        }
        else
        {
            Console.WriteLine("Não tem produções a serem localizadas");
            return null;
        }
    }
    public void Excluir(string id)
    {
        CarregarArquivo();
        if (Producoes.Count > 0) //mesmo principio da função Localizar, só que dessa vez para excluir
        {
            var item = Producoes.FirstOrDefault(p => p.Id.Equals(id));
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
    bool ValidarQuantidade(string quantidade)//valido a quantidade baseando no numero de caracteres que foram escritos
    {
        if (decimal.Parse(quantidade) < 0) return false;
        string[] aux = quantidade.Split(",");
        if (aux.Contains(","))
        {
            if (aux[1].Length > 2) return false;
            if (aux[0].Length > 3) return false;
        }
        else
        {
            if (aux[0].Length > 3) return false;
        }
        
        
        return true;
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