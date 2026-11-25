MateriaPrima mp = new("pó de maquiagem");
List<MateriaPrima> lista = MateriaPrima.buscarTodosRegistros();

foreach (MateriaPrima item in lista)
{
    Console.Write(item.ToString());
}

Console.ReadKey();