
List<MateriaPrima> lista = MateriaPrima.buscarRegistrosInativos();

foreach (MateriaPrima item in lista)
{
    Console.Write(item.ToString());
}

Console.ReadKey();