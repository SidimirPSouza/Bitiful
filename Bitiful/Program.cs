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
                IniciarFornecedor();
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
    var menuClienteInadimplente = new List<string>()
{
    "1 - Inserir Cliente na lista de Inadimplencia",
    "2 - remover Cliente na lista de Inadimplencia",
    "3 - Buscar Cliente Inadimplente",
    "4 - Listar Clientes Inadimplentes",
    "0 - Voltar",
};
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
        } while (!Utils.ValidarCPF(cpf) && Utils.ValidaCPFCadastrado(clientes, cpf));
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
void IniciarFornecedor()
{
    var menuFornecedores = new List<string>()
    {
        "1 - Cadastrar Fornecedor",
        "2 - Buscar Fornecedor",
        "3 - Listar Fornecedores",
        "4 - Editar Fornecedor",
        "5 - Ativar/Inativar Fornecedor",
        "6 - Fornecedores Bloqueados",
        "0 - Voltar",
    };

    var arquivo = "Fornecedores.dat";
    var caminhoArquivo = Utils.ValidarCaminhoArquivo(arquivo);
    var fornecedores = Fornecedor.CarregarFornecedores(caminhoArquivo);
    Console.Clear();
    switch (Utils.CriarMenu(menuFornecedores))
    {
        case '1':
            Console.Clear();
            fornecedores.Add(Fornecedor.CriarFornecedor(fornecedores));
            Fornecedor.SalvarFornecedores(fornecedores, caminhoArquivo);
            break;

        case '2':
            Console.Clear();
            Console.WriteLine("Digite o CNPJ do fornecedor que deseja buscar");
            var cnpj = Console.ReadLine();
            Fornecedor? fornecedor = fornecedores.Find(f => f.Cnpj == cnpj);
            Console.WriteLine(fornecedor.ToString());
            break;

        case '3':
            Console.Clear();
            Utils.mostrarInformacoes(fornecedores);
            IniciarFornecedor();
            break;

        case '4':
            AlterarFornecedor(fornecedores, caminhoArquivo, null);
            break;

        case '5':
            Console.Clear();
            Console.WriteLine("Digite o CNPJ do fornecedor que deseja alterar");
            cnpj = Console.ReadLine();
            Fornecedor.AlterarStatusFornecedor(cnpj, fornecedores);
            Fornecedor.SalvarFornecedores(fornecedores, caminhoArquivo);
            Console.WriteLine("Situação foi alterada com sucesso!");
            break;

        case '6':
            Console.Clear();
            var arquivoBloqueados = "Bloqueados.dat";
            arquivoBloqueados = Utils.ValidarCaminhoArquivo(arquivoBloqueados);
            Console.WriteLine("Digite o CNPJ do fornecedor que deseja alterar");
            cnpj = Console.ReadLine();
            var bloqueados = Fornecedor.CarregarBloqueados(arquivoBloqueados);
            Bloqueados(fornecedores, bloqueados, cnpj, arquivoBloqueados);
            break;

        case '0':
            Iniciar();
            break;

        default:
            Console.WriteLine("Opção inválida!");
            break;
    }

}

void Bloqueados(List<Fornecedor> fornecedores, List<Fornecedor> bloqueados, string cnpj, string arquivo)
{
    var menuFornecedorBloqueado = new List<string>()
{
    "1 - Bloquear Fornecedor",
    "2 - Desbloquear Fornecedor",
    "3 - Buscar Fornecedor Bloqueado",
    "4 - Listar Fornecedor Bloqueado",
    "0 - Voltar",
};
    switch (Utils.CriarMenu(menuFornecedorBloqueado))
    {
        case '1':
            if (Fornecedor.BloquearFornecedor(cnpj, fornecedores, arquivo))
            {
                Console.WriteLine("Fornecedor bloqueado com sucesso!");
            }
            break;

        case '2':
            if (Fornecedor.DesbloquearFornecedor(cnpj, bloqueados, arquivo))
            {
                Console.WriteLine("Fornecador desbloqueado com sucesso!");
            }
            break;

        case '3':
            var fornecedorBloqueado = bloqueados.Find(f => f.Cnpj == cnpj);
            Console.WriteLine(fornecedores.Find(f => f.Cnpj == fornecedorBloqueado.Cnpj).ToString()); 
            break;

        case '4':
            var listagem = new List<Fornecedor>();
            foreach (var bloqueado in bloqueados)
            {
                var fornecedor = fornecedores.Find(f => f.Cnpj == bloqueado.Cnpj);
                listagem.Add(fornecedor);
            }
            Utils.mostrarInformacoes(listagem);
            break;

        case '0':
            IniciarFornecedor();
            break;

        default:
            Console.WriteLine("Opção inválida!");
            break;
    }
}

