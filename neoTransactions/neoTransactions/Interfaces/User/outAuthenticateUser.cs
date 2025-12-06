using neoTools;

namespace neoTransactions.Interfaces.User
{
    public class outAuthenticateUser : outBaseSuperClass
    {
        public outAuthenticateUser(outBaseSuperClass response) : base(response.Scope) { }

        public int Id { get; set; }
        public int Id_type_user { get; set; }
        public int Id_application { get; set; }
        public string? Type_user { get; set; } 
        public string? Application { get; set; }
        public string? Username { get; set; }
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Image { get; set; }
        public string? TokenSingleUse { get; set; }
        //public string? TokenShortUse { get; set; }
        //public string? TokenLongUse { get; set; }

    }
}