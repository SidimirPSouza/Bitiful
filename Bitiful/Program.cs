var menu = new List<string>()
{
    "1 - Clientes",
    "2 - Fornecedores",
    "3 - Materia Prima",
    "4 - Produtos",
    "5 - Compras",
    "6 - Vendas",
    "0 - Encerrar"
};

void Iniciar()
{
    Console.Clear();
    while (true)
    {

        switch (Utils.CriarMenu(menu))
        {
            case '1':
                IniciarClientes();
                break;

            case '2':
                Console.WriteLine("Opção inválida!");
                break;

            case '3':
                Console.WriteLine("Opção inválida!");
                break;

            case '4':
                Console.WriteLine("Opção inválida!");
                break;

            case '0':
                Console.WriteLine("Opção inválida!");
                break;

            default:
                Console.WriteLine("Opção inválida!");
                break;
        }
    }
}
#region Clientes
var menuClientes = new List<string>()
{
    "1 - Cadastrar Cliente",
    "2 - Buscar Cliente",
    "3 - Editar Cliente",
    "4 - Ativar/Inativar Cliente",
    "5 - Clientes Inadimplentes",
    "0 - Voltar",
};

var menuClienteInadimplente = new List<string>()
{
    "1 - Inserir Cliente na lista de Inadimplencia",
    "2 - remover Cliente na lista de Inadimplencia",
    "3 - Buscar Cliente Inadimplente",
    "4 - Listar Clientes Inadimplentes",
    "0 - Voltar",
};

void IniciarClientes()
{
    var menuClientes = new List<string>()
    {
        "1 - Cadastrar Cliente",
        "2 - Buscar Cliente",
        "3 - Listar Clientes",
        "4 - Editar Cliente",
        "5 - Ativar/Inativar Cliente",
        "6 - Clientes Inadimplentes",
        "0 - Voltar",
    };

    var arquivo = "Clientes.dat";
    var clientes = Cliente.CarregaClientes(arquivo);
    Console.Clear();
    switch (Utils.CriarMenu(menuClientes))
    {
        case '1':
            Console.Clear();
            Cliente.AdicionaClientes(clientes);
            Cliente.SalvarListas(clientes, arquivo);
            break;

        case '2':
            Console.Clear();
            Console.WriteLine("Digite o CPF do cliente que deseja buscar");
            var cpf = Console.ReadLine();
            Cliente? cliente = clientes.Find(c => c.Cpf == cpf);
            Cliente.ExibirCliente(Cliente.BuscaCliente(clientes, cpf));
            break;

        case '3':
            Console.Clear();
            Utils.mostrarInformacoes(clientes);
            IniciarClientes();
            break;

        case '4':
            AlterarCliente(clientes, arquivo, null);
            break;

        case '5':
            Console.Clear();
            Console.WriteLine("Digite o CPF do cliente que deseja alterar");
            cpf = Console.ReadLine();
            cliente = clientes.Find(c => c.Cpf == cpf);

            var situacao = cliente.Situacao;
            cliente.AlterarSituacao(situacao);
            Console.WriteLine("Situação foi alterado com sucesso!");
            break;

        case '6':
            Console.Clear();
            var arquivoInadimplente = "Risco.dat";
            Console.WriteLine("Digite o CPF do cliente que deseja alterar");
            cpf = Console.ReadLine();
            var inadimplentes = Cliente.CarregaInadimplentes(arquivoInadimplente);
            Inadimplente(clientes, inadimplentes, cpf, arquivoInadimplente);
            break;

        case '0':
            Iniciar();
            break;

        default:
            Console.WriteLine("Opção inválida!");
            break;
    }

}

void Inadimplente(List<Cliente> clientes, List<Cliente> inadimplentes, string cpf, string arquivo)
{
    switch (Utils.CriarMenu(menuClienteInadimplente))
    {
        case '1':
            if (Cliente.AdicionaInadimplentes(clientes, inadimplentes, cpf))
            {
                Console.WriteLine("Cliente inadimplente adicionado com sucesso!");
                Cliente.SalvarListas(inadimplentes, arquivo);
            }
            break;

        case '2':
            if (Cliente.RemoveCliente(inadimplentes, cpf))
            {
                Console.WriteLine("Cliente inadimplente removido com sucesso!");
                Cliente.SalvarListas(inadimplentes, arquivo);
            }
            break;

        case '3':
            Cliente.ListaInadimplente(clientes, inadimplentes, cpf);
            break;

        case '4':
            var listagem = new List<Cliente>();
            foreach (var inadimplente in inadimplentes)
            {
                var cliente = clientes.Find(c => c.Cpf == inadimplente.Cpf);
                listagem.Add(cliente);
            }
            Utils.mostrarInformacoes(listagem);
            break;

        case '0':
            IniciarClientes();
            break;

        default:
            Console.WriteLine("Opção inválida!");
            break;
    }
}

