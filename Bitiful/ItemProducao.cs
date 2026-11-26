public class ItemProducao
{
    public string IdProducao { get; set; }
    public DateOnly DataProducao { get; set; }
    public string MatPrima { get; set; }
    public string QuantidadeMateriaPrima { get; set; }

    public string Diretorio { get; private set; } = @"C:\dev\Biltiful\";
    public string Arquivo { get; private set; } = "Producao.dat";
    public List<ItemProducao> ListaItemProducao = new List<ItemProducao>();
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
        ListaItemProducao.Clear();
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
                ItemProducao aux = new ItemProducao();
                aux.IdProducao = SepararPorTamanho(0, 5, item);
                aux.DataProducao = DateOnly.Parse(SepararPorTamanho(5, 10, item));
                aux.MatPrima = SepararPorTamanho(15, 6, item);
                string auxiliar = SepararPorTamanho(21, 6, item);
                aux.QuantidadeMateriaPrima = FormatarQuantidade(auxiliar);
                ListaItemProducao.Add(aux);

            }
        }
        sr.Close();
    }
    public void SalvarArquivo()//salvando a lista no arquivo
    {
        if (ArquivoExistente())
        {
            var sw = new StreamWriter(Diretorio + Arquivo);

            foreach (var item in ListaItemProducao)
            {
                sw.WriteLine(item.IdProducao + item.DataProducao + item.MatPrima + item.QuantidadeMateriaPrima);
            }
            sw.Close();
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

    public void ListarMateriaPrima(string idProducao, string materiaPrima, string quantidade)
    {
        if(materiaPrima.Length != 6 && materiaPrima.Contains("mp".ToUpper()))
        {
            Console.WriteLine("Materia prima inválida");
            return;
        }
        if (idProducao.Length > 5)
        {
            Console.WriteLine("Identificador de produção inválido");
            return;
        }
        if (!ValidarQuantidade(quantidade))
        {
            Console.WriteLine("Quantidade Inválida");
        }
        CarregarArquivo();

        ItemProducao aux = new ItemProducao();
        aux.IdProducao = idProducao;
        aux.MatPrima = materiaPrima;
        aux.QuantidadeMateriaPrima = quantidade;
    }
    public void ImprimirMateriaPrima(string idProducao)
    {
        List<ItemProducao> ListaImpressa = new List<ItemProducao>();

        ListaImpressa = ListaItemProducao.FindAll(p => p.IdProducao == idProducao);
        for (int i = 0; i  < ListaImpressa.Count; i++)
        {
            Console.WriteLine($"ID da matéria-prima: {ListaImpressa[i].MatPrima}\tQuantidade Utilizada: {ListaImpressa[i].QuantidadeMateriaPrima}");
        }
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
