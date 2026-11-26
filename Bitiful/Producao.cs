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
        foreach(var item in Producoes)//Percorro a lista de produções e imprimo uma a uma
        {
            
            Console.WriteLine($"ID:{int.Parse(item.Id)}\tData de produção:{item.DataProducao}\tProduto:{item.Produto}\tQuantidade: {decimal.Parse(item.Quantidade)}");
        }
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