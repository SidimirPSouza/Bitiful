using System;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.InteropServices.JavaScript;
using System.Xml.Linq;

public class Cliente(string cpf, string nome, DateOnly dataNascimento, DateOnly ultimaCompra, DateOnly dataCadastro, char sexo, ESituacao situacao)
{
    public string Cpf { get; private set; } = cpf;
    public string Nome { get; private set; } = nome;
    public DateOnly DataNascimento { get; private set; } = dataNascimento;
    public DateOnly UltimaCompra = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly DataCadastro { get; private set; } = DateOnly.FromDateTime(DateTime.Today);
    public char Sexo { get; private set; } = char.ToUpper(sexo);
    public ESituacao Situacao { get; set; }

    public Cliente(string cpf) : this(cpf, string.Empty, default, default, default, ' ', 0)
    {
        this.Cpf = cpf;
    }


    //Validações das propriedades seguindo a regra de negócio

    public bool ValidaLetrasNome(string nome)
    {

        if (string.IsNullOrEmpty(nome))
        {
            return false;
        }

        for (int i = 0; i < nome.Length; i++)

        {
            if (!char.IsLetter(nome[i]) && nome[i] != ' ')

            {
                return false;
            }
        }
        return true;
    } // irá validar se o nome possui apenas letras e espaço 
    public bool ValidaTamanhoNome(string nome)
    {
        if (nome.Length > 50)
        {
            return false;
        }
        return true;
    } // irá validar se o tamanho do nome é menor ou igual a 50 se for retona true
    public bool ValidaNome(string nome)
    {
        if (!ValidaLetrasNome(nome))
        {
            return false;
        }

        if (!ValidaTamanhoNome(nome))
        {
            return false;
        }
        return true;


    }// verifica se os metodos ValidaTamanhoNome e ValidaLetrasNome retornam true, caso sim ele irá retonar true se não retorna false (acredito ser o ideal criar essa função pois depois chamamos apenas ela ao invés de ter que chamar as duas acima)
    public bool ValidaMaioridade(DateOnly dataNascimento) //se for maior de idade retorna true
    {
        if (2026 - dataNascimento.Year >= 18)
        {
            return true;
        }
        return false;
    }
    public bool ValidaDataNascimento(DateOnly dataNascimento)
    {
        if (dataNascimento.Year > 2026)
        {
            return false;
        }
        else
            return true;
    }
    public bool ValidaSexo(char sexo)
    {
        if (sexo == 'M' || sexo == 'F')
        {
            return true;
        }
        return false;

    }// verifica se sexo recebeu M ou F e retorna true caso sim, senão retorna false
    public ESituacao ValidaSituacao(ESituacao situacao, char status)
    {
        if (status == 'A')
        {
            return situacao = (ESituacao)status;
        }
        else
        {
            return situacao = (ESituacao)'I';
        }

    }
    public bool ValidaCPFCadastrado(List<Cliente> clientes, string cpf)
    {
        Cliente? cliente = clientes.Find(c => c.Cpf == cpf);

        if (cliente is null) // se o cliente for nulo
        {
            return true; //retorna true se o cliente não está cadastrado
        }
        else
            return false; //retorna false se ele já está cadastrado
    }



