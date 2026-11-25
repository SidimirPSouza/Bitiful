using System.Data;
using System;
using System.Collections.Specialized;

public class Cliente(string cpf, string nome, DateOnly dataNascimento, DateOnly ultimaCompra, DateOnly dataCadastro, char sexo) //,ESituacao)
{
    public string Cpf { get; private set; } = cpf;
    public string Nome { get; private set; } = nome;
    public DateOnly DataNascimento { get; private set; } = dataNascimento;
    public DateOnly UltimaCompra = DateOnly.FromDateTime(DateTime.Today);
    public DateOnly DataCadastro { get; private set; } = DateOnly.FromDateTime(DateTime.Today);
    public char Sexo { get; private set; } = char.ToUpper(sexo);



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
            throw new ArgumentException("Nome inválido.");
        }

        if (!ValidaTamanhoNome(nome))
        {
            throw new ArgumentException("Nome maior do que permitido.");
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
    public bool ValidaSexo(char sexo)
    {
        if (sexo == 'M' || sexo == 'F')
        {
            return true;
        }
        return false;

    }// verifica se sexo recebeu M ou F e retorna true caso sim, senão retorna false

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


    // Carrega Lista de clientes
    public static List<Cliente> CarregaClientes(string caminho)
    {
        caminho = @"C:\teste\teste.db";

        var clientes = new List<Cliente>();

        try
        {
            if (Utils.ValidarCaminhoArquivo(caminho))
            {
                foreach (var cliente in File.ReadAllLines(caminho))
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

                    char sexo = cliente[91];

                    var novoCliente = new Cliente(cpf, nome, dataNascimento, ultimaCompra, dataCadastro, sexo);

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
        caminho = @"C:\teste\teste.db";

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