

Console.WriteLine("Digite sua data de nascimento:");

DateOnly dataNascimento = DateOnly.ParseExact(Console.ReadLine(), "dd/MM/yyyy");
Console.ReadKey();