public class MateriaPrima
{
    private int Id { get; set; }
    public string Nome { get; set; }
    private DateOnly UltimaCompra { get; set; }
    private DateOnly DataCadastro { get; set; }
    private char Situacao { get; set; }

    public MateriaPrima(string nome)
    {        
        Id = GerarNovoId();
        Nome = nome;
        UltimaCompra = DateOnly.FromDateTime(DateTime.Now);
        DataCadastro = DateOnly.FromDateTime(DateTime.Now);
        Situacao = 'A';

        string idString = ConverterIdFormatoString(Id);

        string linhaParaSalvar = idString+Nome+DataCadastro+UltimaCompra+Situacao;
        SaveSimple(linhaParaSalvar);
    }
    public int GerarNovoId()
    {
        var path = @"C:\dev\";
        var file = "MateriaPrima.dat";

        if (!File.Exists(path + file))
            using (File.Create(path + file)) { }

        int ultimoId = 0;

        string[] linhas = File.ReadAllLines(path + file);

        if (linhas.Length > 0)
        {
            string ultimaLinha = linhas[linhas.Length - 1];
            ultimoId = int.Parse(ultimaLinha.Substring(2, 4));
            return ultimoId + 1;
        }
        else
            return 1;

    }//funcional, utilizando no construtor
    void SaveSimple(string s)
    {
        var path = @"C:\dev\";
        var file = "MateriaPrima.dat";
        File.AppendAllText(path + file, s + Environment.NewLine);
    }
    string ConverterIdFormatoString(int n)
    {
        switch (n)
        {
            case < 10:
                return $"MP000{n}";
            case < 100:
                return $"MP00{n}";
            case < 1000:
                return $"MP0{n}";
            case < 10000:
                return $"MP{n}";
            default:
                return $"MP0000{n}";
        }

    }



}


