
List<MateriaPrima> lista = MateriaPrima.LocalizarPorChaveString("mp0001");

foreach (MateriaPrima item in lista)
{
    Console.Write(item.ToString());
}

Console.ReadKey();