void AlterarFornecedor(List<Fornecedor> fornecedores, string arquivo, string? cnpj)
{
    Console.Clear();
    bool salvo = false;
    var menuAlteraFornecedor = new List<string>()
    {
        "1 - Alterar razão social",
        "2 - Alterar data de abertura",
        "3 - Salvar",
        "4 - Mudar Fornecedor a ser alterado",
        "0 - Voltar"
    };

    if (cnpj == null)
    {
        do
        {
            Console.Clear();
            Console.WriteLine("Digite o CNPJ do Fornecedor que deseja alterar");
            cnpj = Console.ReadLine();
        } while (!Utils.ValidarCNPJ(cnpj));
        //&& Utils.ValidaCNPJCadastrado(fornedores, cnpj)
    }
    Fornecedor? fornecedor = fornecedores.Find(c => c.Cnpj == cnpj);
    switch (Utils.CriarMenu(menuAlteraFornecedor))
    {
        case '1':
            Console.WriteLine("Digite a nova razão social");
            var razao = Console.ReadLine();
            fornecedor.AlterarRazaoSocial(razao);
            Console.Clear();
            Console.WriteLine("Deseja continuar editando o fornecedor? (S/N)");
            var confirmacao = char.Parse(Console.ReadLine());
            if(confirmacao == 'S')
                AlterarFornecedor(fornecedores, arquivo, null);
            break;

        case '2':
            Console.WriteLine("Digite a nova data de abertura");
            var abertura = DateOnly.ParseExact(Console.ReadLine(), "dd/MM/yyyy");
            fornecedor.AlterarDataAbertura(abertura);
            Console.WriteLine("Deseja continuar editando o fornecedor? (S/N)");
            confirmacao = char.Parse(Console.ReadLine());
            if(confirmacao == 'S')
                AlterarFornecedor(fornecedores, arquivo, cnpj);
            AlterarFornecedor(fornecedores, arquivo, null);
            break;

        case '3':
            Console.WriteLine("Deseja Salvar? (S/N)");
            confirmacao = char.Parse(Console.ReadLine());
            if (confirmacao == 'S')
            {
                Fornecedor.SalvarFornecedores(fornecedores, arquivo);
            }
            AlterarFornecedor(fornecedores, arquivo, cnpj);
            break;

        case '4':
            Console.WriteLine("Deseja alterar o fornecedor a ser modificado? (S/N)");
            confirmacao = char.Parse(Console.ReadLine());
            if (confirmacao == 'S')
            {
                AlterarFornecedor(fornecedores, arquivo, null);
            }
            AlterarFornecedor(fornecedores, arquivo, cnpj);
            break;

        case '0':
            Console.Clear();
            if (!salvo)
            {
                Console.WriteLine("Tem certeza que deseja sair sem salvar? (S/N)");
                confirmacao = char.Parse(Console.ReadLine());
                if (confirmacao == 'N')
                {
                    Fornecedor.SalvarFornecedores(fornecedores, arquivo);
                }
            }
            IniciarFornecedor();
            break;

        default:
            Console.WriteLine("Opção inválida!");
            break;
    }
}

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