void AlterarCliente(List<Cliente> clientes, string arquivo, string? cpf)
{
    Console.Clear();
    bool salvo = false;
    var menuAlteraClientes = new List<string>()
    {
        "1 - Alterar nome",
        "2 - Alterar data de nascimento",
        "3 - Alterar Sexo",
        "4 - Salvar",
        "5 - Mudar cliente a ser alterado",
        "0 - Voltar"
    };

    if (cpf == null)
    {
        do
        {
            Console.Clear();
            Console.WriteLine("Digite o CPF do cliente que deseja alterar");
            cpf = Console.ReadLine();
        }while(!Utils.ValidarCPF(cpf) && Utils.ValidaCPFCadastrado(clientes, cpf));
    }
    Cliente? cliente = clientes.Find(c => c.Cpf == cpf);
    switch (Utils.CriarMenu(menuAlteraClientes))
    {
        case '1':
            Console.WriteLine("Digite o novo nome");
            var nome = Console.ReadLine();
            cliente.AlterarNome(nome);
            Console.Clear();
            Console.WriteLine("Deseja continuar editando o cliente? (S/N)");
            var confirmacao = char.Parse(Console.ReadLine());
            AlterarCliente(clientes, arquivo, cpf);
            break;

        case '2':
            Console.WriteLine("Digite a nova data de nascimento");
            var nasc = DateOnly.ParseExact(Console.ReadLine(), "dd/MM/yyyy");
            cliente.AlterarDataNascimento(nasc);
            AlterarCliente(clientes, arquivo, cpf);
            break;

        case '3':
            Console.WriteLine("Digite o novo sexo");
            var sexo = char.Parse(Console.ReadLine());
            cliente.AlterarSexo(sexo);
            AlterarCliente(clientes, arquivo, cpf);
            break;

        case '4':
            Console.WriteLine("Deseja Salvar? (S/N)");
            confirmacao = char.Parse(Console.ReadLine());
            if (confirmacao == 'S')
            {
                Cliente.SalvarListas(clientes, arquivo);
            }
            AlterarCliente(clientes, arquivo, cpf);
            break;

        case '5':
            Console.WriteLine("Deseja alterar o cliente a ser modificado? (S/N)");
            confirmacao = char.Parse(Console.ReadLine());
            if (confirmacao == 'S')
            {
                AlterarCliente(clientes, arquivo, null);
            }
            AlterarCliente(clientes, arquivo, cpf);
            break;

        case '0':
            Console.Clear();
            if (!salvo)
            {
                Console.WriteLine("Tem certeza que deseja sair sem salvar? (S/N)");
                confirmacao = char.Parse(Console.ReadLine());
                if (confirmacao == 'N')
                {
                    Cliente.SalvarListas(clientes, arquivo);
                }
            }
            IniciarClientes();
            break;

        default:
            Console.WriteLine("Opção inválida!");
            break;
    }
}

#endregion

#region Fornecedor
var menuFornecedor = new List<string>()
{
    "1 - Cadastrar Fornecedor",
    "2 - Buscar Fornecedor",
    "3 - Editar Fornecedor",
    "4 - Ativar/Inativar Fornecedor",
    "5 - Fornecedores Bloqueados",
    "0 - Voltar",
};

var menuFornecedorBloqueado = new List<string>()
{
    "1 - Bloquear/Desbloquear Fornecedor",
    "2 - Buscar Fornecedor Bloqueado",
    "0 - Voltar",
};
#endregion

#region Materia Prima
//var menuMateriaPrima = new List<string>()
//{
//    "1 - Cadastrar Materia Prima",
//    "2 - Buscar Materia Prima",
//    "3 - Editar Materia Prima",
//    "4 - Ativar/Inativar Materia Prima",
//    "0 - Voltar",
//};
#endregion


Iniciar();