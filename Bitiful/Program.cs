
List<MateriaPrima> lista = MateriaPrima.LocalizarPorChaveInt(4);

foreach (MateriaPrima item in lista)
{
    Console.Write(item.ToString());
}

Console.ReadKey();