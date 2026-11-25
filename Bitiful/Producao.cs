using System.Runtime.Intrinsics.X86;

public class Producao
{
    public string Id { get; private set; }
    public DateOnly DataProducao { get; set; }
    public string Produto { get; set; }
    public string Quantidade { get; set; }
    public string Diretorio { get; private set; } = @"C:\dev\Biltiful\";
    public string Arquivo { get; private set; } = "Producao.dat";

    private string LinhaEscrita { get; set; }
    public List<Producao> Producoes = new List<Producao>();


    
    public bool DiretorioExistente()
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
    public bool ArquivoExistente()
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

    public void CarregarArquivo()
    {
        Producoes.Clear();
        var sr = new StreamReader(Diretorio + Arquivo);
        if (sr.Read() == -1)
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
                Console.WriteLine("auxiliar é " + auxiliar);
                aux.Quantidade = FormatarQuantidade(auxiliar);
                Console.WriteLine("auxiliar formatada é  "+  aux.Quantidade);
                Producoes.Add(aux);

            }
        }
        sr.Close();
    }

    public void SalvarArquivo()
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
        if (produto.Length > 13 || produto.Length < 0)
        {
            Console.WriteLine("Código de barras inválido");
            return;
        }
        if (!ValidarQuantidade(quantidade))
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
            
            if (Producoes.Count > 0) aux.Id = (int.Parse(Producoes.Last().Id) + 1).ToString().PadLeft(5,'0');
            else aux.Id = 1.ToString().PadLeft(5,'0');

            Producoes.Add(aux);
        }
        SalvarArquivo();
    }

    public Producao? Localizar(string id)
    {
        
        CarregarArquivo();
        if (Producoes.Count > 0)
        {
            Producao _producao = Producoes.FirstOrDefault(p => p.Id.Equals(id));
            return _producao;
        }
        else return null;
    }

    public void Excluir(Producao producao)
    {
        CarregarArquivo();
        producao.Producoes.Remove(producao);
        SalvarArquivo();
    }
    public void ImpressaoRegistro()
    {
        CarregarArquivo();
        foreach(var item in Producoes)
        {
            Console.WriteLine($"ID:{int.Parse(item.Id)}\tData de produção:{item.DataProducao}\tProduto:{item.Produto}\tQuantidade: {item.Quantidade}");
        }
    }

    string SepararPorTamanho(int inicio, int tamanho, string separada)
    {
        
        string aux = string.Empty;
        int auxTamanho = tamanho + inicio;
        for (int i = inicio; i < auxTamanho; i++)
        {
           
            aux += separada[i];

        }
        return aux;
    }

    bool ValidarQuantidade(string quantidade)
    {
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

    string FormatarQuantidade(string quantidade)
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