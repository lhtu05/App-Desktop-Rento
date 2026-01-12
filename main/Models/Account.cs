namespace main.Models
{ 
    public class Account
        {
            public string UserName { get; set; }
            public string PasswordHash { get; set; }
            public Renter Renter { get; set; }
            public Host Host { get; set; }
            public bool Role { get; set; }
        }

}
