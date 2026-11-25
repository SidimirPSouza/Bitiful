MateriaPrima mp = new("teste");
List<MateriaPrima> lista = MateriaPrima.buscarRegistrosAtivos();

foreach (MateriaPrima item in lista)
{
    Console.Write(item.ToString());
}

Console.ReadKey();