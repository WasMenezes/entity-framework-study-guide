namespace DominandoEFCore.Domain
{
    public class Funcionario
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string CPF { get; set; }
        public string RG { get; set; }
        public Departamento Departamento { get; set; }
    }
}
