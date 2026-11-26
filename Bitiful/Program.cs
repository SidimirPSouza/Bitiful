
List<MateriaPrima> lista = MateriaPrima.buscarTodosRegistros();

foreach (MateriaPrima item in lista)
{
    Console.Write(item.ToString());
}

MateriaPrima.AtivarItem(3);
MateriaPrima.AtivarItem(4);

List<MateriaPrima> lista2 = MateriaPrima.buscarTodosRegistros();

foreach (MateriaPrima item in lista2)
{
    Console.Write(item.ToString());
}

Console.ReadKey();