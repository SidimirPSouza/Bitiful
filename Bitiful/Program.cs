Producao teste = new Producao();

teste.Cadastrar();
teste.Cadastrar();
teste.Cadastrar();
teste.Cadastrar();



for (int i = 0; i < teste.Producoes.Count(); i++)
{
    Console.WriteLine(teste.Producoes[i]);
}
