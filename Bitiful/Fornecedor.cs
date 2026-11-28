public class Fornecedor
{
    public string Cnpj { get; private set; }
    public string RazaoSocial { get; private set; }
    public DateOnly DataAbertura { get; private set; }
    public DateOnly DataUltimaCompra { get; private set; } = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly DataCadastro { get; private set; } = DateOnly.FromDateTime(DateTime.Today);
    public ESituacao Situacao { get; private set; } = ESituacao.Ativo;

    // CONTRUTOR PARA CADASTRAR PELA PRIMEIRA VEZ O FORNECEDOR
    public Fornecedor(string cnpj, string rs, DateOnly dataAbertura)
    {
        Cnpj = cnpj;
        RazaoSocial = rs;
        DataAbertura = dataAbertura;
    }

    //CONSTRUTOR PARA PEGAR O FORNECEDOR DO ARQUIVO
    public Fornecedor(string cnpj, string rs, DateOnly dataAbertura, DateOnly dataUltimaCompra, DateOnly dataCadastro, ESituacao situacao)
    {
        Cnpj = cnpj;
        RazaoSocial = rs;
        DataAbertura = dataAbertura;
        DataUltimaCompra = dataUltimaCompra;
        DataCadastro = dataCadastro;
        Situacao = situacao;
    }


    //VALIDACOES
    private bool ValidarTamanhoRS(string razaoSocial)
    {
        return !string.IsNullOrWhiteSpace(razaoSocial)
        && razaoSocial.Length <= 50;
    }

    private bool ValidarDataAbertura(DateOnly dataAbertura)
    {
        return dataAbertura <= DateOnly.FromDateTime(DateTime.Today);

    }

    public bool ValidaCNPJCadastrado(string cnpj, List<Fornecedor> fornecedores)
    {
        var fornecedor = ProcurarFornecedor(cnpj, fornecedores);
        if (fornecedor is null)
        {
            return true;
        }
        else
            return false;
    }

    //ALTERACOES DOS DADOS
    public void AlterarDataAbertura(DateOnly novaData)
    {
        if (ValidarDataAbertura(novaData))
            DataAbertura = novaData;
    }

    public void AlterarRazaoSocial(string razaoSocial)
    {
        if (ValidarTamanhoRS(razaoSocial))
            RazaoSocial = razaoSocial;
    }

    //ALTERAR STATUS ATIVO E INATIVO DO FORNECEDOR
    public void AlterarStatusFornecedor(string cnpj, List<Fornecedor> fornecedores)
    {
        var fornecedor = ProcurarFornecedor(cnpj, fornecedores);

        if (fornecedor == null)
            return;

        if (fornecedor.Situacao == ESituacao.Ativo)
        {
            fornecedor.Situacao = ESituacao.Inativo;
        }
        else
        {
            fornecedor.Situacao = ESituacao.Ativo;
        }
    }


    //CRIANDO UM FORNECEDOR
    public Fornecedor CriarFornecedor(List<Fornecedor> fornecedores)
    {
        try
        {
            string cnpj = string.Empty;
            string razaoSocial = string.Empty;
            DateOnly dataAbertura;

            while (string.IsNullOrEmpty(cnpj))
            {
                //cadastrar cnpj
                Console.Write("Informe o CNPJ do Fornecedor: ");
                cnpj = Console.ReadLine();
                
                if (!Utils.ValidarCNPJ(cnpj))
                {
                    Console.WriteLine("CNPJ digitado está incorreto.");
                    cnpj = null;
                }
                
                else if (!ValidaCNPJCadastrado(cnpj, fornecedores))
                {
                    Console.WriteLine("CNPJ já cadastrado no sistema.");
                    cnpj = null;
                }
                
            }

            while (string.IsNullOrEmpty(razaoSocial))
            {
                Console.Write("Informe a Razão Social do Fornecedor: ");
                razaoSocial = Console.ReadLine();

                if (!ValidarTamanhoRS(razaoSocial))
                {
                    Console.WriteLine("O nome digitado é inválido ou supera o limite de 50 caracteres.");
                    razaoSocial = null;
                }
            }

            bool dataValida;
            do
            {
                Console.Write("Informe a Data de Abertura(dd/MM/yyyy): ");
                dataValida = DateOnly.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", out dataAbertura);
                if (dataValida)
                {
                    dataValida = ValidarDataAbertura(dataAbertura);
                }

                if (!dataValida)
                {
                    Console.WriteLine("Data de abertura inválida.");
                }
            } while (!dataValida);

            var fornecedor = new Fornecedor(cnpj, razaoSocial, dataAbertura);
            return fornecedor;
        }
        catch (Exception e)
        {
            throw new Exception("Houve um erro na criação do fornecedor: " + e.Message);
        }
    }


    //PROCURANDO UM FORNECEDOR PELO CNPJ
    public Fornecedor ProcurarFornecedor(string cnpj, List<Fornecedor> fornecedores)
    {
        if (cnpj is null || fornecedores is null)
            return null;


        foreach (var fornecedor in fornecedores)
        {
            if (cnpj.Equals(fornecedor.Cnpj))
                return fornecedor;
        }

        return null;

    }


    //REMOVER O FORNECEDOR PELO CNPJ
    public bool RemoverFornecedor(string cnpj, List<Fornecedor> fornecedores)
    {
        var fornecedor = ProcurarFornecedor(cnpj, fornecedores);
        if (fornecedor != null)
        {
            fornecedores.Remove(fornecedor);
            return true;
        }
        else
            return false;
    }


    //BLOQUEAR E DESBLOQUEAR O FORNECEDOR
    public bool BloquearFornecedor(string cnpj, List<Fornecedor> fornecedores, string CaminhoArquivoBloqueados)
    {
        try
        {
            var cnpjsBloqueados = new List<string>();

            if (Utils.ValidarCaminhoArquivo(CaminhoArquivoBloqueados))
                foreach (var c in File.ReadAllLines(CaminhoArquivoBloqueados))
                {
                    //pegando o cnpj do arquivo
                    string cnpjBloq = c.Trim();

                    //adicionando na lista o cnpj inativo
                    cnpjsBloqueados.Add(cnpjBloq);
                }

            //pegando o fornecedor da lista de fornecedores
            var fornecedor = ProcurarFornecedor(cnpj, fornecedores);

            if (fornecedor == null)
                return false;

            //verificando se ele ja esta bloqueado, senao ele add no arq de bloq
            if (!cnpjsBloqueados.Contains(fornecedor.Cnpj))
            {
                //adicionando o cnpj a lista de bloqueados
                cnpjsBloqueados.Add(fornecedor.Cnpj);

                var sw = new StreamWriter(CaminhoArquivoBloqueados);

                //passando pela lista de cnpj bloqueados e escrevendo em cada linha no arquivo
                foreach (var c in cnpjsBloqueados)
                {
                    sw.WriteLine(c);
                }
                sw.Close();

                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocorreu um erro ao BLOQUEAR os fornecedores: {e.Message}");
            return false;
        }
    }

    public bool DesbloquearFornecedor(string cnpj, List<Fornecedor> fornecedores, string CaminhoArquivoBloqueados)
    {
        try
        {
            var cnpjsBloqueados = new List<string>();

            if (Utils.ValidarCaminhoArquivo(CaminhoArquivoBloqueados))
                foreach (var c in File.ReadAllLines(CaminhoArquivoBloqueados))
                {
                    string cnpjBloq = c.Trim();
                    cnpjsBloqueados.Add(cnpjBloq);
                }

            var fornecedor = ProcurarFornecedor(cnpj, fornecedores);

            if (fornecedor == null)
                return false;

            //verifica se consegue remover o cnpj do arquivo
            if (cnpjsBloqueados.Remove(fornecedor.Cnpj))
            {
                //escreve a lista no arquivo
                File.WriteAllLines(CaminhoArquivoBloqueados, cnpjsBloqueados);
                return true;
            }

            return false;
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocorreu um erro ao DESBLOQUEAR o fornecedor: {e.Message}");
            return false;
        }
    }


    //PEGAR FORNECEDOR DO ARQUIVO e ENVIAR PARA UMA LISTA
    public static List<Fornecedor> CarregarFornecedores(string CaminhoArquivo)
    {
        var fornecedores = new List<Fornecedor>();
        try
        {
            if (Utils.ValidarCaminhoArquivo(CaminhoArquivo))
                foreach (var fornecedor in File.ReadAllLines(CaminhoArquivo))
                {
                    //pegando os dados do arquivo
                    string cnpj = fornecedor.Substring(0, 14).Trim();
                    string razaoSocial = fornecedor.Substring(14, 50).Trim();
                    DateOnly dataAbertura = DateOnly.ParseExact(fornecedor.Substring(64, 10), "dd/MM/yyyy");
                    DateOnly dataUltimaCompra = DateOnly.ParseExact(fornecedor.Substring(74, 10), "dd/MM/yyyy");
                    DateOnly dataCadastro = DateOnly.ParseExact(fornecedor.Substring(84, 10), "dd/MM/yyyy");
                    ESituacao situacao = Enum.Parse<ESituacao>(fornecedor.Substring(94).Trim());

                    //criando um objeto Fornecedor com os dados do arquivo
                    var novoFornecedor = new Fornecedor(cnpj, razaoSocial, dataAbertura, dataUltimaCompra, dataCadastro, situacao);
                    //adicionando na lista o novo objeto
                    fornecedores.Add(novoFornecedor);

                }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocorreu um erro ao carregar os fornecedores: {e.Message}");
        }
        //retorando a lista de fornecedores do arquivo
        return fornecedores;
    }


    //SALVAR FORNECEDOR DENTRO DO ARQUIVO
    public static void SalvarFornecedores(List<Fornecedor> fornecedores, string CaminhoArquivo)
    {
        try
        {
            var sw = new StreamWriter(CaminhoArquivo);

            //passando pela lista de fornecedores e escrevendo em cada linha no arquivo
            foreach (var fornecedor in fornecedores)
            {
                sw.WriteLine(fornecedor.ToFileString());
            }
            sw.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ao salvar o fornecedor houve um erro: {e.Message}");
        }
    }


    public override string ToString()
    {
        return $"Cnpj: {Cnpj.ToString()}" +
            $"\nRazao Social: {RazaoSocial}" +
            $"\nData Abertura: {DataAbertura}" +
            $"\nData Ultima Compra: {DataUltimaCompra}" +
            $"\nData Cadastro: {DataCadastro}" +
            $"\nSituação: {Situacao}";
    }

    public string ToFileString()
    {
        return $"{Cnpj.PadRight(14, ' ')}" +
            $"{RazaoSocial.PadRight(50, ' ')}" +
            $"{DataAbertura:dd/MM/yyyy}" +
            $"{DataUltimaCompra:dd/MM/yyyy}" +
            $"{DataCadastro:dd/MM/yyyy}" +
            $"{Situacao}";
    }


}

