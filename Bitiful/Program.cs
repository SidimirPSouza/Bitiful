
List<MateriaPrima> lista = MateriaPrima.buscarTodosRegistros();

foreach (MateriaPrima item in lista)
{
    Console.Write(item.ToString());
}

MateriaPrima.EditarNomePorId(4, "agora vai");

List<MateriaPrima> lista2 = MateriaPrima.buscarTodosRegistros();

foreach (MateriaPrima item in lista2)
{
    Console.Write(item.ToString());
}

Console.ReadKey();