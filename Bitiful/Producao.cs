public class Producao
{
    public static string Id { get; private set; }
    public DateOnly DataProducao { get; set; }
    public string Produto { get; set; }
    public int Quantidade { get; set; }
    public string Diretorio { get; private set; } = @"C:\dev\Biltiful\";
    public string Arquivo { get; private set; } = "Producao.dat";
    
    public List<string> Producoes = new List<string>();

    
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
        if (ArquivoExistente())
        {
            

            foreach(var item in File.ReadAllLines(Diretorio + Arquivo))
            {
                Producoes.Add(item);
            }
        }
    }

    public void SalvarArquivo()
    {
        if (ArquivoExistente())
        {
            var sw = new StreamWriter(Diretorio + Arquivo);
            foreach(var item in Producoes)
            {
                sw.WriteLine(item);
            }
            sw.Close();
        }
    }
    public void Cadastrar()
    {
        CarregarArquivo();
        if (ArquivoExistente())
        {
            if (Producoes.Count > 0)
            {
             

                int aux; //Auxiliar para conversão de tipo de dado
                aux = int.Parse(Producoes.Last()) + 1;
                Producoes.Add(aux.ToString().PadLeft(5,'0'));
                
            }
            else Producoes.Add("1".PadLeft(5,'0'));
        }
        SalvarArquivo();
    }

    public void Localizar()
    {

    }

    public void Excluir(string id)
    {
        CarregarArquivo();
        Producoes.Remove(id);
        SalvarArquivo();
    }
    public void ImpressaoRegistro()
    {

    }


}