    // Alteração dos dados
    public void AlteraNome(string nome)
    {
        ValidaNome(nome);
        this.Nome = nome;

    } // Recebe o novo nome e chama função validar nome, se retonar true ele atribui o novo nome para a propriedade Nome
    public void AlteraDataNascimento(DateOnly dataNascimento)
    {
        this.DataNascimento = dataNascimento;
    }
    public void AlteraSexo(char sexo)
    {
        if (ValidaSexo(sexo))
        {
            this.Sexo = char.ToUpper(sexo);
        }
        else
        {
            throw new ArgumentException("Sexo inválido");
        }

    }
    public ESituacao AlteraSituacaoAtivo(ESituacao situacao, char novoStatus)
    {
        return situacao = (ESituacao)'A';
    } //Altera situacao para ativo
    public ESituacao AlteraSituacaoInativo(ESituacao situacao, char novoStatus)
    {
        return situacao = (ESituacao)'I';
    }//Altera situacao para inativo
    public bool AdicionaInadimplentes(List<Cliente> clientes, List<Cliente> inadimplentes, string cpf)
    {
        try
        {
            if (inadimplentes.Find(c => c.Cpf == cpf) is null) //receba um cliente c e retorne se o CPF dele é igual ao CPF procurado, se for nulo é porque ele não está na lista de inadimplentes.
            {
                Cliente? cliente = clientes.Find(c => c.Cpf == cpf); // cria um objeto do tipo cliente e irá receber os dados do objeto com o cpf 

                if (cliente is not null) // se existir na lista de cliente ele adiciona na lista de inadimplentes 
                {
                    inadimplentes.Add(cliente);
                    return true;
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao adicionar cliente inadimplente: {e.Message}");
        }
        return false;

    } // adiciona um cpf na lista de inadimplente
    public bool RemoveInadimplentes(List<Cliente> inadimplentes, string cpf)
    {
        try
        {
            Cliente? cliente = inadimplentes.Find(c => c.Cpf == cpf); //receba um cliente c e retorne se o CPF dele é igual ao CPF procurado.

            if (cliente is not null) // se o cliente não for nulo (ou seja se encontrar o cliente)
            {
                return inadimplentes.Remove(cliente); //remove o cliente da lista de inadimplentes
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Erro ao remover cliente inadimplente: {e.Message}");
        }
        return false;
    }
    public void AdicionaClientes(List<Cliente> clientes)
    {

        string? cpf = null;
        string? nome = null;
        DateOnly dataNascimento;
        DateOnly dataCadastro = DateOnly.FromDateTime(DateTime.Today);
        DateOnly ultimaCompra = DateOnly.FromDateTime(DateTime.Today);
        char sexo = ' ';
        char status;
        bool situacaoValida = false;
        bool dataValida = false;
        ESituacao situacao;



        while (string.IsNullOrEmpty(cpf))
        {
            Console.WriteLine("Digite seu CPF:");
            cpf = Console.ReadLine();

            if (!Utils.ValidarCPF(cpf))
            {
                Console.WriteLine("CPF digitado está incorreto.");
                cpf = null;
            }
            else if (!ValidaCPFCadastrado(clientes, cpf))
            {
                Console.WriteLine("CPF já cadastrado no sistema.");
                cpf = null;
            }
        }

        while (string.IsNullOrEmpty(nome))
        {
            Console.WriteLine("Digite seu nome:");
            nome = Console.ReadLine();

            if (!ValidaNome(nome))
            {
                Console.WriteLine("O nome digitado é inválido ou supera o limite de 50 caracteres.");
                nome = null;
            }
        }
        
        do
        {
            Console.WriteLine("Digite sua data de nascimento (DD/MM/AAAA):");
            dataValida = DateOnly.TryParseExact(Console.ReadLine(), "dd/MM/yyyy", out dataNascimento);
            if (!dataValida || !ValidaDataNascimento(dataNascimento))
            {
                Console.WriteLine("Data de nascimento inválida.");
            }
      

        } while (dataValida == false);

        while (sexo == ' ')
        {
            Console.WriteLine("Digite seu sexo (M -Masculino/F -Feminino)");
            char.TryParse(Console.ReadLine(), out sexo);
            sexo = char.ToUpper(sexo);

            if (!ValidaSexo(sexo))
            {
                Console.WriteLine("Sexo digitado está incorreto");
                sexo = ' ';
            }
        }

        do
        {
            Console.WriteLine("Digite a situação (A - Ativo | I - Inativo):");

            char.TryParse(Console.ReadLine(), out status);

            status = char.ToUpper(status);

            situacao = (ESituacao)status;

            situacaoValida = Enum.IsDefined(typeof(ESituacao), situacao);

            if (!situacaoValida)
            {
                Console.WriteLine("Valor digitado é inválido");
            }

        } while (!situacaoValida);


        var novoCliente = new Cliente(cpf, nome, dataNascimento, ultimaCompra, dataCadastro, sexo, situacao);
        clientes.Add(novoCliente);
    }



    //RemoveCliente faltando
    //BuscaDados faltando
    //ImpressãoPorRegistro faltando



    // Carrega Lista de clientes
    public static List<Cliente> CarregaClientes(string arquivo)
    {
        var clientes = new List<Cliente>();
        arquivo = "Clientes.dat";

        try
        {
            if (Utils.ValidarCaminhoArquivo(arquivo))
            {
                foreach (var cliente in File.ReadAllLines(arquivo))
                {
                    if (cliente.Length != 92)
                    {
                        Console.WriteLine("Registro inválido.");
                        continue;
                    }

                    string cpf = cliente.Substring(0, 11).Trim();

                    string nome = cliente.Substring(11, 50).Trim();

                    DateOnly dataNascimento = DateOnly.ParseExact(cliente.Substring(61, 10), "dd/MM/yyyy");

                    DateOnly ultimaCompra = DateOnly.ParseExact(cliente.Substring(71, 10), "dd/MM/yyyy");

                    DateOnly dataCadastro = DateOnly.ParseExact(cliente.Substring(81, 10), "dd/MM/yyyy");

                    string sexo = cliente.Substring(91, 1);
                    char caractereSexo = char.Parse(sexo);


                    string status = cliente.Substring(92, 1);
                    char caractereSitaucao = char.Parse(status);
                    ESituacao situacao = (ESituacao)caractereSitaucao;


                    var novoCliente = new Cliente(cpf, nome, dataNascimento, ultimaCompra, dataCadastro, caractereSexo, situacao);

                    clientes.Add(novoCliente);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocorreu um erro ao carregar os clientes: {e.Message}");
        }

        return clientes;
    }

    //Salvar lista de cliente no arquivo
    public static void SalvarClientes(List<Cliente> clientes, string caminho)
    {
        try
        {
            var diretorio = Path.GetDirectoryName(caminho);

            if (!Directory.Exists(diretorio))
            {
                Directory.CreateDirectory(diretorio);
            }
            using (var sw = new StreamWriter(caminho))
            {
                foreach (var cliente in clientes)
                {
                    sw.WriteLine(cliente.ToFile());
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocorreu um erro ao salvar os contatos: {e.Message}");
        }
    }

    // Carrega Inadimplentes
    public static List<Cliente> CarregaInadimplentes(string arquivo)
    {
        var clientes = new List<Cliente>();
        arquivo = "Risco.dat";

        try
        {
            if (Utils.ValidarCaminhoArquivo(arquivo))
            {
                foreach (var cliente in File.ReadAllLines(arquivo))
                {
                    if (cliente.Length != 92)
                    {
                        Console.WriteLine("Registro inválido.");
                        continue;
                    }

                    string cpf = cliente.Substring(0, 11).Trim();

                    var novoCliente = new Cliente(cpf);

                    clientes.Add(novoCliente);
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine($"Ocorreu um erro ao carregar os clientes: {e.Message}");
        }

        return clientes;
    }


    //Formatação para o arquivo
    public string ToFile()
    {

        return $"{Cpf.PadRight(11, ' ')}" +
            $"{Nome.PadRight(50, ' ')}" +
            $"{DataNascimento.ToString("dd/MM/yyyy")}" +
            $"{UltimaCompra.ToString("dd/MM/yyyy")}" +
            $"{DataCadastro.ToString("dd/MM/yyyy")}" +
            $"{Sexo}";
    }


}